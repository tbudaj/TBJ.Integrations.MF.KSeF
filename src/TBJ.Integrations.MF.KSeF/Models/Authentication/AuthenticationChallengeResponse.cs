using System.Text.Json.Serialization;

namespace TBJ.Integrations.MF.KSeF.Models.Authentication;

/// <summary>
/// Odpowiedź z challenge wymaganego do uwierzytelnienia w KSeF.
/// </summary>
public sealed class AuthenticationChallengeResponse
{
    /// <summary>
    /// Wartość challenge (ważna domyślnie 10 minut).
    /// </summary>
    [JsonPropertyName("challenge")]
    public string Challenge { get; set; } = string.Empty;

    /// <summary>
    /// Czas wygenerowania challenge.
    /// </summary>
    [JsonPropertyName("timestamp")]
    public long Timestamp { get; set; }
}
