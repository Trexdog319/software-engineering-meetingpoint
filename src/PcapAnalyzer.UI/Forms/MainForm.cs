using PcapAnalyzer.Core.Interfaces;
using PcapAnalyzer.Core.Models;
using PcapAnalyzer.Core.Services;
using PcapAnalyzer.Data;
using System.Windows.Forms;

namespace PcapAnalyzer.UI.Forms;

/// <summary>
/// Main application form for PCAP analysis
/// </summary>
public partial class MainForm : Form
{
    private readonly IPcapParser _pcapParser;
    private readonly IPacketAnalyzer _packetAnalyzer;
    private readonly IDataExporter _dataExporter;
    private readonly LogWriter _logWriter;
    private AnalysisResult? _currentAnalysis;
    
    private DataGridView dgvPackets = null!;
    private GroupBox gbDetails = null!;
    private GroupBox gbStatistics = null!;
    private MenuStrip menuStrip = null!;
    private StatusStrip statusStrip = null!;
    private ToolStripStatusLabel statusLabel = null!;
    private TextBox txtDetails = null!;
    private TextBox txtStatistics = null!;
    private SplitContainer splitContainer = null!;
    private ToolStrip toolStrip = null!;
    
    public MainForm()
    {
        _packetAnalyzer = new PacketAnalyzer();
        _pcapParser = new PcapParser(_packetAnalyzer);
        _dataExporter = new DataExporter();
        
        // Get the docs/Logs directory - use a simpler path
        var logsPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "..", "..", "..", "..", "..", "docs", "Logs");
        logsPath = Path.GetFullPath(logsPath);
        _logWriter = new LogWriter(logsPath);
        
        // Initialize form properties first
        this.Text = "PCAP Analyzer";
        this.Size = new Size(1200, 800);
        this.StartPosition = FormStartPosition.CenterScreen;
        
