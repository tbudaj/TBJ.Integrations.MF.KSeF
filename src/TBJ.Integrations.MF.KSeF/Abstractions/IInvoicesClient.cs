using TBJ.Integrations.MF.KSeF.Models.Invoices;
using TBJ.Integrations.MF.KSeF.Pagination;

namespace TBJ.Integrations.MF.KSeF.Abstractions;

/// <summary>
/// Klient operacji na fakturach w KSeF API v2.
/// </summary>
public interface IInvoicesClient
{
    /// <summary>
    /// Wyszukuje faktury spełniające podane kryteria.
    /// </summary>
    Task<IPage<Invoice>> QueryInvoicesAsync(
        string? ksefNumber = null,
        string? invoiceNumber = null,
        DateTimeOffset? dateFrom = null,
        DateTimeOffset? dateTo = null,
        string? partyName = null,
        string? partyNip = null,
        int? pageSize = null,
        string? continuationToken = null,
        string? accessToken = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Iteruje przez wszystkie faktury spełniające kryteria.
    /// </summary>
    IAsyncEnumerable<Invoice> GetAllInvoicesAsync(
        string? ksefNumber = null,
        string? invoiceNumber = null,
        DateTimeOffset? dateFrom = null,
        DateTimeOffset? dateTo = null,
        string? partyName = null,
        string? partyNip = null,
        int? pageSize = null,
        string? accessToken = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Pobiera szczegóły faktury.
    /// </summary>
    /// <param name="ksefNumber">Numer KSeF faktury.</param>
    /// <param name="accessToken">Token dostępowy. Gdy null — używany <c>DefaultAccessToken</c> z opcji (Scenariusz B).</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    Task<Invoice> GetInvoiceAsync(
        string ksefNumber,
        string? accessToken = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Pobiera status faktury.
    /// </summary>
    /// <param name="ksefNumber">Numer KSeF faktury.</param>
    /// <param name="accessToken">Token dostępowy. Gdy null — używany <c>DefaultAccessToken</c> z opcji (Scenariusz B).</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    Task<InvoiceStatus> GetInvoiceStatusAsync(
        string ksefNumber,
        string? accessToken = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Pobiera UPO dla faktury.
    /// </summary>
    /// <param name="ksefNumber">Numer KSeF faktury.</param>
    /// <param name="accessToken">Token dostępowy. Gdy null — używany <c>DefaultAccessToken</c> z opcji (Scenariusz B).</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    Task<InvoiceUpo> GetInvoiceUpoAsync(
        string ksefNumber,
        string? accessToken = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Pobiera plik faktury.
    /// </summary>
    /// <param name="ksefNumber">Numer KSeF faktury.</param>
    /// <param name="accessToken">Token dostępowy. Gdy null — używany <c>DefaultAccessToken</c> z opcji (Scenariusz B).</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    Task<Stream> DownloadInvoiceAsync(
        string ksefNumber,
        string? accessToken = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Inicjuje eksport faktur.
    /// </summary>
    /// <param name="request">Żądanie eksportu.</param>
    /// <param name="accessToken">Token dostępowy. Gdy null — używany <c>DefaultAccessToken</c> z opcji (Scenariusz B).</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    Task<ExportResponse> InitiateExportAsync(
        ExportRequest request,
        string? accessToken = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Pobiera status zleconego eksportu.
    /// </summary>
    /// <param name="exportId">Identyfikator eksportu.</param>
    /// <param name="accessToken">Token dostępowy. Gdy null — używany <c>DefaultAccessToken</c> z opcji (Scenariusz B).</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    Task<ExportStatus> GetExportStatusAsync(
        string exportId,
        string? accessToken = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Pobiera wynik zleconego eksportu.
    /// </summary>
    /// <param name="exportId">Identyfikator eksportu.</param>
    /// <param name="accessToken">Token dostępowy. Gdy null — używany <c>DefaultAccessToken</c> z opcji (Scenariusz B).</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    Task<Stream> DownloadExportAsync(
        string exportId,
        string? accessToken = null,
        CancellationToken cancellationToken = default);
}
