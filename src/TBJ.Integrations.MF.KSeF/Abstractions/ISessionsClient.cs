using TBJ.Integrations.MF.KSeF.Models.Sessions;
using TBJ.Integrations.MF.KSeF.Pagination;

namespace TBJ.Integrations.MF.KSeF.Abstractions;

/// <summary>
/// Klient operacji sesji wystawiania faktur w KSeF API v2.
/// </summary>
public interface ISessionsClient
{
    /// <summary>
    /// Wyszukuje sesje spełniające podane kryteria.
    /// </summary>
    Task<IPage<SessionIdentifier>> QuerySessionsAsync(
        SessionType? sessionType = null,
        string? referenceNumber = null,
        DateTimeOffset? dateCreatedFrom = null,
        DateTimeOffset? dateCreatedTo = null,
        int? pageSize = null,
        string? continuationToken = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Iteruje przez wszystkie sesje spełniające kryteria.
    /// </summary>
    IAsyncEnumerable<SessionIdentifier> GetAllSessionsAsync(
        SessionType? sessionType = null,
        string? referenceNumber = null,
        DateTimeOffset? dateCreatedFrom = null,
        DateTimeOffset? dateCreatedTo = null,
        int? pageSize = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Otwiera sesję interaktywną do wysyłki pojedynczych faktur.
    /// </summary>
    /// <param name="request">Żądanie otwarcia sesji.</param>
    /// <param name="accessToken">Token dostępowy. Gdy null — używany <c>DefaultAccessToken</c> z opcji (Scenariusz B).</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    Task<OpenSessionResponse> OpenOnlineSessionAsync(
        OpenSessionRequest request,
        string? accessToken = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Zamyka sesję interaktywną.
    /// </summary>
    /// <param name="sessionIdentifier">Identyfikator sesji.</param>
    /// <param name="accessToken">Token dostępowy. Gdy null — używany <c>DefaultAccessToken</c> z opcji (Scenariusz B).</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    Task CloseOnlineSessionAsync(
        string sessionIdentifier,
        string? accessToken = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Wysyła pojedynczą fakturę w sesji online.
    /// </summary>
    /// <param name="sessionIdentifier">Identyfikator sesji.</param>
    /// <param name="invoiceContent">Treść faktury (XML).</param>
    /// <param name="metadata">Metadane faktury.</param>
    /// <param name="accessToken">Token dostępowy. Gdy null — używany <c>DefaultAccessToken</c> z opcji (Scenariusz B).</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    Task<SendInvoiceResponse> SendInvoiceAsync(
        string sessionIdentifier,
        Stream invoiceContent,
        InvoiceMetadata metadata,
        string? accessToken = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Otwiera sesję wsadową.
    /// </summary>
    /// <param name="request">Żądanie otwarcia sesji wsadowej.</param>
    /// <param name="accessToken">Token dostępowy. Gdy null — używany <c>DefaultAccessToken</c> z opcji (Scenariusz B).</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    Task<OpenSessionResponse> OpenBatchSessionAsync(
        OpenBatchSessionRequest request,
        string? accessToken = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Zamyka sesję wsadową.
    /// </summary>
    /// <param name="sessionIdentifier">Identyfikator sesji.</param>
    /// <param name="accessToken">Token dostępowy. Gdy null — używany <c>DefaultAccessToken</c> z opcji (Scenariusz B).</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    Task CloseBatchSessionAsync(
        string sessionIdentifier,
        string? accessToken = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Pobiera status sesji.
    /// </summary>
    /// <param name="sessionIdentifier">Identyfikator sesji.</param>
    /// <param name="accessToken">Token dostępowy. Gdy null — używany <c>DefaultAccessToken</c> z opcji (Scenariusz B).</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    Task<SessionStatusResponse> GetSessionStatusAsync(
        string sessionIdentifier,
        string? accessToken = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Pobiera UPO dla sesji.
    /// </summary>
    /// <param name="sessionIdentifier">Identyfikator sesji.</param>
    /// <param name="accessToken">Token dostępowy. Gdy null — używany <c>DefaultAccessToken</c> z opcji (Scenariusz B).</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    Task<SessionUpo> GetSessionUpoAsync(
        string sessionIdentifier,
        string? accessToken = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Pobiera jedną stronę faktur w sesji.
    /// </summary>
    Task<IPage<InvoiceInSession>> GetSessionInvoicesPageAsync(
        string sessionIdentifier,
        int? pageSize = null,
        string? continuationToken = null,
        string? accessToken = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Iteruje przez wszystkie faktury w sesji.
    /// </summary>
    IAsyncEnumerable<InvoiceInSession> GetAllSessionInvoicesAsync(
        string sessionIdentifier,
        int? pageSize = null,
        string? accessToken = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Pobiera listę faktur, które nie zostały przetworzone w sesji.
    /// </summary>
    Task<IPage<InvoiceInSession>> GetFailedInvoicesPageAsync(
        string sessionIdentifier,
        int? pageSize = null,
        string? continuationToken = null,
        string? accessToken = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Pobiera status pojedynczej faktury w sesji.
    /// </summary>
    /// <param name="sessionIdentifier">Identyfikator sesji.</param>
    /// <param name="ksefNumber">Numer KSeF faktury.</param>
    /// <param name="accessToken">Token dostępowy. Gdy null — używany <c>DefaultAccessToken</c> z opcji (Scenariusz B).</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    Task<InvoiceStatusInfo> GetInvoiceStatusAsync(
        string sessionIdentifier,
        string ksefNumber,
        string? accessToken = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Pobiera UPO dla pojedynczej faktury w sesji.
    /// </summary>
    /// <param name="sessionIdentifier">Identyfikator sesji.</param>
    /// <param name="ksefNumber">Numer KSeF faktury.</param>
    /// <param name="accessToken">Token dostępowy. Gdy null — używany <c>DefaultAccessToken</c> z opcji (Scenariusz B).</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    Task<SessionUpo> GetInvoiceUpoAsync(
        string sessionIdentifier,
        string ksefNumber,
        string? accessToken = null,
        CancellationToken cancellationToken = default);
}
