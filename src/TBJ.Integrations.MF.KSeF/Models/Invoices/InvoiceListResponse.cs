using System.Text.Json.Serialization;

namespace TBJ.Integrations.MF.KSeF.Models.Invoices;

/// <summary>
/// Odpowiedź z listą faktur.
/// </summary>
internal sealed class InvoiceListResponse
{
    /// <summary>
    /// Token do pobrania kolejnej strony wyników.
    /// </summary>
    [JsonPropertyName("continuationToken")]
    public string? ContinuationToken { get; set; }

    /// <summary>
    /// Lista faktur.
    /// </summary>
    [JsonPropertyName("items")]
    public IReadOnlyList<Invoice> Items { get; set; } = Array.Empty<Invoice>();
}
