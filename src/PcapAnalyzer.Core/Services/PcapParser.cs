using PcapAnalyzer.Core.Interfaces;
using PcapAnalyzer.Core.Models;
using SharpPcap;
using SharpPcap.LibPcap;
using PacketDotNet;

namespace PcapAnalyzer.Core.Services;

/// <summary>
/// Implementation of PCAP file parser using SharpPcap
/// </summary>
public class PcapParser : IPcapParser
{
    private readonly IPacketAnalyzer _packetAnalyzer;
    
    public PcapParser(IPacketAnalyzer packetAnalyzer)
    {
        _packetAnalyzer = packetAnalyzer;
    }
    
    /// <inheritdoc/>
    public IEnumerable<PacketInfo> ParseFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"PCAP file not found: {filePath}");
        }
        
        var packets = new List<PacketInfo>();
        
        try
        {
            using var device = new CaptureFileReaderDevice(filePath);
            device.Open();
            
            int frameNumber = 1;
            PacketCapture capture;
            
            // Read packets one at a time
            while (device.GetNextPacket(out capture) == GetPacketStatus.PacketRead)
            {
                try
                {
                    var rawCapture = capture.GetPacket();
                    if (rawCapture != null)
                    {
                        var packetInfo = ParsePacket(rawCapture, frameNumber);
                        if (packetInfo != null)
                        {
                            packets.Add(packetInfo);
                        }
                        frameNumber++;
                    }
                }
                catch (Exception ex)
                {
                    // Log packet parsing error but continue
                    packets.Add(new PacketInfo
                    {
                        FrameNumber = frameNumber,
                        Timestamp = DateTime.Now,
                        Protocol = "Error",
                        Info = $"Error parsing packet: {ex.Message}"
                    });
                    frameNumber++;
                }
            }
            
            device.Close();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error parsing PCAP file: {ex.Message}", ex);
        }
        
        return packets;
    }
    
    /// <inheritdoc/>
    public AnalysisResult AnalyzeFile(string filePath)
    {
        var result = new AnalysisResult
        {
            FilePath = filePath,
            AnalysisTimestamp = DateTime.Now
        };
        
        try
        {
            result.Packets = ParseFile(filePath).ToList();
            result.Statistics = _packetAnalyzer.CalculateStatistics(result.Packets);
        }
        catch (Exception ex)
        {
            result.Warnings.Add($"Error during analysis: {ex.Message}");
        }
        
        return result;
    }
    
    /// <inheritdoc/>
    public bool IsValidPcapFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return false;
        }
        
        try
        {
            using var device = new CaptureFileReaderDevice(filePath);
            device.Open();
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    /// <summary>
    /// Parse a raw packet into PacketInfo object
    /// </summary>
    private PacketInfo? ParsePacket(RawCapture rawCapture, int frameNumber)
    {
        try
        {
            var packet = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);
            if (packet == null)
            {
                return new PacketInfo
                {
                    FrameNumber = frameNumber,
                    Timestamp = rawCapture.Timeval.Date,
                    PacketSize = rawCapture.Data.Length,
                    Protocol = "Unknown",
                    SourceAddress = "N/A",
                    DestinationAddress = "N/A",
                    IpVersion = Models.IPVersion.Unknown,
                    Info = "Could not parse packet"
                };
            }
            
            // Try to extract IP packet (IPv4 or IPv6)
            var ipv4Packet = packet.Extract<IPv4Packet>();
            var ipv6Packet = packet.Extract<IPv6Packet>();
            
            // If no IP packet, try to show Ethernet info
            if (ipv4Packet == null && ipv6Packet == null)
            {
                var ethernetPacket = packet.Extract<EthernetPacket>();
                if (ethernetPacket != null)
                {
                    return new PacketInfo
                    {
                        FrameNumber = frameNumber,
                        Timestamp = rawCapture.Timeval.Date,
                        PacketSize = rawCapture.Data.Length,
                        SourceAddress = ethernetPacket.SourceHardwareAddress?.ToString() ?? "N/A",
                        DestinationAddress = ethernetPacket.DestinationHardwareAddress?.ToString() ?? "N/A",
                        Protocol = ethernetPacket.Type.ToString(),
                        IpVersion = Models.IPVersion.Unknown,
                        Info = $"Non-IP: {ethernetPacket.Type}"
                    };
                }
                
                return new PacketInfo
                {
                    FrameNumber = frameNumber,
                    Timestamp = rawCapture.Timeval.Date,
                    PacketSize = rawCapture.Data.Length,
                    SourceAddress = "N/A",
                    DestinationAddress = "N/A",
                    Protocol = "Non-IP",
                    IpVersion = Models.IPVersion.Unknown,
                    Info = "Non-IP packet"
                };
            }
            
            var packetInfo = new PacketInfo
            {
                FrameNumber = frameNumber,
                Timestamp = rawCapture.Timeval.Date,
                PacketSize = rawCapture.Data.Length
            };
            
            // Handle IPv4
            if (ipv4Packet != null)
            {
                packetInfo.SourceAddress = ipv4Packet.SourceAddress?.ToString() ?? "N/A";
                packetInfo.DestinationAddress = ipv4Packet.DestinationAddress?.ToString() ?? "N/A";
                packetInfo.IpVersion = Models.IPVersion.IPv4;
                
                // Extract protocol and port information
                var tcpPacket = packet.Extract<TcpPacket>();
                var udpPacket = packet.Extract<UdpPacket>();
                var icmpPacket = packet.Extract<IcmpV4Packet>();
                
                if (tcpPacket != null)
                {
                    packetInfo.Protocol = "TCP";
                    packetInfo.SourcePort = tcpPacket.SourcePort;
                    packetInfo.DestinationPort = tcpPacket.DestinationPort;
                    packetInfo.Payload = tcpPacket.PayloadData ?? Array.Empty<byte>();
                    packetInfo.Info = $"TCP {tcpPacket.SourcePort} → {tcpPacket.DestinationPort}";
                }
                else if (udpPacket != null)
                {
                    packetInfo.Protocol = "UDP";
                    packetInfo.SourcePort = udpPacket.SourcePort;
                    packetInfo.DestinationPort = udpPacket.DestinationPort;
                    packetInfo.Payload = udpPacket.PayloadData ?? Array.Empty<byte>();
                    packetInfo.Info = $"UDP {udpPacket.SourcePort} → {udpPacket.DestinationPort}";
                }
                else if (icmpPacket != null)
                {
                    packetInfo.Protocol = "ICMPv4";
                    packetInfo.Payload = icmpPacket.PayloadData ?? Array.Empty<byte>();
                    packetInfo.Info = $"ICMP Type: {icmpPacket.TypeCode}";
                }
                else
                {
                    packetInfo.Protocol = ipv4Packet.Protocol.ToString();
                    packetInfo.Payload = ipv4Packet.PayloadData ?? Array.Empty<byte>();
                    packetInfo.Info = $"IPv4 {ipv4Packet.Protocol}";
                }
            }
            // Handle IPv6
            else if (ipv6Packet != null)
            {
                packetInfo.SourceAddress = ipv6Packet.SourceAddress?.ToString() ?? "N/A";
                packetInfo.DestinationAddress = ipv6Packet.DestinationAddress?.ToString() ?? "N/A";
                packetInfo.IpVersion = Models.IPVersion.IPv6;
                
                // Extract protocol and port information
                var tcpPacket = packet.Extract<TcpPacket>();
                var udpPacket = packet.Extract<UdpPacket>();
                var icmpV6Packet = packet.Extract<IcmpV6Packet>();
                
                if (tcpPacket != null)
                {
                    packetInfo.Protocol = "TCP";
                    packetInfo.SourcePort = tcpPacket.SourcePort;
                    packetInfo.DestinationPort = tcpPacket.DestinationPort;
                    packetInfo.Payload = tcpPacket.PayloadData ?? Array.Empty<byte>();
                    packetInfo.Info = $"TCP {tcpPacket.SourcePort} → {tcpPacket.DestinationPort}";
                }
                else if (udpPacket != null)
                {
                    packetInfo.Protocol = "UDP";
                    packetInfo.SourcePort = udpPacket.SourcePort;
                    packetInfo.DestinationPort = udpPacket.DestinationPort;
                    packetInfo.Payload = udpPacket.PayloadData ?? Array.Empty<byte>();
                    packetInfo.Info = $"UDP {udpPacket.SourcePort} → {udpPacket.DestinationPort}";
                }
                else if (icmpV6Packet != null)
                {
                    packetInfo.Protocol = "ICMPv6";
                    packetInfo.Payload = icmpV6Packet.PayloadData ?? Array.Empty<byte>();
                    packetInfo.Info = $"ICMPv6 Type: {icmpV6Packet.Type}";
                }
                else
                {
                    packetInfo.Protocol = ipv6Packet.NextHeader.ToString();
                    packetInfo.Payload = ipv6Packet.PayloadData ?? Array.Empty<byte>();
                    packetInfo.Info = $"IPv6 {ipv6Packet.NextHeader}";
                }
            }
            
            return packetInfo;
        }
        catch (Exception ex)
        {
            // Return a packet with error info instead of null
            return new PacketInfo
            {
                FrameNumber = frameNumber,
                Timestamp = rawCapture?.Timeval.Date ?? DateTime.Now,
                PacketSize = rawCapture?.Data.Length ?? 0,
                SourceAddress = "N/A",
                DestinationAddress = "N/A",
                Protocol = "Error",
                IpVersion = Models.IPVersion.Unknown,
                Info = $"Parse error: {ex.Message}"
            };
        }
    }
}
