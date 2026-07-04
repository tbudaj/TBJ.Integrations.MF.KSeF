using System.Text.Json.Serialization;

namespace TBJ.Integrations.MF.KSeF.Models.Authentication;

/// <summary>
/// Żądanie odświeżenia accessToken za pomocą refreshToken.
/// </summary>
public sealed class TokenRefreshRequest
{
    /// <summary>
    /// Token odświeżania.
    /// </summary>
    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; set; } = string.Empty;
}
