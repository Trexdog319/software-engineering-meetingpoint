namespace PcapAnalyzer.Core.Models;

/// <summary>
/// Contains statistical information about analyzed packets
/// </summary>
public class Statistics
{
    /// <summary>
    /// Total number of packets
    /// </summary>
    public int TotalPackets { get; set; }
    
    /// <summary>
    /// Total bytes across all packets
    /// </summary>
    public long TotalBytes { get; set; }
    
    /// <summary>
    /// Average packet size in bytes
    /// </summary>
    public double AveragePacketSize { get; set; }
    
    /// <summary>
    /// Smallest packet size in bytes
    /// </summary>
    public int MinPacketSize { get; set; }
    
    /// <summary>
    /// Largest packet size in bytes
    /// </summary>
    public int MaxPacketSize { get; set; }
    
    /// <summary>
    /// Count of packets by protocol
    /// </summary>
    public Dictionary<string, int> ProtocolCounts { get; set; } = new();
    
    /// <summary>
    /// Count of IPv4 vs IPv6 packets
    /// </summary>
    public Dictionary<IPVersion, int> IpVersionCounts { get; set; } = new();
    
    /// <summary>
    /// Top source addresses by packet count
    /// </summary>
    public Dictionary<string, int> TopSourceAddresses { get; set; } = new();
    
    /// <summary>
    /// Top destination addresses by packet count
    /// </summary>
    public Dictionary<string, int> TopDestinationAddresses { get; set; } = new();
    
    /// <summary>
    /// First packet timestamp
    /// </summary>
    public DateTime? FirstPacketTime { get; set; }
    
    /// <summary>
    /// Last packet timestamp
    /// </summary>
    public DateTime? LastPacketTime { get; set; }
    
    /// <summary>
    /// Duration of the capture
    /// </summary>
    public TimeSpan? CaptureDuration => 
        FirstPacketTime.HasValue && LastPacketTime.HasValue 
            ? LastPacketTime.Value - FirstPacketTime.Value 
            : null;
}
