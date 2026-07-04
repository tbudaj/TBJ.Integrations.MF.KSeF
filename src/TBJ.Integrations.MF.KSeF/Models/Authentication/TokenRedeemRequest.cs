using System.Text.Json.Serialization;

namespace TBJ.Integrations.MF.KSeF.Models.Authentication;

/// <summary>
/// Żądanie wymiany authenticationToken na tokeny dostępowe (accessToken i refreshToken).
/// </summary>
public sealed class TokenRedeemRequest
{
    /// <summary>
    /// Numer referencyjny operacji uwierzytelniania.
    /// </summary>
    [JsonPropertyName("referenceNumber")]
    public string ReferenceNumber { get; set; } = string.Empty;

    /// <summary>
    /// Tymczasowy token uwierzytelniania.
    /// </summary>
    [JsonPropertyName("authenticationToken")]
    public string AuthenticationToken { get; set; } = string.Empty;
}
