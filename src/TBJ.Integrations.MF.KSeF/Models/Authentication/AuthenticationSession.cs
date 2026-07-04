using System.Text.Json.Serialization;

namespace TBJ.Integrations.MF.KSeF.Models.Authentication;

/// <summary>
/// Pojedyncza aktywna sesja uwierzytelnienia w KSeF.
/// </summary>
public sealed class AuthenticationSession
{
    /// <summary>
    /// Numer referencyjny sesji uwierzytelnienia.
    /// </summary>
    [JsonPropertyName("referenceNumber")]
    public string ReferenceNumber { get; set; } = string.Empty;

    /// <summary>
    /// Czy sesja jest aktualną sesją powiązaną z użytym tokenem.
    /// </summary>
    [JsonPropertyName("isCurrent")]
    public bool IsCurrent { get; set; }

    /// <summary>
    /// Czas rozpoczęcia sesji.
    /// </summary>
    [JsonPropertyName("startDate")]
    public DateTimeOffset StartDate { get; set; }

    /// <summary>
    /// Nazwa metody uwierzytelniania (np. <c>QualifiedSeal</c>).
    /// </summary>
    [JsonPropertyName("authenticationMethod")]
    public string AuthenticationMethod { get; set; } = string.Empty;

    /// <summary>
    /// Informacja o metodzie uwierzytelniania.
    /// </summary>
    [JsonPropertyName("authenticationMethodInfo")]
    public AuthenticationMethodInfo? AuthenticationMethodInfo { get; set; }

    /// <summary>
    /// Status sesji uwierzytelnienia.
    /// </summary>
    [JsonPropertyName("status")]
    public AuthenticationStatusInfo Status { get; set; } = new();

    /// <summary>
    /// Czy token został już wykorzystany.
    /// </summary>
    [JsonPropertyName("isTokenRedeemed")]
    public bool IsTokenRedeemed { get; set; }

    /// <summary>
    /// Czas ważności refreshToken.
    /// </summary>
    [JsonPropertyName("refreshTokenValidUntil")]
    public DateTimeOffset? RefreshTokenValidUntil { get; set; }
}
