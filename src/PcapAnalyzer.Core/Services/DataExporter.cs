using System.Text;
using System.Text.Json;
using PcapAnalyzer.Core.Interfaces;
using PcapAnalyzer.Core.Models;

namespace PcapAnalyzer.Core.Services;

/// <summary>
/// Implementation of data export functionality
/// </summary>
public class DataExporter : IDataExporter
{
    /// <inheritdoc/>
    public void ExportToJson(AnalysisResult result, string outputPath)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        
        var json = JsonSerializer.Serialize(result, options);
        File.WriteAllText(outputPath, json);
    }
    
    /// <inheritdoc/>
    public void ExportToCsv(AnalysisResult result, string outputPath)
    {
        var csv = new StringBuilder();
        
        // Header
        csv.AppendLine("FrameNumber,Timestamp,PacketSize,SourceAddress,DestinationAddress,Protocol,IpVersion,SourcePort,DestinationPort,PayloadSize");
        
        // Data rows
        foreach (var packet in result.Packets)
        {
            csv.AppendLine($"{packet.FrameNumber}," +
                          $"{packet.Timestamp:yyyy-MM-dd HH:mm:ss.fff}," +
                          $"{packet.PacketSize}," +
                          $"{packet.SourceAddress}," +
                          $"{packet.DestinationAddress}," +
                          $"{packet.Protocol}," +
                          $"{packet.IpVersion}," +
                          $"{packet.SourcePort}," +
                          $"{packet.DestinationPort}," +
                          $"{packet.Payload.Length}");
        }
        
        File.WriteAllText(outputPath, csv.ToString());
    }
    
    /// <inheritdoc/>
    public void ExportToHtml(AnalysisResult result, string outputPath)
    {
        var html = new StringBuilder();
        
        html.AppendLine("<!DOCTYPE html>");
        html.AppendLine("<html>");
        html.AppendLine("<head>");
        html.AppendLine("    <title>PCAP Analysis Report</title>");
        html.AppendLine("    <style>");
        html.AppendLine("        body { font-family: Arial, sans-serif; margin: 20px; }");
        html.AppendLine("        h1 { color: #333; }");
        html.AppendLine("        table { border-collapse: collapse; width: 100%; margin-top: 20px; }");
        html.AppendLine("        th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }");
        html.AppendLine("        th { background-color: #4CAF50; color: white; }");
        html.AppendLine("        tr:nth-child(even) { background-color: #f2f2f2; }");
        html.AppendLine("        .stats { background-color: #f9f9f9; padding: 15px; margin: 20px 0; border-radius: 5px; }");
        html.AppendLine("    </style>");
        html.AppendLine("</head>");
        html.AppendLine("<body>");
        html.AppendLine($"    <h1>PCAP Analysis Report</h1>");
        html.AppendLine($"    <p><strong>File:</strong> {result.FilePath}</p>");
        html.AppendLine($"    <p><strong>Analysis Date:</strong> {result.AnalysisTimestamp:yyyy-MM-dd HH:mm:ss}</p>");
        
        // Statistics
        html.AppendLine("    <div class='stats'>");
        html.AppendLine("        <h2>Statistics</h2>");
        html.AppendLine($"        <p><strong>Total Packets:</strong> {result.Statistics.TotalPackets}</p>");
        html.AppendLine($"        <p><strong>Total Bytes:</strong> {result.Statistics.TotalBytes:N0}</p>");
        html.AppendLine($"        <p><strong>Average Packet Size:</strong> {result.Statistics.AveragePacketSize:F2} bytes</p>");
        html.AppendLine($"        <p><strong>Min/Max Packet Size:</strong> {result.Statistics.MinPacketSize} / {result.Statistics.MaxPacketSize} bytes</p>");
        
        if (result.Statistics.CaptureDuration.HasValue)
        {
            html.AppendLine($"        <p><strong>Capture Duration:</strong> {result.Statistics.CaptureDuration.Value.TotalSeconds:F2} seconds</p>");
        }
        
        html.AppendLine("    </div>");
        
        // Packet table
        html.AppendLine("    <h2>Packets</h2>");
        html.AppendLine("    <table>");
        html.AppendLine("        <tr>");
        html.AppendLine("            <th>Frame</th>");
        html.AppendLine("            <th>Time</th>");
        html.AppendLine("            <th>Source</th>");
        html.AppendLine("            <th>Destination</th>");
        html.AppendLine("            <th>Protocol</th>");
        html.AppendLine("            <th>Length</th>");
        html.AppendLine("            <th>Info</th>");
        html.AppendLine("        </tr>");
        
        foreach (var packet in result.Packets.Take(1000)) // Limit to first 1000 for performance
        {
            html.AppendLine("        <tr>");
            html.AppendLine($"            <td>{packet.FrameNumber}</td>");
            html.AppendLine($"            <td>{packet.Timestamp:HH:mm:ss.fff}</td>");
            html.AppendLine($"            <td>{packet.SourceAddress}</td>");
            html.AppendLine($"            <td>{packet.DestinationAddress}</td>");
            html.AppendLine($"            <td>{packet.Protocol}</td>");
            html.AppendLine($"            <td>{packet.PacketSize}</td>");
            html.AppendLine($"            <td>{packet.Info}</td>");
            html.AppendLine("        </tr>");
        }
        
        html.AppendLine("    </table>");
        html.AppendLine("</body>");
        html.AppendLine("</html>");
        
        File.WriteAllText(outputPath, html.ToString());
    }
    
    /// <inheritdoc/>
    public void ExportStatisticsToText(Statistics statistics, string outputPath)
    {
        var text = new StringBuilder();
        
        text.AppendLine("=== PCAP Analysis Statistics ===");
        text.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        text.AppendLine();
        
        text.AppendLine("Overall Statistics:");
        text.AppendLine($"  Total Packets: {statistics.TotalPackets}");
        text.AppendLine($"  Total Bytes: {statistics.TotalBytes:N0}");
        text.AppendLine($"  Average Packet Size: {statistics.AveragePacketSize:F2} bytes");
        text.AppendLine($"  Min Packet Size: {statistics.MinPacketSize} bytes");
        text.AppendLine($"  Max Packet Size: {statistics.MaxPacketSize} bytes");
        
        if (statistics.CaptureDuration.HasValue)
        {
            text.AppendLine($"  Capture Duration: {statistics.CaptureDuration.Value.TotalSeconds:F2} seconds");
        }
        
        text.AppendLine();
        text.AppendLine("Protocol Distribution:");
        foreach (var protocol in statistics.ProtocolCounts.OrderByDescending(p => p.Value))
        {
            var percentage = (protocol.Value * 100.0) / statistics.TotalPackets;
            text.AppendLine($"  {protocol.Key}: {protocol.Value} ({percentage:F2}%)");
        }
        
        text.AppendLine();
        text.AppendLine("IP Version Distribution:");
        foreach (var version in statistics.IpVersionCounts)
        {
            var percentage = (version.Value * 100.0) / statistics.TotalPackets;
            text.AppendLine($"  {version.Key}: {version.Value} ({percentage:F2}%)");
        }
        
        text.AppendLine();
        text.AppendLine("Top Source Addresses:");
        foreach (var addr in statistics.TopSourceAddresses)
        {
            text.AppendLine($"  {addr.Key}: {addr.Value} packets");
        }
        
        text.AppendLine();
        text.AppendLine("Top Destination Addresses:");
        foreach (var addr in statistics.TopDestinationAddresses)
        {
            text.AppendLine($"  {addr.Key}: {addr.Value} packets");
        }
        
        File.WriteAllText(outputPath, text.ToString());
    }
}
