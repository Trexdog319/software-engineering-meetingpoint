namespace PcapAnalyzer.Core.Models;

/// <summary>
/// Represents the complete analysis result of a PCAP file
/// </summary>
public class AnalysisResult
{
    /// <summary>
    /// Path to the analyzed PCAP file
    /// </summary>
    public string FilePath { get; set; } = string.Empty;
    
    /// <summary>
    /// When the analysis was performed
    /// </summary>
    public DateTime AnalysisTimestamp { get; set; }
    
    /// <summary>
    /// Collection of all packets in the capture
    /// </summary>
    public List<PacketInfo> Packets { get; set; } = new();
    
    /// <summary>
    /// Statistical information about the capture
    /// </summary>
    public Statistics Statistics { get; set; } = new();
    
    /// <summary>
    /// Total number of packets analyzed
    /// </summary>
    public int TotalPackets => Packets.Count;
    
    /// <summary>
    /// Any errors or warnings encountered during analysis
    /// </summary>
    public List<string> Warnings { get; set; } = new();
}
