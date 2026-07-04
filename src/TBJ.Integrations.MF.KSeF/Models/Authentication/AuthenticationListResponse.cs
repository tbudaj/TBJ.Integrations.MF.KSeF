using System.Text.Json.Serialization;

namespace TBJ.Integrations.MF.KSeF.Models.Authentication;

/// <summary>
/// Odpowiedź z listą aktywnych sesji uwierzytelnienia.
/// </summary>
public sealed class AuthenticationListResponse
{
    /// <summary>
    /// Token do pobrania kolejnej strony wyników.
    /// </summary>
    [JsonPropertyName("continuationToken")]
    public string? ContinuationToken { get; set; }

    /// <summary>
    /// Lista aktywnych sesji uwierzytelnienia.
    /// </summary>
    [JsonPropertyName("items")]
    public IReadOnlyList<AuthenticationSession> Items { get; set; } = Array.Empty<AuthenticationSession>();
}
