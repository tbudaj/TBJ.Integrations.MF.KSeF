using System.Text.Json.Serialization;
using TBJ.Integrations.MF.KSeF.Models.Authentication;

namespace TBJ.Integrations.MF.KSeF.Models.Sessions;

/// <summary>
/// Żądanie otwarcia sesji wystawiania faktur (online lub batch).
/// </summary>
public sealed class OpenSessionRequest
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
}
