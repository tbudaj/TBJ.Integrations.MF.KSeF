using System.Text.Json.Serialization;

namespace TBJ.Integrations.MF.KSeF.Models.Sessions;

/// <summary>
/// Identyfikator sesji wystawiania faktur.
/// </summary>
public sealed class SessionIdentifier
{
    /// <summary>
    /// Unikalny identyfikator sesji.
    /// </summary>
    [JsonPropertyName("sessionIdentifier")]
    public string SessionIdentifierValue { get; set; } = string.Empty;

    /// <summary>
    /// Numer referencyjny sesji.
    /// </summary>
    [JsonPropertyName("referenceNumber")]
    public string ReferenceNumber { get; set; } = string.Empty;
}
