namespace PcapAnalyzer.Core.Models;

/// <summary>
/// Represents detailed information about a network packet
/// </summary>
public class PacketInfo
{
    /// <summary>
    /// Sequential frame number in the capture
    /// </summary>
    public int FrameNumber { get; set; }
    
    /// <summary>
    /// Timestamp when the packet was captured
    /// </summary>
    public DateTime Timestamp { get; set; }
    
    /// <summary>
    /// Total size of the packet in bytes
    /// </summary>
    public int PacketSize { get; set; }
    
    /// <summary>
    /// Source IP address
    /// </summary>
    public string SourceAddress { get; set; } = string.Empty;
    
    /// <summary>
    /// Destination IP address
    /// </summary>
    public string DestinationAddress { get; set; } = string.Empty;
    
    /// <summary>
    /// Network protocol (TCP, UDP, ICMP, etc.)
    /// </summary>
    public string Protocol { get; set; } = string.Empty;
    
    /// <summary>
    /// IP version (IPv4 or IPv6)
    /// </summary>
    public IPVersion IpVersion { get; set; }
    
    /// <summary>
    /// Source port number (if applicable)
    /// </summary>
    public int? SourcePort { get; set; }
    
    /// <summary>
    /// Destination port number (if applicable)
    /// </summary>
    public int? DestinationPort { get; set; }
    
    /// <summary>
    /// Raw payload data
    /// </summary>
    public byte[] Payload { get; set; } = Array.Empty<byte>();
    
    /// <summary>
    /// Payload data as hexadecimal string
    /// </summary>
    public string PayloadHex => BitConverter.ToString(Payload).Replace("-", " ");
    
    /// <summary>
    /// Payload data as ASCII string (printable characters only)
    /// </summary>
    public string PayloadAscii => new string(
        Payload.Select(b => b >= 32 && b <= 126 ? (char)b : '.').ToArray()
    );
    
    /// <summary>
    /// Additional packet information or flags
    /// </summary>
    public string Info { get; set; } = string.Empty;
}
