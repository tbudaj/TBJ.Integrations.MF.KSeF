using System.Text.Json.Serialization;

namespace TBJ.Integrations.MF.KSeF.Models.Authentication;

/// <summary>
/// Informacja o metodzie uwierzytelniania użytej w sesji KSeF.
/// </summary>
public sealed class AuthenticationMethodInfo
{
    /// <summary>
    /// Kategoria metody uwierzytelniania (np. <c>XadesSignature</c>).
    /// </summary>
    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Kod metody uwierzytelniania.
    /// </summary>
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Czytelna nazwa metody uwierzytelniania.
    /// </summary>
    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; } = string.Empty;
}
