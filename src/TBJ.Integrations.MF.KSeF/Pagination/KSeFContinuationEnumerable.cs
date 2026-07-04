namespace TBJ.Integrations.MF.KSeF.Pagination;

/// <summary>
/// Pomocnik do iteracji przez wszystkie strony wyników KSeF API
/// na podstawie tokenu kontynuacji (<c>x-continuation-token</c>).
/// </summary>
internal static class KSeFContinuationEnumerable
{
    /// <summary>
    /// Iteruje przez wszystkie elementy ze wszystkich stron wyników.
    /// </summary>
    /// <typeparam name="TItem">Typ elementu listy wyników.</typeparam>
    /// <param name="fetchPage">Funkcja pobierająca stronę wyników na podstawie tokenu kontynuacji.</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    public static async IAsyncEnumerable<TItem> IterateAll<TItem>(
        Func<string?, CancellationToken, Task<IPage<TItem>>> fetchPage,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? continuationToken = null;

        while (true)
        {
            var page = await fetchPage(continuationToken, cancellationToken);

            foreach (var item in page.Items)
                yield return item;

            if (!page.HasNextPage)
                break;

            continuationToken = page.ContinuationToken;
        }
    }
}
