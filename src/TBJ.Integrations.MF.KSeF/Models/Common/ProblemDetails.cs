using System.Text.Json.Serialization;

namespace TBJ.Integrations.MF.KSeF.Models.Common;

/// <summary>
/// Model błędu zwracany przez KSeF API w formacie Problem Details (RFC 7807).
/// </summary>
public sealed class ProblemDetails
{
    /// <summary>
    /// Krótki, czytelny tytuł problemu.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// Kod statusu HTTP.
    /// </summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>
    /// Szczegółowy opis problemu.
    /// </summary>
    [JsonPropertyName("detail")]
    public string? Detail { get; set; }

    /// <summary>
    /// Identyfikator instancji żądania (np. referencyjny numer sesji).
    /// </summary>
    [JsonPropertyName("instance")]
    public string? Instance { get; set; }

    /// <summary>
    /// Kod wyjątku KSeF.
    /// </summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }
}
