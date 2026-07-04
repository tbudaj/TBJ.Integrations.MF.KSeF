using System.Text.Json.Serialization;
using TBJ.Integrations.MF.KSeF.Models.Authentication;

namespace TBJ.Integrations.MF.KSeF.Models.Sessions;

/// <summary>
/// Żądanie otwarcia sesji wsadowej.
/// </summary>
public sealed class OpenBatchSessionRequest
{
    /// <summary>
    /// Identyfikator kontekstu, w którym otwierana jest sesja.
    /// </summary>
    [JsonPropertyName("contextIdentifier")]
    public ContextIdentifier ContextIdentifier { get; set; } = new();

    /// <summary>
    /// Opcjonalny identyfikator klasy faktur.
    /// </summary>
    [JsonPropertyName("invoiceType")]
    public string? InvoiceType { get; set; }

    /// <summary>
    /// Lista faktur do wysłania w sesji wsadowej.
    /// </summary>
    [JsonPropertyName("invoices")]
    public IReadOnlyList<BatchInvoice> Invoices { get; set; } = Array.Empty<BatchInvoice>();
}
