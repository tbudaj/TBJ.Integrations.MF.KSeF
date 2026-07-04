using System.Text.Json.Serialization;

namespace TBJ.Integrations.MF.KSeF.Models.Authentication;

/// <summary>
/// Odpowiedź z tokenami dostępowymi KSeF.
/// </summary>
public sealed class TokenRedeemResponse
{
    /// <summary>
    /// Token dostępowy JWT używany do autoryzacji operacji API.
    /// </summary>
    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Token do odświeżania accessToken.
    /// </summary>
    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// Czas ważności accessToken w sekundach.
    /// </summary>
    [JsonPropertyName("expiresIn")]
    public long ExpiresIn { get; set; }
}
