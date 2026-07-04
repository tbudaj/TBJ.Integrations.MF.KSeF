using TBJ.Integrations.MF.KSeF.Abstractions;
using TBJ.Integrations.MF.KSeF.Configuration;
using TBJ.Integrations.MF.KSeF.Internal;
using TBJ.Integrations.MF.KSeF.Models.Sessions;
using TBJ.Integrations.MF.KSeF.Pagination;
using Microsoft.Extensions.Logging;

namespace TBJ.Integrations.MF.KSeF.Clients;

/// <summary>
/// Implementacja <see cref="ISessionsClient"/> — obsługuje sesje wystawiania faktur w KSeF.
/// </summary>
internal sealed class SessionsClient : ISessionsClient
{
    private readonly KSeFApiHttpClient _http;
    private readonly KSeFApiOptions _options;
    private readonly ILogger<SessionsClient> _logger;

    private const string SessionsEndpoint = "sessions";
    private const string OnlineOpenEndpoint = "sessions/online/open";
    private const string OnlineCloseEndpoint = "sessions/online/close";
    private const string OnlineSendEndpoint = "sessions/online/send-invoice";
    private const string BatchOpenEndpoint = "sessions/batch/open";
    private const string BatchCloseEndpoint = "sessions/batch/close";

    public SessionsClient(KSeFApiHttpClient http, KSeFApiOptions options, ILogger<SessionsClient> logger)
    {
        _http = http;
        _options = options;
        _logger = logger;
    }

    /// <summary>
    /// Rozwiązuje access token na podstawie priorytetu:
    /// per-żądanie (Scenariusz A) → <see cref="KSeFApiOptions.DefaultAccessToken"/> (Scenariusz B).
    /// </summary>
    /// <param name="accessToken">Token przekazany przez wywołującego (opcjonalny).</param>
    /// <returns>Zrealizowany access token.</returns>
    /// <exception cref="InvalidOperationException">Gdy brak tokenu w obu źródłach.</exception>
    private string ResolveToken(string? accessToken)
    {
        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            // Scenariusz A: token tenanta przekazany per-żądanie
            _logger.LogDebug("KSeF Sessions: używam accessToken per-request (Scenariusz A)");
            return accessToken;
        }

        if (!string.IsNullOrWhiteSpace(_options.DefaultAccessToken))
        {
            // Scenariusz B: domyślny token z konfiguracji
            _logger.LogInformation("KSeF Sessions: brak accessToken per-request — używam DefaultAccessToken z KSeFApiOptions (Scenariusz B)");
            return _options.DefaultAccessToken;
        }

