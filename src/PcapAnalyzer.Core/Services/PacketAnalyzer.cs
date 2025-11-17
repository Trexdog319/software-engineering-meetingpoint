using PcapAnalyzer.Core.Interfaces;
using PcapAnalyzer.Core.Models;

namespace PcapAnalyzer.Core.Services;

/// <summary>
/// Implementation of packet analysis functionality
/// </summary>
public class PacketAnalyzer : IPacketAnalyzer
{
    /// <inheritdoc/>
    public Statistics CalculateStatistics(IEnumerable<PacketInfo> packets)
    {
        var packetList = packets.ToList();
        
        if (!packetList.Any())
        {
            return new Statistics();
        }
        
        var stats = new Statistics
        {
            TotalPackets = packetList.Count,
            TotalBytes = packetList.Sum(p => (long)p.PacketSize),
            AveragePacketSize = packetList.Average(p => p.PacketSize),
            MinPacketSize = packetList.Min(p => p.PacketSize),
            MaxPacketSize = packetList.Max(p => p.PacketSize),
            FirstPacketTime = packetList.Min(p => p.Timestamp),
            LastPacketTime = packetList.Max(p => p.Timestamp)
        };
        
        // Calculate protocol distribution
        stats.ProtocolCounts = packetList
            .GroupBy(p => p.Protocol)
            .ToDictionary(g => g.Key, g => g.Count());
        
        // Calculate IP version distribution
        stats.IpVersionCounts = packetList
            .GroupBy(p => p.IpVersion)
            .ToDictionary(g => g.Key, g => g.Count());
        
        // Top 10 source addresses
        stats.TopSourceAddresses = packetList
            .GroupBy(p => p.SourceAddress)
            .OrderByDescending(g => g.Count())
            .Take(10)
            .ToDictionary(g => g.Key, g => g.Count());
        
        // Top 10 destination addresses
        stats.TopDestinationAddresses = packetList
            .GroupBy(p => p.DestinationAddress)
            .OrderByDescending(g => g.Count())
            .Take(10)
            .ToDictionary(g => g.Key, g => g.Count());
        
        return stats;
    }
    
    /// <inheritdoc/>
    public IEnumerable<PacketInfo> FilterByProtocol(IEnumerable<PacketInfo> packets, string protocol)
    {
        return packets.Where(p => p.Protocol.Equals(protocol, StringComparison.OrdinalIgnoreCase));
    }
    
    /// <inheritdoc/>
    public IEnumerable<PacketInfo> FilterByIpVersion(IEnumerable<PacketInfo> packets, IPVersion version)
    {
        return packets.Where(p => p.IpVersion == version);
    }
    
    /// <inheritdoc/>
    public IEnumerable<PacketInfo> FilterByIpAddress(IEnumerable<PacketInfo> packets, string ipAddress)
    {
        return packets.Where(p => 
            p.SourceAddress.Equals(ipAddress, StringComparison.OrdinalIgnoreCase) ||
            p.DestinationAddress.Equals(ipAddress, StringComparison.OrdinalIgnoreCase));
    }
}
