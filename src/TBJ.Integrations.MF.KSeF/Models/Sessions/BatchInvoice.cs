using System.Text.Json.Serialization;

namespace TBJ.Integrations.MF.KSeF.Models.Sessions;

/// <summary>
/// Faktura w paczce sesji wsadowej.
/// </summary>
public sealed class BatchInvoice
{
    /// <summary>
    /// Zaszyfrowana treść faktury (Base64).
    /// </summary>
    [JsonPropertyName("invoice")]
    public string Invoice { get; set; } = string.Empty;

    /// <summary>
    /// Metadane faktury.
    /// </summary>
    [JsonPropertyName("metadata")]
    public InvoiceMetadata Metadata { get; set; } = new();
}
