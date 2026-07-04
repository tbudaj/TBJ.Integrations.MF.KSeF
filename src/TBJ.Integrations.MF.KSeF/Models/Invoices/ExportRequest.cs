using System.Text.Json.Serialization;

namespace TBJ.Integrations.MF.KSeF.Models.Invoices;

/// <summary>
/// Żądanie inicjacji eksportu faktur.
/// </summary>
public sealed class ExportRequest
{
    /// <summary>
    /// Kryteria eksportu.
    /// </summary>
    [JsonPropertyName("criteria")]
    public ExportCriteria Criteria { get; set; } = new();

    /// <summary>
    /// Format eksportu.
    /// </summary>
    [JsonPropertyName("format")]
    public ExportFormat Format { get; set; }
}
