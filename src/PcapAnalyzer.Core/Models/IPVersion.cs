namespace PcapAnalyzer.Core.Models;

/// <summary>
/// Enumeration for IP protocol versions
/// </summary>
public enum IPVersion
{
    /// <summary>
    /// Unknown or unsupported IP version
    /// </summary>
    Unknown = 0,
    
    /// <summary>
    /// Internet Protocol version 4
    /// </summary>
    IPv4 = 4,
    
    /// <summary>
    /// Internet Protocol version 6
    /// </summary>
    IPv6 = 6
}