        // Then setup all UI controls
        SetupUI();
    }
    
    private void InitializeComponent()
    {
        // This method is kept for compatibility but not used
    }
    
    private void SetupUI()
    {
        // Menu Strip
        menuStrip = new MenuStrip();
        var fileMenu = new ToolStripMenuItem("&File");
        var openMenuItem = new ToolStripMenuItem("&Open PCAP...", null, OpenFile_Click);
        openMenuItem.ShortcutKeys = Keys.Control | Keys.O;
        var exportMenu = new ToolStripMenuItem("&Export");
        var exportJsonMenuItem = new ToolStripMenuItem("Export to &JSON...", null, ExportJson_Click);
        var exportCsvMenuItem = new ToolStripMenuItem("Export to &CSV...", null, ExportCsv_Click);
        var exportHtmlMenuItem = new ToolStripMenuItem("Export to &HTML...", null, ExportHtml_Click);
        exportMenu.DropDownItems.AddRange(new ToolStripItem[] { exportJsonMenuItem, exportCsvMenuItem, exportHtmlMenuItem });
        var exitMenuItem = new ToolStripMenuItem("E&xit", null, (s, e) => Application.Exit());
        
        fileMenu.DropDownItems.AddRange(new ToolStripItem[] { openMenuItem, exportMenu, new ToolStripSeparator(), exitMenuItem });
        menuStrip.Items.Add(fileMenu);
        
        // Tool Strip
        toolStrip = new ToolStrip();
        var openButton = new ToolStripButton("Open PCAP", null, OpenFile_Click);
        var refreshButton = new ToolStripButton("Refresh", null, (s, e) => RefreshDisplay());
        toolStrip.Items.AddRange(new ToolStripItem[] { openButton, new ToolStripSeparator(), refreshButton });
        
        // Split Container
        splitContainer = new SplitContainer
        {
            Dock = DockStyle.Fill,
            Orientation = Orientation.Horizontal,
            SplitterDistance = 400
        };
        
        // Packet DataGridView
        dgvPackets = new DataGridView
        {
            Dock = DockStyle.Fill,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            AutoGenerateColumns = false
        };
        dgvPackets.SelectionChanged += DgvPackets_SelectionChanged;
        dgvPackets.DataError += DgvPackets_DataError;
        
        // Bottom panel with details and statistics
        var bottomSplitContainer = new SplitContainer
        {
            Dock = DockStyle.Fill,
            Orientation = Orientation.Vertical
        };
        
        // Details GroupBox
        gbDetails = new GroupBox
        {
            Text = "Packet Details",
            Dock = DockStyle.Fill
        };
        txtDetails = new TextBox
        {
            Dock = DockStyle.Fill,
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Both,
            Font = new Font("Consolas", 9)
        };
        gbDetails.Controls.Add(txtDetails);
        
        // Statistics GroupBox
        gbStatistics = new GroupBox
        {
            Text = "Statistics",
            Dock = DockStyle.Fill
        };
        txtStatistics = new TextBox
        {
            Dock = DockStyle.Fill,
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Both,
            Font = new Font("Consolas", 9)
        };
        gbStatistics.Controls.Add(txtStatistics);
        
        bottomSplitContainer.Panel1.Controls.Add(gbDetails);
        bottomSplitContainer.Panel2.Controls.Add(gbStatistics);
        
        splitContainer.Panel1.Controls.Add(dgvPackets);
        splitContainer.Panel2.Controls.Add(bottomSplitContainer);
        
        // Status Strip
        statusStrip = new StatusStrip();
        statusLabel = new ToolStripStatusLabel("Ready");
        statusStrip.Items.Add(statusLabel);
        
        // Add controls to form
        this.Controls.Add(splitContainer);
        this.Controls.Add(toolStrip);
        this.Controls.Add(menuStrip);
        this.Controls.Add(statusStrip);
        this.MainMenuStrip = menuStrip;
    }
    
    private void OpenFile_Click(object? sender, EventArgs e)
    {
        using var openFileDialog = new OpenFileDialog
        {
            Filter = "PCAP Files (*.pcap;*.pcapng)|*.pcap;*.pcapng|All Files (*.*)|*.*",
            Title = "Select PCAP File"
        };
        
        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            LoadPcapFile(openFileDialog.FileName);
        }
    }
    
    private void LoadPcapFile(string filePath)
    {
        try
        {
            if (statusLabel == null || dgvPackets == null || txtStatistics == null)
            {
                MessageBox.Show("UI controls not initialized properly. Please restart the application.", 
                    "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            statusLabel.Text = "Loading and analyzing PCAP file...";
            this.Cursor = Cursors.WaitCursor;
            Application.DoEvents();
            
            _currentAnalysis = _pcapParser.AnalyzeFile(filePath);
            
            if (_currentAnalysis != null && _currentAnalysis.Packets != null)
            {
                DisplayPackets();
                DisplayStatistics();
                
                statusLabel.Text = $"Loaded {_currentAnalysis.TotalPackets} packets from {Path.GetFileName(filePath)}";
                
                // Log the analysis - wrap in try-catch to prevent logging errors from crashing
                try
                {
                    _logWriter.WriteEventLog("INFO", $"Analyzed file: {filePath} ({_currentAnalysis.TotalPackets} packets)");
                }
                catch
                {
                    // Ignore logging errors
                }
            }
            else
            {
                MessageBox.Show("No packets found in the file.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                statusLabel.Text = "No packets found";
            }
        }
        catch (Exception ex)
        {
            var errorMsg = $"Error loading PCAP file:\n\n{ex.Message}\n\nStack trace:\n{ex.StackTrace}";
            MessageBox.Show(errorMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            if (statusLabel != null)
            {
                statusLabel.Text = "Error loading file";
            }
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    }
    
    private void DisplayPackets()
    {
        if (_currentAnalysis == null || dgvPackets == null) return;
        
        try
        {
            // Suspend layout to prevent errors during update
            dgvPackets.SuspendLayout();
            
            // Clear existing data
            dgvPackets.DataSource = null;
            dgvPackets.Columns.Clear();
            
            // Add columns manually
            dgvPackets.Columns.Add(new DataGridViewTextBoxColumn { Name = "FrameNumber", HeaderText = "No.", DataPropertyName = "FrameNumber", Width = 60 });
            dgvPackets.Columns.Add(new DataGridViewTextBoxColumn { Name = "Timestamp", HeaderText = "Time", DataPropertyName = "Timestamp", Width = 150 });
            dgvPackets.Columns.Add(new DataGridViewTextBoxColumn { Name = "SourceAddress", HeaderText = "Source", DataPropertyName = "SourceAddress", Width = 150 });
            dgvPackets.Columns.Add(new DataGridViewTextBoxColumn { Name = "DestinationAddress", HeaderText = "Destination", DataPropertyName = "DestinationAddress", Width = 150 });
            dgvPackets.Columns.Add(new DataGridViewTextBoxColumn { Name = "Protocol", HeaderText = "Protocol", DataPropertyName = "Protocol", Width = 80 });
            dgvPackets.Columns.Add(new DataGridViewTextBoxColumn { Name = "PacketSize", HeaderText = "Length", DataPropertyName = "PacketSize", Width = 80 });
            dgvPackets.Columns.Add(new DataGridViewTextBoxColumn { Name = "Info", HeaderText = "Info", DataPropertyName = "Info", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            
            // Use BindingList for better data binding
            var bindingList = new System.ComponentModel.BindingList<PacketInfo>(_currentAnalysis.Packets);
            dgvPackets.DataSource = bindingList;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error displaying packets: {ex.Message}", "Display Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        finally
        {
            dgvPackets.ResumeLayout();
        }
    }
    
    private void DisplayStatistics()
    {
        if (_currentAnalysis?.Statistics == null) return;
        
        var stats = _currentAnalysis.Statistics;
        var sb = new System.Text.StringBuilder();
        
        sb.AppendLine("=== Overall Statistics ===");
        sb.AppendLine($"Total Packets: {stats.TotalPackets:N0}");
        sb.AppendLine($"Total Bytes: {stats.TotalBytes:N0}");
        sb.AppendLine($"Average Size: {stats.AveragePacketSize:F2} bytes");
        sb.AppendLine($"Min Size: {stats.MinPacketSize} bytes");
        sb.AppendLine($"Max Size: {stats.MaxPacketSize} bytes");
        
        if (stats.CaptureDuration.HasValue)
        {
            sb.AppendLine($"Duration: {stats.CaptureDuration.Value.TotalSeconds:F2} seconds");
        }
        
        sb.AppendLine();
        sb.AppendLine("=== Protocol Distribution ===");
        foreach (var proto in stats.ProtocolCounts.OrderByDescending(p => p.Value))
        {
            var pct = (proto.Value * 100.0) / stats.TotalPackets;
            sb.AppendLine($"{proto.Key}: {proto.Value:N0} ({pct:F2}%)");
        }
        
        sb.AppendLine();
        sb.AppendLine("=== IP Version Distribution ===");
        foreach (var ver in stats.IpVersionCounts)
        {
            var pct = (ver.Value * 100.0) / stats.TotalPackets;
            sb.AppendLine($"{ver.Key}: {ver.Value:N0} ({pct:F2}%)");
        }
        
        txtStatistics.Text = sb.ToString();
    }
    
    private void DgvPackets_SelectionChanged(object? sender, EventArgs e)
    {
        if (dgvPackets.SelectedRows.Count == 0) return;
        
        var packet = dgvPackets.SelectedRows[0].DataBoundItem as PacketInfo;
        if (packet == null) return;
        
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"Frame Number: {packet.FrameNumber}");
        sb.AppendLine($"Timestamp: {packet.Timestamp:yyyy-MM-dd HH:mm:ss.ffffff}");
        sb.AppendLine($"Packet Size: {packet.PacketSize} bytes");
        sb.AppendLine();
        sb.AppendLine($"Source Address: {packet.SourceAddress}");
        sb.AppendLine($"Destination Address: {packet.DestinationAddress}");
        sb.AppendLine($"Protocol: {packet.Protocol}");
        sb.AppendLine($"IP Version: {packet.IpVersion}");
        
        if (packet.SourcePort.HasValue)
            sb.AppendLine($"Source Port: {packet.SourcePort}");
        if (packet.DestinationPort.HasValue)
            sb.AppendLine($"Destination Port: {packet.DestinationPort}");
        
        sb.AppendLine();
        sb.AppendLine($"Payload Size: {packet.Payload.Length} bytes");
        
        if (packet.Payload.Length > 0)
        {
            sb.AppendLine();
            sb.AppendLine("Payload (Hex):");
            sb.AppendLine(packet.PayloadHex);
            sb.AppendLine();
            sb.AppendLine("Payload (ASCII):");
            sb.AppendLine(packet.PayloadAscii);
        }
        
        txtDetails.Text = sb.ToString();
    }
    
    private void DgvPackets_DataError(object? sender, DataGridViewDataErrorEventArgs e)
    {
        // Suppress the default error dialog
        e.ThrowException = false;
        e.Cancel = true;
    }
    
    private void RefreshDisplay()
    {
        if (_currentAnalysis != null)
        {
            DisplayPackets();
            DisplayStatistics();
        }
    }
    
    private void ExportJson_Click(object? sender, EventArgs e)
    {
        if (_currentAnalysis == null)
        {
            MessageBox.Show("No analysis data to export.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        
        using var saveFileDialog = new SaveFileDialog
        {
            Filter = "JSON Files (*.json)|*.json",
            Title = "Export to JSON",
            FileName = $"analysis_{DateTime.Now:yyyy-MM-dd_HHmmss}.json"
        };
        
        if (saveFileDialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                _dataExporter.ExportToJson(_currentAnalysis, saveFileDialog.FileName);
                MessageBox.Show("Export completed successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Export failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
    
    private void ExportCsv_Click(object? sender, EventArgs e)
    {
        if (_currentAnalysis == null)
        {
            MessageBox.Show("No analysis data to export.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        
        using var saveFileDialog = new SaveFileDialog
        {
            Filter = "CSV Files (*.csv)|*.csv",
            Title = "Export to CSV",
            FileName = $"analysis_{DateTime.Now:yyyy-MM-dd_HHmmss}.csv"
        };
        
        if (saveFileDialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                _dataExporter.ExportToCsv(_currentAnalysis, saveFileDialog.FileName);
                MessageBox.Show("Export completed successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Export failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
    
    private void ExportHtml_Click(object? sender, EventArgs e)
    {
        if (_currentAnalysis == null)
        {
            MessageBox.Show("No analysis data to export.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        
        using var saveFileDialog = new SaveFileDialog
        {
            Filter = "HTML Files (*.html)|*.html",
            Title = "Export to HTML",
            FileName = $"analysis_{DateTime.Now:yyyy-MM-dd_HHmmss}.html"
        };
        
        if (saveFileDialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                _dataExporter.ExportToHtml(_currentAnalysis, saveFileDialog.FileName);
                MessageBox.Show("Export completed successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Export failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
