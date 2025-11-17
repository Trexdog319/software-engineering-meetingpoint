using PcapAnalyzer.Core.Models;

namespace PcapAnalyzer.Core.Interfaces;

/// <summary>
/// Interface for exporting analysis data to various formats
/// </summary>
public interface IDataExporter
{
    /// <summary>
    /// Export analysis result to JSON format
    /// </summary>
    /// <param name="result">Analysis result to export</param>
    /// <param name="outputPath">Output file path</param>
    void ExportToJson(AnalysisResult result, string outputPath);
    
    /// <summary>
    /// Export analysis result to CSV format
    /// </summary>
    /// <param name="result">Analysis result to export</param>
    /// <param name="outputPath">Output file path</param>
    void ExportToCsv(AnalysisResult result, string outputPath);
    
    /// <summary>
    /// Export analysis result to HTML report
    /// </summary>
    /// <param name="result">Analysis result to export</param>
    /// <param name="outputPath">Output file path</param>
    void ExportToHtml(AnalysisResult result, string outputPath);
    
    /// <summary>
    /// Export statistics summary to text file
    /// </summary>
    /// <param name="statistics">Statistics to export</param>
    /// <param name="outputPath">Output file path</param>
    void ExportStatisticsToText(Statistics statistics, string outputPath);
}
