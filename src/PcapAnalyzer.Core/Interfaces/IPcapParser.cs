using PcapAnalyzer.Core.Models;

namespace PcapAnalyzer.Core.Interfaces;

/// <summary>
/// Interface for parsing PCAP files
/// </summary>
public interface IPcapParser
{
    /// <summary>
    /// Parse a PCAP file and extract packet information
    /// </summary>
    /// <param name="filePath">Path to the PCAP file</param>
    /// <returns>Collection of packet information</returns>
    IEnumerable<PacketInfo> ParseFile(string filePath);
    
    /// <summary>
    /// Analyze a PCAP file and generate complete analysis result
    /// </summary>
    /// <param name="filePath">Path to the PCAP file</param>
    /// <returns>Complete analysis result with statistics</returns>
    AnalysisResult AnalyzeFile(string filePath);
    
    /// <summary>
    /// Validate if a file is a valid PCAP file
    /// </summary>
    /// <param name="filePath">Path to the file to validate</param>
    /// <returns>True if valid PCAP file, false otherwise</returns>
    bool IsValidPcapFile(string filePath);
}
