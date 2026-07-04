using System.Text.Json.Serialization;

namespace TBJ.Integrations.MF.KSeF.Models.Sessions;

/// <summary>
/// Faktura w ramach sesji wystawiania.
/// </summary>
public sealed class InvoiceInSession
{
    /// <summary>
    /// Numer KSeF faktury.
    /// </summary>
    [JsonPropertyName("ksefNumber")]
    public string KsefNumber { get; set; } = string.Empty;

    /// <summary>
    /// Numer referencyjny operacji przetwarzania faktury.
    /// </summary>
    [JsonPropertyName("referenceNumber")]
    public string ReferenceNumber { get; set; } = string.Empty;

    /// <summary>
    /// Status faktury w sesji.
    /// </summary>
    [JsonPropertyName("status")]
    public InvoiceStatusInfo? Status { get; set; }
}
