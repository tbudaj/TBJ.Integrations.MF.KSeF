namespace TBJ.Integrations.MF.KSeF.Pagination;

/// <summary>
/// Domyślna implementacja strony wyników KSeF API.
/// </summary>
/// <typeparam name="TItem">Typ elementu na stronie.</typeparam>
public sealed class Page<TItem> : IPage<TItem>
{
    /// <summary>
    /// Inicjalizuje nową instancję strony wyników.
    /// </summary>
    public Page(IReadOnlyList<TItem> items, string? continuationToken)
    {
        Items = items;
        ContinuationToken = continuationToken;
    }

    /// <inheritdoc />
    public IReadOnlyList<TItem> Items { get; }

    /// <inheritdoc />
    public string? ContinuationToken { get; }

    /// <inheritdoc />
    public bool HasNextPage => !string.IsNullOrWhiteSpace(ContinuationToken);
}
