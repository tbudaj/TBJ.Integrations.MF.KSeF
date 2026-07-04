namespace TBJ.Integrations.MF.KSeF.Pagination;

/// <summary>
/// Pojedyncza strona wyników z KSeF API z tokenem kontynuacji.
/// </summary>
/// <typeparam name="TItem">Typ elementu na stronie.</typeparam>
public interface IPage<TItem>
{
    /// <summary>
    /// Elementy bieżącej strony.
    /// </summary>
    IReadOnlyList<TItem> Items { get; }

    /// <summary>
    /// Token do pobrania kolejnej strony wyników.
    /// Wartość <c>null</c> oznacza brak kolejnych stron.
    /// </summary>
    string? ContinuationToken { get; }

    /// <summary>
    /// Informuje, czy dostępna jest kolejna strona wyników.
    /// </summary>
    bool HasNextPage { get; }
}
