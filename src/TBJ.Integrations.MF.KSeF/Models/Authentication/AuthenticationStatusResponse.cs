using System.Text.Json.Serialization;

namespace TBJ.Integrations.MF.KSeF.Models.Authentication;

/// <summary>
/// Odpowiedź ze statusem operacji uwierzytelniania.
/// </summary>
public sealed class AuthenticationStatusResponse
{
    /// <summary>
    /// Numer referencyjny operacji uwierzytelniania.
    /// </summary>
    [JsonPropertyName("referenceNumber")]
    public string ReferenceNumber { get; set; } = string.Empty;

    /// <summary>
    /// Status operacji uwierzytelniania.
    /// </summary>
    [JsonPropertyName("status")]
    public AuthenticationStatusInfo Status { get; set; } = new();

    /// <summary>
    /// Informacja o metodzie uwierzytelniania.
    /// </summary>
    [JsonPropertyName("authenticationMethodInfo")]
    public AuthenticationMethodInfo? AuthenticationMethodInfo { get; set; }

    /// <summary>
    /// Czas rozpoczęcia sesji uwierzytelniania.
    /// </summary>
    [JsonPropertyName("startDate")]
    public DateTimeOffset? StartDate { get; set; }
}
