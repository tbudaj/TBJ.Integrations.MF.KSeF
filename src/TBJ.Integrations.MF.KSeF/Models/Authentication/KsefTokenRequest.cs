using System.Text.Json.Serialization;

namespace TBJ.Integrations.MF.KSeF.Models.Authentication;

/// <summary>
/// Żądanie uwierzytelnienia za pomocą tokena KSeF.
/// </summary>
public sealed class KsefTokenRequest
{
    /// <summary>
    /// Token KSeF zaszyfrowany kluczem publicznym Ministerstwa Finansów.
    /// </summary>
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp żądania (Unix timestamp w milisekundach).
    /// </summary>
    [JsonPropertyName("timestamp")]
    public long Timestamp { get; set; }

    /// <summary>
    /// Identyfikator kontekstu uwierzytelniania.
    /// </summary>
    [JsonPropertyName("contextIdentifier")]
    public ContextIdentifier ContextIdentifier { get; set; } = new();

    /// <summary>
    /// Sposób identyfikacji podmiotu na podstawie certyfikatu.
    /// </summary>
    [JsonPropertyName("subjectIdentifierType")]
    public SubjectIdentifierType SubjectIdentifierType { get; set; }

    /// <summary>
    /// Opcjonalna polityka autoryzacji dla tokena.
    /// </summary>
    [JsonPropertyName("authorizationPolicy")]
    public AuthorizationPolicy? AuthorizationPolicy { get; set; }
}
