using System.Text.Json.Serialization;

namespace TBJ.Integrations.MF.KSeF.Models.Invoices;

/// <summary>
/// Odpowiedź z identyfikatorem zleconego eksportu faktur.
/// </summary>
public sealed class ExportResponse
{
    /// <summary>
    /// Identyfikator eksportu.
    /// </summary>
    [JsonPropertyName("exportId")]
    public string ExportId { get; set; } = string.Empty;

    /// <summary>
    /// Numer referencyjny operacji eksportu.
    /// </summary>
    [JsonPropertyName("referenceNumber")]
    public string ReferenceNumber { get; set; } = string.Empty;
}
