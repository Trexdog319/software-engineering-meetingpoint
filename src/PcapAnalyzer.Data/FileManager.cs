using System.Text.Json;

namespace PcapAnalyzer.Data;

/// <summary>
/// Manages file operations for the application
/// </summary>
public class FileManager
{
    private readonly string _baseDirectory;
    
    public FileManager(string baseDirectory)
    {
        _baseDirectory = baseDirectory;
        EnsureDirectoryExists();
    }
    
    /// <summary>
    /// Ensure the base directory exists
    /// </summary>
    private void EnsureDirectoryExists()
    {
        if (!Directory.Exists(_baseDirectory))
        {
            Directory.CreateDirectory(_baseDirectory);
        }
    }
    
    /// <summary>
    /// Save data to a file
    /// </summary>
    public void SaveToFile(string filename, string content)
    {
        var filePath = Path.Combine(_baseDirectory, filename);
        File.WriteAllText(filePath, content);
    }
    
    /// <summary>
    /// Save object as JSON
    /// </summary>
    public void SaveAsJson<T>(string filename, T data)
    {
        var filePath = Path.Combine(_baseDirectory, filename);
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(data, options);
        File.WriteAllText(filePath, json);
    }
    
    /// <summary>
    /// Load data from a file
    /// </summary>
    public string LoadFromFile(string filename)
    {
        var filePath = Path.Combine(_baseDirectory, filename);
        return File.Exists(filePath) ? File.ReadAllText(filePath) : string.Empty;
    }
    
    /// <summary>
    /// Load object from JSON file
    /// </summary>
    public T? LoadFromJson<T>(string filename)
    {
        var filePath = Path.Combine(_baseDirectory, filename);
        if (!File.Exists(filePath))
        {
            return default;
        }
        
        var json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<T>(json);
    }
    
    /// <summary>
    /// Get all files in the directory
    /// </summary>
    public IEnumerable<string> GetFiles(string searchPattern = "*.*")
    {
        return Directory.GetFiles(_baseDirectory, searchPattern);
    }
    
    /// <summary>
    /// Delete a file
    /// </summary>
    public bool DeleteFile(string filename)
    {
        var filePath = Path.Combine(_baseDirectory, filename);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            return true;
        }
        return false;
    }
}
