using PcapAnalyzer.Core.Models;
using PcapAnalyzer.Core.Services;
using Xunit;

namespace PcapAnalyzer.Core.Tests;

public class DataExporterTests
{
    private readonly DataExporter _exporter;
    private readonly string _testOutputPath;

    public DataExporterTests()
    {
        _exporter = new DataExporter();
        _testOutputPath = Path.Combine(Path.GetTempPath(), "PcapAnalyzerTests");
        Directory.CreateDirectory(_testOutputPath);
    }

    [Fact]
    public void ExportToJson_ValidData_CreatesFile()
    {
        // Arrange
        var result = CreateSampleAnalysisResult();
        var outputFile = Path.Combine(_testOutputPath, "test_export.json");

        // Act
        _exporter.ExportToJson(result, outputFile);

        // Assert
        Assert.True(File.Exists(outputFile));
        var content = File.ReadAllText(outputFile);
        Assert.Contains("\"TotalPackets\": 2", content);

        // Cleanup
        File.Delete(outputFile);
    }

    [Fact]
    public void ExportToCsv_ValidData_CreatesFile()
    {
        // Arrange
        var result = CreateSampleAnalysisResult();
        var outputFile = Path.Combine(_testOutputPath, "test_export.csv");

        // Act
        _exporter.ExportToCsv(result, outputFile);

        // Assert
        Assert.True(File.Exists(outputFile));
        var content = File.ReadAllText(outputFile);
        Assert.Contains("FrameNumber,Timestamp", content);

        // Cleanup
        File.Delete(outputFile);
    }

    [Fact]
    public void ExportToHtml_ValidData_CreatesFile()
    {
        // Arrange
        var result = CreateSampleAnalysisResult();
        var outputFile = Path.Combine(_testOutputPath, "test_export.html");

        // Act
        _exporter.ExportToHtml(result, outputFile);

        // Assert
        Assert.True(File.Exists(outputFile));
        var content = File.ReadAllText(outputFile);
        Assert.Contains("<!DOCTYPE html>", content);
        Assert.Contains("PCAP Analysis Report", content);

        // Cleanup
        File.Delete(outputFile);
    }

    private AnalysisResult CreateSampleAnalysisResult()
    {
        return new AnalysisResult
        {
            FilePath = "test.pcap",
            AnalysisTimestamp = DateTime.Now,
            Packets = new List<PacketInfo>
            {
                new PacketInfo
                {
                    FrameNumber = 1,
                    Timestamp = DateTime.Now,
                    PacketSize = 100,
                    SourceAddress = "192.168.1.1",
                    DestinationAddress = "192.168.1.2",
                    Protocol = "TCP",
                    IpVersion = IPVersion.IPv4
                },
                new PacketInfo
                {
                    FrameNumber = 2,
                    Timestamp = DateTime.Now,
                    PacketSize = 200,
                    SourceAddress = "192.168.1.2",
                    DestinationAddress = "192.168.1.1",
                    Protocol = "UDP",
                    IpVersion = IPVersion.IPv4
                }
            },
            Statistics = new Statistics
            {
                TotalPackets = 2,
                TotalBytes = 300
            }
        };
    }
}
