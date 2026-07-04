using System.Text.Json.Serialization;

namespace TBJ.Integrations.MF.KSeF.Models.Invoices;

/// <summary>
/// Status zleconego eksportu faktur.
/// </summary>
public sealed class ExportStatus
{
    /// <summary>
    /// Identyfikator eksportu.
    /// </summary>
    [JsonPropertyName("exportId")]
    public string ExportId { get; set; } = string.Empty;

    /// <summary>
    /// Status operacji eksportu.
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Opis statusu eksportu.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Procent postępu eksportu.
    /// </summary>
    [JsonPropertyName("progress")]
    public int? Progress { get; set; }
}
