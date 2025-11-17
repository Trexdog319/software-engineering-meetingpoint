using System.Text;

namespace PcapAnalyzer.Data;

/// <summary>
/// Handles writing log files for analysis results
/// </summary>
public class LogWriter
{
    private readonly string _logDirectory;
    
    public LogWriter(string logDirectory)
    {
        _logDirectory = logDirectory;
        EnsureLogDirectoryExists();
    }
    
    /// <summary>
    /// Ensure the log directory exists
    /// </summary>
    private void EnsureLogDirectoryExists()
    {
        if (!Directory.Exists(_logDirectory))
        {
            Directory.CreateDirectory(_logDirectory);
        }
    }
    
    /// <summary>
    /// Generate a timestamped filename
    /// </summary>
    private string GenerateTimestampedFilename(string prefix, string extension)
    {
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HHmmss");
        return $"{prefix}_{timestamp}.{extension}";
    }
    
    /// <summary>
    /// Write a log entry
    /// </summary>
    public string WriteLog(string content, string prefix = "analysis", string extension = "txt")
    {
        var filename = GenerateTimestampedFilename(prefix, extension);
        var filePath = Path.Combine(_logDirectory, filename);
        
        File.WriteAllText(filePath, content);
        return filePath;
    }
    
    /// <summary>
    /// Append to an existing log file
    /// </summary>
    public void AppendToLog(string filename, string content)
    {
        var filePath = Path.Combine(_logDirectory, filename);
        File.AppendAllText(filePath, content + Environment.NewLine);
    }
    
    /// <summary>
    /// Get all log files
    /// </summary>
    public IEnumerable<string> GetLogFiles(string searchPattern = "*.txt")
    {
        return Directory.GetFiles(_logDirectory, searchPattern)
            .OrderByDescending(f => File.GetCreationTime(f));
    }
    
    /// <summary>
    /// Delete old log files
    /// </summary>
    public int CleanOldLogs(int daysToKeep = 30)
    {
        var cutoffDate = DateTime.Now.AddDays(-daysToKeep);
        var deletedCount = 0;
        
        foreach (var file in Directory.GetFiles(_logDirectory))
        {
            if (File.GetCreationTime(file) < cutoffDate)
            {
                File.Delete(file);
                deletedCount++;
            }
        }
        
        return deletedCount;
    }
    
    /// <summary>
    /// Write application event log
    /// </summary>
    public void WriteEventLog(string eventType, string message)
    {
        var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{eventType}] {message}";
        var filename = $"application_{DateTime.Now:yyyy-MM}.log";
        AppendToLog(filename, logEntry);
    }
}
