using System.Text.Json.Serialization;

namespace TBJ.Integrations.MF.KSeF.Models.Sessions;

/// <summary>
/// Odpowiedź z listą faktur w sesji.
/// </summary>
public sealed class SessionInvoiceListResponse
{
    /// <summary>
    /// Token do pobrania kolejnej strony wyników.
    /// </summary>
    [JsonPropertyName("continuationToken")]
    public string? ContinuationToken { get; set; }

    /// <summary>
    /// Lista faktur w sesji.
    /// </summary>
    [JsonPropertyName("items")]
    public IReadOnlyList<InvoiceInSession> Items { get; set; } = Array.Empty<InvoiceInSession>();
}
