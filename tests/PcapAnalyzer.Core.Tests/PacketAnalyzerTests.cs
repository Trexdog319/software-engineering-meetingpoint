using PcapAnalyzer.Core.Models;
using PcapAnalyzer.Core.Services;
using Xunit;

namespace PcapAnalyzer.Core.Tests;

public class PacketAnalyzerTests
{
    private readonly PacketAnalyzer _analyzer;

    public PacketAnalyzerTests()
    {
        _analyzer = new PacketAnalyzer();
    }

    [Fact]
    public void CalculateStatistics_EmptyPacketList_ReturnsEmptyStatistics()
    {
        // Arrange
        var packets = new List<PacketInfo>();

        // Act
        var stats = _analyzer.CalculateStatistics(packets);

        // Assert
        Assert.NotNull(stats);
        Assert.Equal(0, stats.TotalPackets);
        Assert.Equal(0, stats.TotalBytes);
    }

    [Fact]
    public void CalculateStatistics_ValidPackets_ReturnsCorrectCounts()
    {
        // Arrange
        var packets = new List<PacketInfo>
        {
            new PacketInfo { FrameNumber = 1, PacketSize = 100, Protocol = "TCP", IpVersion = IPVersion.IPv4 },
            new PacketInfo { FrameNumber = 2, PacketSize = 200, Protocol = "UDP", IpVersion = IPVersion.IPv4 },
            new PacketInfo { FrameNumber = 3, PacketSize = 150, Protocol = "TCP", IpVersion = IPVersion.IPv6 }
        };

        // Act
        var stats = _analyzer.CalculateStatistics(packets);

        // Assert
        Assert.Equal(3, stats.TotalPackets);
        Assert.Equal(450, stats.TotalBytes);
        Assert.Equal(150, stats.AveragePacketSize);
        Assert.Equal(100, stats.MinPacketSize);
        Assert.Equal(200, stats.MaxPacketSize);
    }

    [Fact]
    public void CalculateStatistics_ValidPackets_ReturnsCorrectProtocolDistribution()
    {
        // Arrange
        var packets = new List<PacketInfo>
        {
            new PacketInfo { Protocol = "TCP" },
            new PacketInfo { Protocol = "TCP" },
            new PacketInfo { Protocol = "UDP" }
        };

        // Act
        var stats = _analyzer.CalculateStatistics(packets);

        // Assert
        Assert.Equal(2, stats.ProtocolCounts["TCP"]);
        Assert.Equal(1, stats.ProtocolCounts["UDP"]);
    }

    [Fact]
    public void FilterByProtocol_TcpProtocol_ReturnsOnlyTcpPackets()
    {
        // Arrange
        var packets = new List<PacketInfo>
        {
            new PacketInfo { Protocol = "TCP" },
            new PacketInfo { Protocol = "UDP" },
            new PacketInfo { Protocol = "TCP" }
        };

        // Act
        var filtered = _analyzer.FilterByProtocol(packets, "TCP").ToList();

        // Assert
        Assert.Equal(2, filtered.Count);
        Assert.All(filtered, p => Assert.Equal("TCP", p.Protocol));
    }

    [Fact]
    public void FilterByIpVersion_IPv4_ReturnsOnlyIPv4Packets()
    {
        // Arrange
        var packets = new List<PacketInfo>
        {
            new PacketInfo { IpVersion = IPVersion.IPv4 },
            new PacketInfo { IpVersion = IPVersion.IPv6 },
            new PacketInfo { IpVersion = IPVersion.IPv4 }
        };

        // Act
        var filtered = _analyzer.FilterByIpVersion(packets, IPVersion.IPv4).ToList();

        // Assert
        Assert.Equal(2, filtered.Count);
        Assert.All(filtered, p => Assert.Equal(IPVersion.IPv4, p.IpVersion));
    }

    [Fact]
    public void FilterByIpAddress_SourceOrDestination_ReturnsMatchingPackets()
    {
        // Arrange
        var testIp = "192.168.1.1";
        var packets = new List<PacketInfo>
        {
            new PacketInfo { SourceAddress = testIp, DestinationAddress = "10.0.0.1" },
            new PacketInfo { SourceAddress = "10.0.0.2", DestinationAddress = testIp },
            new PacketInfo { SourceAddress = "10.0.0.3", DestinationAddress = "10.0.0.4" }
        };

        // Act
        var filtered = _analyzer.FilterByIpAddress(packets, testIp).ToList();

        // Assert
        Assert.Equal(2, filtered.Count);
    }
}
