# Sample PCAP Files

Place your PCAP test files in this directory for easy access.

## Recommended Sample Files

You can download free sample PCAP files from:

1. **Wireshark Sample Captures**
   - URL: https://wiki.wireshark.org/SampleCaptures
   - Good starter files:
     - `http.cap` - HTTP traffic
     - `dns.cap` - DNS queries
     - `tcp-ecn-sample.pcap` - TCP traffic

2. **NETRESEC**
   - URL: https://www.netresec.com/index.ashx?page=PcapFiles
   - Real-world traffic samples

3. **PacketLife**
   - URL: https://packetlife.net/captures/
   - Various protocol captures

## File Organization

You can organize files by type:

```
sample-pcaps/
├── http/
│   ├── web-traffic.pcap
│   └── api-calls.pcap
├── dns/
│   └── dns-queries.pcap
├── tcp/
│   └── tcp-handshake.pcap
└── mixed/
    └── general-traffic.pcapng
```

## Using These Files

1. **In the Application**:
   - File → Open PCAP
   - Navigate to this folder
   - Select your file

2. **In Tests**:
   - Copy files to `tests/PcapAnalyzer.Core.Tests/TestData/`
   - Reference in integration tests

## Creating Your Own PCAP Files

Use Wireshark to capture network traffic:

1. Download Wireshark: https://www.wireshark.org/
2. Start a capture (Capture → Start)
3. Perform some network activity (browse web, etc.)
4. Stop capture (Capture → Stop)
5. Save as: File → Save As → Choose .pcap or .pcapng

## Notes

- **File Size**: Start with files under 10MB for quick testing
- **Privacy**: Don't commit files with sensitive data to the repository
- **Git Ignore**: Large PCAP files are ignored by git (.gitignore)

---

This directory is for local testing only. Sample files are not tracked in version control.
