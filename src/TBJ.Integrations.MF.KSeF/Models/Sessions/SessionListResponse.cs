using System.Text.Json.Serialization;

namespace TBJ.Integrations.MF.KSeF.Models.Sessions;

/// <summary>
/// Odpowiedź z listą sesji wystawiania faktur.
/// </summary>
internal sealed class SessionListResponse
{
    /// <summary>
    /// Token do pobrania kolejnej strony wyników.
    /// </summary>
    [JsonPropertyName("continuationToken")]
    public string? ContinuationToken { get; set; }

    /// <summary>
    /// Lista sesji.
    /// </summary>
    [JsonPropertyName("items")]
    public IReadOnlyList<SessionIdentifier> Items { get; set; } = Array.Empty<SessionIdentifier>();
}
