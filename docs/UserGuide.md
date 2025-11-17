# PCAP Analyzer - User Guide

## Introduction
PCAP Analyzer is a powerful tool for analyzing network packet capture files. This guide will help you get started with analyzing your PCAP files.

## Getting Started

### System Requirements
- Windows 10 or later
- .NET 8.0 Runtime
- Minimum 4GB RAM recommended
- PCAP or PCAPNG files to analyze

### Installation
1. Download the latest release
2. Extract to desired location
3. Run `PcapAnalyzer.UI.exe`

## Using the Application

### Opening a PCAP File
1. Click **File → Open PCAP...** or press `Ctrl+O`
2. Browse to your PCAP file
3. Select the file and click **Open**
4. The application will analyze the file and display results

### Main Window Overview

#### Packet List (Top Panel)
Displays all packets in the capture with the following columns:
- **No.**: Sequential frame number
- **Time**: Timestamp of packet capture
- **Source**: Source IP address
- **Destination**: Destination IP address
- **Protocol**: Network protocol (TCP, UDP, ICMP, etc.)
- **Length**: Packet size in bytes
- **Info**: Additional packet information

#### Packet Details (Bottom Left)
Shows detailed information about the selected packet:
- Frame number and timestamp
- IP addresses and ports
- Protocol information
- Payload data in hex and ASCII format

#### Statistics (Bottom Right)
Displays aggregated statistics:
- Total packet count and bytes
- Average, minimum, and maximum packet sizes
- Protocol distribution
- IP version distribution
- Capture duration

### Exporting Data

#### Export to JSON
1. Click **File → Export → Export to JSON...**
2. Choose destination and filename
3. Click **Save**

JSON export includes complete packet data and statistics.

#### Export to CSV
1. Click **File → Export → Export to CSV...**
2. Choose destination and filename
3. Click **Save**

CSV format is ideal for importing into spreadsheet applications.

#### Export to HTML
1. Click **File → Export → Export to HTML...**
2. Choose destination and filename
3. Click **Save**

HTML export creates a formatted report viewable in web browsers.

## Understanding the Data

### IP Versions
- **IPv4**: Traditional 32-bit IP addresses (e.g., 192.168.1.1)
- **IPv6**: Modern 128-bit IP addresses (e.g., 2001:db8::1)

### Common Protocols
- **TCP**: Transmission Control Protocol - reliable, connection-oriented
- **UDP**: User Datagram Protocol - fast, connectionless
- **ICMP**: Internet Control Message Protocol - network diagnostics
- **HTTP/HTTPS**: Web traffic
- **DNS**: Domain Name System queries

### Packet Information
- **Source/Destination Address**: IP addresses of sender and receiver
- **Ports**: Application-level identifiers (e.g., 80 for HTTP, 443 for HTTPS)
- **Payload**: Actual data being transmitted

## Log Files
Analysis logs are automatically saved to:
```
docs/Logs/
```

Log files include:
- Analysis timestamp
- File information
- Any errors or warnings

## Tips and Best Practices

### Performance
- Large PCAP files (>100MB) may take time to load
- Close other applications if analyzing very large files
- Export subsets of data for easier analysis

### Analysis Tips
1. Check protocol distribution to understand traffic types
2. Look for unusual packet sizes
3. Examine source/destination patterns for anomalies
4. Use exports for further analysis in specialized tools

### Troubleshooting

#### File Won't Open
- Ensure file is valid PCAP/PCAPNG format
- Check file isn't corrupted
- Verify you have read permissions

#### Application Crashes
- Check available memory
- Try smaller PCAP files first
- Review error logs in `docs/Logs/`

#### Missing Data
- Some protocols may not be fully parsed
- Encrypted payloads will show as binary data
- Fragmented packets may appear incomplete

## Keyboard Shortcuts
- `Ctrl+O`: Open PCAP file
- `Arrow Keys`: Navigate packet list
- `F5`: Refresh display

## Support and Feedback
For issues, questions, or feature requests, please contact your system administrator or consult the technical documentation.

## Glossary
- **PCAP**: Packet Capture - file format for network data
- **Frame**: Individual network packet
- **Protocol**: Rules for network communication
- **Payload**: Data portion of a packet
- **Capture**: Recording of network traffic

---
Last Updated: November 16, 2025
Version: 1.0
