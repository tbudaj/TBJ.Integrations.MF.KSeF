using System.Text.Json.Serialization;

namespace TBJ.Integrations.MF.KSeF.Models.Sessions;

/// <summary>
/// Informacja o statusie sesji.
/// </summary>
public sealed class SessionStatusInfo
{
    /// <summary>
    /// Kod statusu sesji.
    /// </summary>
    [JsonPropertyName("code")]
    public SessionStatusCode Code { get; set; }

    /// <summary>
    /// Opis statusu sesji.
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
}
