using System.Text.Json.Serialization;

namespace TBJ.Integrations.MF.KSeF.Models.Authentication;

/// <summary>
/// Informacja o statusie operacji uwierzytelniania.
/// </summary>
public sealed class AuthenticationStatusInfo
{
    /// <summary>
    /// Kod statusu operacji.
    /// </summary>
    [JsonPropertyName("code")]
    public int Code { get; set; }

    /// <summary>
    /// Opis statusu operacji.
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
}
