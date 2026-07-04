using System.Text.Json.Serialization;

namespace TBJ.Integrations.MF.KSeF.Models.Authentication;

/// <summary>
/// Odpowiedź z tymczasowym tokenem uwierzytelniania oraz numerem referencyjnym sesji.
/// </summary>
public sealed class AuthenticationTokenResponse
{
    /// <summary>
    /// Tymczasowy token uwierzytelniania (authenticationToken).
    /// </summary>
    [JsonPropertyName("authenticationToken")]
    public string AuthenticationToken { get; set; } = string.Empty;

    /// <summary>
    /// Numer referencyjny operacji uwierzytelniania.
    /// </summary>
    [JsonPropertyName("referenceNumber")]
    public string ReferenceNumber { get; set; } = string.Empty;
}
