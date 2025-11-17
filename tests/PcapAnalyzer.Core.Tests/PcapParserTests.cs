using PcapAnalyzer.Core.Services;
using Xunit;

namespace PcapAnalyzer.Core.Tests;

public class PcapParserTests
{
    [Fact]
    public void IsValidPcapFile_NonExistentFile_ReturnsFalse()
    {
        // Arrange
        var parser = new PcapParser(new PacketAnalyzer());
        var nonExistentFile = "nonexistent_file.pcap";

        // Act
        var result = parser.IsValidPcapFile(nonExistentFile);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ParseFile_NonExistentFile_ThrowsFileNotFoundException()
    {
        // Arrange
        var parser = new PcapParser(new PacketAnalyzer());
        var nonExistentFile = "nonexistent_file.pcap";

        // Act & Assert
        Assert.Throws<FileNotFoundException>(() => parser.ParseFile(nonExistentFile));
    }

    // Note: Additional tests requiring actual PCAP files should be placed in integration tests
    // or use test data files in the TestData folder
}
