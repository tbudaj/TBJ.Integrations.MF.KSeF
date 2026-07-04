using System.Text.Json.Serialization;

namespace TBJ.Integrations.MF.KSeF.Models.Authentication;

/// <summary>
/// Odpowiedź z nowym tokenem dostępowym po odświeżeniu.
/// </summary>
public sealed class TokenRefreshResponse
{
    /// <summary>
    /// Nowy token dostępowy JWT.
    /// </summary>
    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Nowy token odświeżania.
    /// </summary>
    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// Czas ważności accessToken w sekundach.
    /// </summary>
    [JsonPropertyName("expiresIn")]
    public long ExpiresIn { get; set; }
}