        throw new InvalidOperationException(
            "Brak access token KSeF. Przekaż accessToken w parametrze metody (Scenariusz A) " +
            "lub skonfiguruj DefaultAccessToken w KSeFApiOptions (Scenariusz B).");
    }

    /// <inheritdoc />
    public async Task<IPage<SessionIdentifier>> QuerySessionsAsync(
        SessionType? sessionType = null,
        string? referenceNumber = null,
        DateTimeOffset? dateCreatedFrom = null,
        DateTimeOffset? dateCreatedTo = null,
        int? pageSize = null,
        string? continuationToken = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "KSeF: wyszukiwanie sesji — Type: {SessionType}, Reference: {ReferenceNumber}",
            sessionType,
            referenceNumber);

        var qs = QueryBuilder.Build(
            ("sessionType", sessionType?.ToString()),
            ("referenceNumber", referenceNumber),
            ("dateCreatedFrom", dateCreatedFrom?.ToString("O")),
            ("dateCreatedTo", dateCreatedTo?.ToString("O")),
            ("pageSize", pageSize?.ToString()));

        var json = await _http.GetAsync(SessionsEndpoint, qs, continuationToken: continuationToken, cancellationToken: cancellationToken);
        var response = KSeFJsonSerializer.Deserialize<SessionListResponse>(json)
            ?? throw new InvalidOperationException("KSeF API zwróciło pustą odpowiedź listy sesji.");

        return new Page<SessionIdentifier>(response.Items, response.ContinuationToken);
    }

    /// <inheritdoc />
    public IAsyncEnumerable<SessionIdentifier> GetAllSessionsAsync(
        SessionType? sessionType = null,
        string? referenceNumber = null,
        DateTimeOffset? dateCreatedFrom = null,
        DateTimeOffset? dateCreatedTo = null,
        int? pageSize = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("KSeF: iteracja przez wszystkie sesje");
        return KSeFContinuationEnumerable.IterateAll(
            (token, ct) => QuerySessionsAsync(sessionType, referenceNumber, dateCreatedFrom, dateCreatedTo, pageSize, token, ct),
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<OpenSessionResponse> OpenOnlineSessionAsync(
        OpenSessionRequest request,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var token = ResolveToken(accessToken);

        _logger.LogInformation("KSeF: otwieranie sesji online dla kontekstu {ContextType}", request.ContextIdentifier.Type);
        var json = await _http.PostAsJsonAsync(OnlineOpenEndpoint, request, token, cancellationToken);
        return KSeFJsonSerializer.Deserialize<OpenSessionResponse>(json)
            ?? throw new InvalidOperationException("KSeF API zwróciło pustą odpowiedź otwarcia sesji online.");
    }

    /// <inheritdoc />
    public async Task CloseOnlineSessionAsync(
        string sessionIdentifier,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sessionIdentifier);
        var token = ResolveToken(accessToken);

        _logger.LogInformation("KSeF: zamykanie sesji online {SessionIdentifier}", sessionIdentifier);
        await _http.PostAsJsonAsync($"{OnlineCloseEndpoint}/{sessionIdentifier}", new object(), token, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<SendInvoiceResponse> SendInvoiceAsync(
        string sessionIdentifier,
        Stream invoiceContent,
        InvoiceMetadata metadata,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sessionIdentifier);
        ArgumentNullException.ThrowIfNull(invoiceContent);
        var token = ResolveToken(accessToken);

        _logger.LogInformation("KSeF: wysyłka faktury w sesji online {SessionIdentifier}", sessionIdentifier);

        using var content = new MultipartFormDataContent();
        content.Add(new StreamContent(invoiceContent), "invoice", "invoice.xml");
        content.Add(new StringContent(KSeFJsonSerializer.Serialize(metadata)), "metadata");

        var json = await _http.PostMultipartAsync(
            $"{OnlineSendEndpoint}/{sessionIdentifier}",
            content,
            token,
            cancellationToken);

        return KSeFJsonSerializer.Deserialize<SendInvoiceResponse>(json)
            ?? throw new InvalidOperationException("KSeF API zwróciło pustą odpowiedź wysyłki faktury.");
    }

    /// <inheritdoc />
    public async Task<OpenSessionResponse> OpenBatchSessionAsync(
        OpenBatchSessionRequest request,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var token = ResolveToken(accessToken);

        _logger.LogInformation(
            "KSeF: otwieranie sesji wsadowej dla kontekstu {ContextType} z {Count} fakturami",
            request.ContextIdentifier.Type,
            request.Invoices.Count);

        var json = await _http.PostAsJsonAsync(BatchOpenEndpoint, request, token, cancellationToken);
        return KSeFJsonSerializer.Deserialize<OpenSessionResponse>(json)
            ?? throw new InvalidOperationException("KSeF API zwróciło pustą odpowiedź otwarcia sesji wsadowej.");
    }

    /// <inheritdoc />
    public async Task CloseBatchSessionAsync(
        string sessionIdentifier,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sessionIdentifier);
        var token = ResolveToken(accessToken);

        _logger.LogInformation("KSeF: zamykanie sesji wsadowej {SessionIdentifier}", sessionIdentifier);
        await _http.PostAsJsonAsync($"{BatchCloseEndpoint}/{sessionIdentifier}", new object(), token, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<SessionStatusResponse> GetSessionStatusAsync(
        string sessionIdentifier,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sessionIdentifier);
        var token = ResolveToken(accessToken);

        _logger.LogDebug("KSeF: pobieranie statusu sesji {SessionIdentifier}", sessionIdentifier);
        var json = await _http.GetAsync($"{SessionsEndpoint}/{sessionIdentifier}/status", bearerToken: token, cancellationToken: cancellationToken);
        return KSeFJsonSerializer.Deserialize<SessionStatusResponse>(json)
            ?? throw new InvalidOperationException("KSeF API zwróciło pustą odpowiedź statusu sesji.");
    }

    /// <inheritdoc />
    public async Task<SessionUpo> GetSessionUpoAsync(
        string sessionIdentifier,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sessionIdentifier);
        var token = ResolveToken(accessToken);

        _logger.LogInformation("KSeF: pobieranie UPO dla sesji {SessionIdentifier}", sessionIdentifier);
        var json = await _http.GetAsync($"{SessionsEndpoint}/{sessionIdentifier}/upo", bearerToken: token, cancellationToken: cancellationToken);
        return KSeFJsonSerializer.Deserialize<SessionUpo>(json)
            ?? throw new InvalidOperationException("KSeF API zwróciło pustą odpowiedź UPO sesji.");
    }

    /// <inheritdoc />
    public async Task<IPage<InvoiceInSession>> GetSessionInvoicesPageAsync(
        string sessionIdentifier,
        int? pageSize = null,
        string? continuationToken = null,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sessionIdentifier);

        _logger.LogDebug("KSeF: pobieranie strony faktur sesji {SessionIdentifier}", sessionIdentifier);
        var qs = QueryBuilder.Build(("pageSize", pageSize?.ToString()));
        var json = await _http.GetAsync(
            $"{SessionsEndpoint}/{sessionIdentifier}/invoices",
            qs,
            accessToken,
            continuationToken,
            cancellationToken);

        var response = KSeFJsonSerializer.Deserialize<SessionInvoiceListResponse>(json)
            ?? throw new InvalidOperationException("KSeF API zwróciło pustą odpowiedź listy faktur sesji.");

        return new Page<InvoiceInSession>(response.Items, response.ContinuationToken);
    }

    /// <inheritdoc />
    public IAsyncEnumerable<InvoiceInSession> GetAllSessionInvoicesAsync(
        string sessionIdentifier,
        int? pageSize = null,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("KSeF: iteracja przez wszystkie faktury sesji {SessionIdentifier}", sessionIdentifier);
        return KSeFContinuationEnumerable.IterateAll(
            (token, ct) => GetSessionInvoicesPageAsync(sessionIdentifier, pageSize, token, accessToken, ct),
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IPage<InvoiceInSession>> GetFailedInvoicesPageAsync(
        string sessionIdentifier,
        int? pageSize = null,
        string? continuationToken = null,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sessionIdentifier);

        _logger.LogInformation("KSeF: pobieranie faktur odrzuconych w sesji {SessionIdentifier}", sessionIdentifier);
        var qs = QueryBuilder.Build(("pageSize", pageSize?.ToString()));
        var json = await _http.GetAsync(
            $"{SessionsEndpoint}/{sessionIdentifier}/invoices/failed",
            qs,
            accessToken,
            continuationToken,
            cancellationToken);

        var response = KSeFJsonSerializer.Deserialize<SessionInvoiceListResponse>(json)
            ?? throw new InvalidOperationException("KSeF API zwróciło pustą odpowiedź listy faktur odrzuconych.");

        return new Page<InvoiceInSession>(response.Items, response.ContinuationToken);
    }

    /// <inheritdoc />
    public async Task<InvoiceStatusInfo> GetInvoiceStatusAsync(
        string sessionIdentifier,
        string ksefNumber,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sessionIdentifier);
        ArgumentException.ThrowIfNullOrWhiteSpace(ksefNumber);
        var token = ResolveToken(accessToken);

        _logger.LogDebug("KSeF: pobieranie statusu faktury {KsefNumber} w sesji {SessionIdentifier}", ksefNumber, sessionIdentifier);
        var json = await _http.GetAsync(
            $"{SessionsEndpoint}/{sessionIdentifier}/invoices/{ksefNumber}/status",
            bearerToken: token,
            cancellationToken: cancellationToken);

        return KSeFJsonSerializer.Deserialize<InvoiceStatusInfo>(json)
            ?? throw new InvalidOperationException("KSeF API zwróciło pustą odpowiedź statusu faktury.");
    }

    /// <inheritdoc />
    public async Task<SessionUpo> GetInvoiceUpoAsync(
        string sessionIdentifier,
        string ksefNumber,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sessionIdentifier);
        ArgumentException.ThrowIfNullOrWhiteSpace(ksefNumber);
        var token = ResolveToken(accessToken);

        _logger.LogInformation("KSeF: pobieranie UPO faktury {KsefNumber} w sesji {SessionIdentifier}", ksefNumber, sessionIdentifier);
        var json = await _http.GetAsync(
            $"{SessionsEndpoint}/{sessionIdentifier}/invoices/{ksefNumber}/upo",
            bearerToken: token,
            cancellationToken: cancellationToken);

        return KSeFJsonSerializer.Deserialize<SessionUpo>(json)
            ?? throw new InvalidOperationException("KSeF API zwróciło pustą odpowiedź UPO faktury.");
    }
}
