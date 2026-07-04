using System.Text.Json.Serialization;

namespace TBJ.Integrations.MF.KSeF.Models.Sessions;

/// <summary>
/// Urzędowe Potwierdzenie Odbioru (UPO) dla sesji.
/// </summary>
public sealed class SessionUpo
{
    /// <summary>
    /// Numer referencyjny sesji.
    /// </summary>
    [JsonPropertyName("referenceNumber")]
    public string ReferenceNumber { get; set; } = string.Empty;

    /// <summary>
    /// Treść UPO (Base64) lub identyfikator do pobrania.
    /// </summary>
    [JsonPropertyName("upo")]
    public string? Upo { get; set; }

    /// <summary>
    /// Data wygenerowania UPO.
    /// </summary>
    [JsonPropertyName("generationDate")]
    public DateTimeOffset? GenerationDate { get; set; }
}
