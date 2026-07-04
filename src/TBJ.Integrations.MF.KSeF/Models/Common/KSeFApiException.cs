namespace TBJ.Integrations.MF.KSeF.Models.Common;

/// <summary>
/// Wyjątek reprezentujący błąd zwrócony przez KSeF API v2.
/// </summary>
public class KSeFApiException : Exception
{
    /// <summary>
    /// Inicjalizuje nową instancję wyjątku z podstawowym komunikatem.
    /// </summary>
    public KSeFApiException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Inicjalizuje nową instancję wyjątku z komunikatem i przyczyną wewnętrzną.
    /// </summary>
    public KSeFApiException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Inicjalizuje nową instancję wyjątku z kodem HTTP i szczegółami błędu.
    /// </summary>
    public KSeFApiException(string message, int statusCode, string? ksefExceptionCode = null, string? description = null, string? details = null)
        : base(message)
    {
        StatusCode = statusCode;
        KsefExceptionCode = ksefExceptionCode;
        Description = description;
        Details = details;
    }

    /// <summary>
    /// Kod HTTP odpowiedzi z KSeF API.
    /// </summary>
    public int? StatusCode { get; }

    /// <summary>
    /// Kod wyjątku KSeF (np. <c>21405</c>).
    /// </summary>
    public string? KsefExceptionCode { get; }

    /// <summary>
    /// Opis wyjątku KSeF.
    /// </summary>
    public string? Description { get; }

    /// <summary>
    /// Dodatkowe szczegóły błędu (np. treść z walidatora).
    /// </summary>
    public string? Details { get; }
}
