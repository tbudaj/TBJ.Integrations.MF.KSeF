using System.Text.Json.Serialization;

namespace TBJ.Integrations.MF.KSeF.Models.Authentication;

/// <summary>
/// Identyfikator kontekstu uwierzytelniania w KSeF.
/// </summary>
public sealed class ContextIdentifier
{
    /// <summary>
    /// Rodzaj identyfikatora kontekstu.
    /// </summary>
    [JsonPropertyName("type")]
    public ContextIdentifierType Type { get; set; }

    /// <summary>
    /// Wartość identyfikatora kontekstu (np. numer NIP).
    /// </summary>
    [JsonPropertyName("identifier")]
    public string Identifier { get; set; } = string.Empty;
}
