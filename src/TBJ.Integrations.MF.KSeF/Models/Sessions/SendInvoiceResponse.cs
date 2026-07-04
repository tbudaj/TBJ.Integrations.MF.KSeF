using System.Text.Json.Serialization;

namespace TBJ.Integrations.MF.KSeF.Models.Sessions;

/// <summary>
/// Odpowiedź z wysyłki pojedynczej faktury w sesji online.
/// </summary>
public sealed class SendInvoiceResponse
{
    /// <summary>
    /// Numer KSeF przypisany fakturze.
    /// </summary>
    [JsonPropertyName("ksefNumber")]
    public string KsefNumber { get; set; } = string.Empty;

    /// <summary>
    /// Numer referencyjny operacji przetwarzania faktury.
    /// </summary>
    [JsonPropertyName("referenceNumber")]
    public string ReferenceNumber { get; set; } = string.Empty;

    /// <summary>
    /// Status przetwarzania faktury.
    /// </summary>
    [JsonPropertyName("status")]
    public InvoiceStatusInfo? Status { get; set; }
}
