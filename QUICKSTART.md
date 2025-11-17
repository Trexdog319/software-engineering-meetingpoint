# Quick Start Guide - PCAP Analyzer

## How to Run the Application

### Method 1: Using Visual Studio (Recommended)
1. Open `src/PcapAnalyzer.sln` in Visual Studio 2022
2. Set `PcapAnalyzer.UI` as the startup project (right-click → Set as Startup Project)
3. Press `F5` or click the "Start" button
4. The application window will open

### Method 2: Using Command Line
```powershell
cd src
dotnet run --project PcapAnalyzer.UI
```

### Method 3: Run the Compiled Executable
After building the project:
```powershell
cd src\PcapAnalyzer.UI\bin\Debug\net8.0-windows
.\PcapAnalyzer.UI.exe
```

Or simply double-click `PcapAnalyzer.UI.exe` in Windows Explorer at:
```
src\PcapAnalyzer.UI\bin\Debug\net8.0-windows\PcapAnalyzer.UI.exe
```

## Where to Put Your PCAP Files

### Option 1: Anywhere on Your Computer (Recommended)
The application has a file picker dialog, so you can keep your PCAP files anywhere:

1. **Desktop**: `C:\Users\YourName\Desktop\`
2. **Documents**: `C:\Users\YourName\Documents\`
3. **Downloads**: `C:\Users\YourName\Downloads\`
4. **Project folder**: Create a folder like:
   ```
   C:\Users\NateB\Desktop\vscode\softslop\software-engineering-meetingpoint\sample-pcaps\
   ```

When you run the application:
- Click **File → Open PCAP...** (or press `Ctrl+O`)
- Browse to your PCAP file location
- Select the file and click Open

### Option 2: For Testing with Code
If you want to use PCAP files in your unit tests:

```
tests\PcapAnalyzer.Core.Tests\TestData\
```

Example:
```
tests\PcapAnalyzer.Core.Tests\TestData\sample.pcap
tests\PcapAnalyzer.Core.Tests\TestData\test-http.pcap
tests\PcapAnalyzer.Core.Tests\TestData\test-dns.pcapng
```

## Using the Application

1. **Launch the application** (using one of the methods above)

2. **Open a PCAP file**:
   - Click `File → Open PCAP...` or press `Ctrl+O`
   - Navigate to your PCAP file
   - Click Open

3. **View the results**:
   - **Packet List** (top): Shows all packets with key information
   - **Packet Details** (bottom-left): Click any packet to see details
   - **Statistics** (bottom-right): View aggregated analysis

4. **Export the analysis**:
   - `File → Export → Export to JSON...`
   - `File → Export → Export to CSV...`
   - `File → Export → Export to HTML...`

## Where to Find Sample PCAP Files

### Online Resources:
1. **Wireshark Sample Captures**: https://wiki.wireshark.org/SampleCaptures
2. **PCAP Repository**: https://www.netresec.com/index.ashx?page=PcapFiles
3. **Create Your Own**: Use Wireshark to capture network traffic

### Example Downloads:
```powershell
# Create a sample-pcaps folder
mkdir sample-pcaps

# Download a sample (example URLs - verify current links)
# Then move downloaded files to sample-pcaps folder
```

## Example File Structure

```
software-engineering-meetingpoint/
├── src/
│   └── PcapAnalyzer.sln
├── tests/
│   └── PcapAnalyzer.Core.Tests/
│       └── TestData/
│           ├── sample.pcap          ← Put test PCAP files here
│           └── http-traffic.pcap
├── sample-pcaps/                    ← Optional: Create this folder
│   ├── my-capture.pcap
│   ├── dns-queries.pcapng
│   └── web-traffic.pcap
└── docs/
    └── Logs/                        ← Analysis results saved here
        ├── analysis_2025-11-16_143022.json
        └── analysis_2025-11-16_143022.csv
```

## Troubleshooting

### "Application won't start"
- Ensure .NET 8.0 Runtime is installed
- Try building first: `dotnet build src/PcapAnalyzer.sln`
- Check for errors in the build output

### "Can't open PCAP file"
- Verify the file has `.pcap` or `.pcapng` extension
- Ensure the file isn't corrupted
- Try with a different PCAP file from Wireshark samples

### "No window appears"
- Make sure you're running `PcapAnalyzer.UI` project (not Core or Data)
- Check Windows Firewall isn't blocking the app
- Try running from Visual Studio instead of command line

## Quick Test

1. **Download a sample PCAP**:
   - Go to: https://wiki.wireshark.org/SampleCaptures
   - Download `http.cap` or any small sample
   - Save it to your Desktop

2. **Run the application**:
   ```powershell
   cd src
   dotnet run --project PcapAnalyzer.UI
   ```

3. **Open the file**:
   - In the app: File → Open PCAP
   - Navigate to Desktop
   - Select the downloaded file

4. **View the results**!

## Tips

- **Small files first**: Start with PCAP files under 10MB
- **Check statistics**: The statistics panel shows protocol distribution
- **Export for analysis**: Export to CSV to analyze in Excel
- **Save logs**: All analysis is logged to `docs/Logs/`

---

Need help? Check the full documentation in `docs/UserGuide.md`
