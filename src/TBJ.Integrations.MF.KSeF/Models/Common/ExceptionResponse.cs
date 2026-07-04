using System.Text.Json.Serialization;

namespace TBJ.Integrations.MF.KSeF.Models.Common;

/// <summary>
/// Model błędu zwracany przez KSeF API w formacie <c>ExceptionResponse</c>.
/// </summary>
public sealed class ExceptionResponse
{
    /// <summary>
    /// Kod wyjątku KSeF.
    /// </summary>
    [JsonPropertyName("exceptionCode")]
    public string? ExceptionCode { get; set; }

    /// <summary>
    /// Opis wyjątku KSeF.
    /// </summary>
    [JsonPropertyName("exceptionDescription")]
    public string? ExceptionDescription { get; set; }

    /// <summary>
    /// Dodatkowe szczegóły błędu.
    /// </summary>
    [JsonPropertyName("details")]
    public string? Details { get; set; }
}
