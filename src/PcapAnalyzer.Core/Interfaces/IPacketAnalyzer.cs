using PcapAnalyzer.Core.Models;

namespace PcapAnalyzer.Core.Interfaces;

/// <summary>
/// Interface for analyzing network packets
/// </summary>
public interface IPacketAnalyzer
{
    /// <summary>
    /// Calculate statistics from a collection of packets
    /// </summary>
    /// <param name="packets">Collection of packets to analyze</param>
    /// <returns>Statistical analysis of the packets</returns>
    Statistics CalculateStatistics(IEnumerable<PacketInfo> packets);
    
    /// <summary>
    /// Filter packets by protocol
    /// </summary>
    /// <param name="packets">Collection of packets</param>
    /// <param name="protocol">Protocol to filter by</param>
    /// <returns>Filtered packets</returns>
    IEnumerable<PacketInfo> FilterByProtocol(IEnumerable<PacketInfo> packets, string protocol);
    
    /// <summary>
    /// Filter packets by IP version
    /// </summary>
    /// <param name="packets">Collection of packets</param>
    /// <param name="version">IP version to filter by</param>
    /// <returns>Filtered packets</returns>
    IEnumerable<PacketInfo> FilterByIpVersion(IEnumerable<PacketInfo> packets, IPVersion version);
    
    /// <summary>
    /// Filter packets by IP address (source or destination)
    /// </summary>
    /// <param name="packets">Collection of packets</param>
    /// <param name="ipAddress">IP address to filter by</param>
    /// <returns>Filtered packets</returns>
    IEnumerable<PacketInfo> FilterByIpAddress(IEnumerable<PacketInfo> packets, string ipAddress);
}
