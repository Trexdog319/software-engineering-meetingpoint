# PCAP Analyzer

A Windows desktop application for analyzing network packet capture (PCAP) files with detailed packet information, statistics, and export capabilities.

## Features

- **Parse PCAP/PCAPNG Files**: Read and analyze standard packet capture formats
- **Detailed Packet View**: Display comprehensive information including:
  - Source and destination IP addresses
  - Protocol types (TCP, UDP, ICMP, etc.)
  - IPv4/IPv6 support
  - Port numbers
  - Packet sizes
  - Payload data (hex and ASCII)
- **Statistics Dashboard**: View aggregated analysis including:
  - Protocol distribution
  - IP version breakdown
  - Packet size statistics
  - Top communicating addresses
- **Multiple Export Formats**: Export analysis results to JSON, CSV, and HTML
- **Automated Logging**: All analysis operations logged to `docs/Logs/`

## Project Structure

```
software-engineering-meetingpoint/
├── src/
│   ├── PcapAnalyzer.Core/        # Business logic and data models
│   │   ├── Models/               # PacketInfo, Statistics, etc.
│   │   ├── Services/             # PcapParser, PacketAnalyzer, DataExporter
│   │   └── Interfaces/           # Service contracts
│   ├── PcapAnalyzer.Data/        # File I/O and logging
│   ├── PcapAnalyzer.UI/          # Windows Forms interface
│   │   └── Forms/                # MainForm and UI components
│   └── PcapAnalyzer.sln          # Solution file
├── tests/
│   └── PcapAnalyzer.Core.Tests/  # Unit tests
├── docs/
│   ├── DesignDocument.md         # Architecture details
│   ├── UserGuide.md              # User documentation
│   └── Logs/                     # Analysis output directory
└── README.md
```

## Technology Stack

- **.NET 8.0**: Modern .NET framework
- **Windows Forms**: Desktop UI framework
- **SharpPcap 6.3.1**: PCAP file parsing library
- **PacketDotNet 1.4.8**: Network packet dissection
- **xUnit**: Unit testing framework

## Getting Started

### Prerequisites

- Windows 10 or later
- .NET 8.0 SDK or Runtime
- Visual Studio 2022 or VS Code (for development)

### Building the Project

1. Clone the repository:
   ```bash
   git clone https://github.com/Trexdog319/software-engineering-meetingpoint.git
   cd software-engineering-meetingpoint
   ```

2. Build the solution:
   ```bash
   cd src
   dotnet build PcapAnalyzer.sln
   ```

3. Run the application:
   ```bash
   dotnet run --project PcapAnalyzer.UI
   ```

### Running Tests

```bash
cd tests/PcapAnalyzer.Core.Tests
dotnet test
```

## Usage

1. **Launch the Application**
   - Run `PcapAnalyzer.UI.exe` from the build output directory

2. **Open a PCAP File**
   - Click `File → Open PCAP...` or press `Ctrl+O`
   - Select your PCAP or PCAPNG file
   - The application will parse and display packet information

3. **View Packet Details**
   - Click on any packet in the list to view detailed information
   - See payload data in both hexadecimal and ASCII formats

4. **Export Analysis Results**
   - Use `File → Export` to save results in:
     - **JSON**: Complete data structure
     - **CSV**: Spreadsheet-compatible format
     - **HTML**: Formatted report for viewing in browsers

5. **Review Statistics**
   - Check the statistics panel for:
     - Protocol distribution
     - IP version usage
     - Packet size metrics
     - Top communicating addresses

## Development Guidelines

### Architecture

The application follows a **three-tier architecture**:

1. **Presentation Layer** (`PcapAnalyzer.UI`): User interface
2. **Business Logic Layer** (`PcapAnalyzer.Core`): Core functionality
3. **Data Access Layer** (`PcapAnalyzer.Data`): File operations and logging

### Design Patterns

- **Dependency Injection**: Services are injected via constructors
- **Interface Segregation**: Clear separation of concerns
- **Repository Pattern**: Abstracted data access

### Adding New Features

1. Add models to `PcapAnalyzer.Core/Models/`
2. Define interfaces in `PcapAnalyzer.Core/Interfaces/`
3. Implement services in `PcapAnalyzer.Core/Services/`
4. Add UI components in `PcapAnalyzer.UI/Forms/` or `Controls/`
5. Write tests in `tests/PcapAnalyzer.Core.Tests/`

## Documentation

- **[Design Document](docs/DesignDocument.md)**: Detailed architecture and technical design
- **[User Guide](docs/UserGuide.md)**: Complete user documentation
- **[Project Overview](docs/ProjectOverview.txt)**: Quick reference guide

## Log Files

Analysis logs are automatically saved to `docs/Logs/` with timestamped filenames:
- `analysis_YYYY-MM-DD_HHMMSS.json`
- `analysis_YYYY-MM-DD_HHMMSS.csv`
- `analysis_YYYY-MM-DD_HHMMSS.html`
- `application_YYYY-MM.log`

## Future Enhancements

- [ ] Real-time packet capture
- [ ] Advanced filtering and search
- [ ] Statistical charts and graphs
- [ ] Deep packet inspection
- [ ] Network flow visualization
- [ ] Protocol-specific analysis
- [ ] Comparative analysis of multiple captures

## Contributing

This is a group software engineering class assignment project. For collaboration:

1. Create a feature branch
2. Make your changes
3. Write/update tests
4. Submit a pull request

## License

See [LICENSE](LICENSE) file for details.

## Authors

Software Engineering Class - Group Project

---

**Last Updated**: November 16, 2025  
**Version**: 1.0.0
