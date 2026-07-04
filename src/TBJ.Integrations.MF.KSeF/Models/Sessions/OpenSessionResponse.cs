using System.Text.Json.Serialization;

namespace TBJ.Integrations.MF.KSeF.Models.Sessions;

/// <summary>
/// Odpowiedź z identyfikatorem i kluczem sesji wystawiania faktur.
/// </summary>
public sealed class OpenSessionResponse
{
    /// <summary>
    /// Identyfikator sesji.
    /// </summary>
    [JsonPropertyName("sessionIdentifier")]
    public SessionIdentifier SessionIdentifier { get; set; } = new();

    /// <summary>
    /// Klucz szyfrujący faktury w sesji.
    /// </summary>
    [JsonPropertyName("encryptionKey")]
    public string EncryptionKey { get; set; } = string.Empty;
}
