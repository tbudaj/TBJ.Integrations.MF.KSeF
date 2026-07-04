using TBJ.Integrations.MF.KSeF.Abstractions;
using TBJ.Integrations.MF.KSeF.Configuration;
using TBJ.Integrations.MF.KSeF.Internal;
using TBJ.Integrations.MF.KSeF.Models.Invoices;
using TBJ.Integrations.MF.KSeF.Pagination;
using Microsoft.Extensions.Logging;

namespace TBJ.Integrations.MF.KSeF.Clients;

/// <summary>
/// Implementacja <see cref="IInvoicesClient"/> — obsługuje operacje na fakturach w KSeF.
/// </summary>
internal sealed class InvoicesClient : IInvoicesClient
{
    private readonly KSeFApiHttpClient _http;
    private readonly KSeFApiOptions _options;
    private readonly ILogger<InvoicesClient> _logger;

    private const string InvoicesEndpoint = "invoices";
    private const string ExportsEndpoint = "invoices/exports";

    public InvoicesClient(KSeFApiHttpClient http, KSeFApiOptions options, ILogger<InvoicesClient> logger)
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
            _logger.LogDebug("KSeF Invoices: używam accessToken per-request (Scenariusz A)");
            return accessToken;
        }

        if (!string.IsNullOrWhiteSpace(_options.DefaultAccessToken))
        {
            // Scenariusz B: domyślny token z konfiguracji
            _logger.LogInformation("KSeF Invoices: brak accessToken per-request — używam DefaultAccessToken z KSeFApiOptions (Scenariusz B)");
            return _options.DefaultAccessToken;
        }

        throw new InvalidOperationException(
            "Brak access token KSeF. Przekaż accessToken w parametrze metody (Scenariusz A) " +
            "lub skonfiguruj DefaultAccessToken w KSeFApiOptions (Scenariusz B).");
    }

    /// <inheritdoc />
    public async Task<IPage<Invoice>> QueryInvoicesAsync(
        string? ksefNumber = null,
        string? invoiceNumber = null,
        DateTimeOffset? dateFrom = null,
        DateTimeOffset? dateTo = null,
        string? partyName = null,
        string? partyNip = null,
        int? pageSize = null,
        string? continuationToken = null,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "KSeF: wyszukiwanie faktur — KSeFNumber: {KsefNumber}, InvoiceNumber: {InvoiceNumber}, PartyNip: {PartyNip}",
            ksefNumber,
            invoiceNumber,
            partyNip);

        var qs = QueryBuilder.Build(
            ("ksefNumber", ksefNumber),
            ("invoiceNumber", invoiceNumber),
            ("dateFrom", dateFrom?.ToString("O")),
            ("dateTo", dateTo?.ToString("O")),
            ("partyName", partyName),
            ("partyNip", partyNip),
            ("pageSize", pageSize?.ToString()));

        var json = await _http.GetAsync(InvoicesEndpoint, qs, accessToken, continuationToken, cancellationToken);
        var response = KSeFJsonSerializer.Deserialize<InvoiceListResponse>(json)
            ?? throw new InvalidOperationException("KSeF API zwróciło pustą odpowiedź listy faktur.");

        return new Page<Invoice>(response.Items, response.ContinuationToken);
    }

    /// <inheritdoc />
    public IAsyncEnumerable<Invoice> GetAllInvoicesAsync(
        string? ksefNumber = null,
        string? invoiceNumber = null,
        DateTimeOffset? dateFrom = null,
        DateTimeOffset? dateTo = null,
        string? partyName = null,
        string? partyNip = null,
        int? pageSize = null,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("KSeF: iteracja przez wszystkie faktury");
        return KSeFContinuationEnumerable.IterateAll(
            (token, ct) => QueryInvoicesAsync(ksefNumber, invoiceNumber, dateFrom, dateTo, partyName, partyNip, pageSize, token, accessToken, ct),
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Invoice> GetInvoiceAsync(
        string ksefNumber,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ksefNumber);
        var token = ResolveToken(accessToken);

        _logger.LogInformation("KSeF: pobieranie faktury {KsefNumber}", ksefNumber);
        var json = await _http.GetAsync($"{InvoicesEndpoint}/{ksefNumber}", bearerToken: token, cancellationToken: cancellationToken);
        return KSeFJsonSerializer.Deserialize<Invoice>(json)
            ?? throw new InvalidOperationException("KSeF API zwróciło pustą odpowiedź faktury.");
    }

    /// <inheritdoc />
    public async Task<InvoiceStatus> GetInvoiceStatusAsync(
        string ksefNumber,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ksefNumber);
        var token = ResolveToken(accessToken);

        _logger.LogDebug("KSeF: pobieranie statusu faktury {KsefNumber}", ksefNumber);
        var json = await _http.GetAsync($"{InvoicesEndpoint}/{ksefNumber}/status", bearerToken: token, cancellationToken: cancellationToken);
        return KSeFJsonSerializer.Deserialize<InvoiceStatus>(json)
            ?? throw new InvalidOperationException("KSeF API zwróciło pustą odpowiedź statusu faktury.");
    }

    /// <inheritdoc />
    public async Task<InvoiceUpo> GetInvoiceUpoAsync(
        string ksefNumber,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ksefNumber);
        var token = ResolveToken(accessToken);

        _logger.LogInformation("KSeF: pobieranie UPO faktury {KsefNumber}", ksefNumber);
        var json = await _http.GetAsync($"{InvoicesEndpoint}/{ksefNumber}/upo", bearerToken: token, cancellationToken: cancellationToken);
        return KSeFJsonSerializer.Deserialize<InvoiceUpo>(json)
            ?? throw new InvalidOperationException("KSeF API zwróciło pustą odpowiedź UPO faktury.");
    }

    /// <inheritdoc />
    public async Task<Stream> DownloadInvoiceAsync(
        string ksefNumber,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ksefNumber);
        var token = ResolveToken(accessToken);

        _logger.LogInformation("KSeF: pobieranie pliku faktury {KsefNumber}", ksefNumber);
        return await _http.GetStreamAsync($"{InvoicesEndpoint}/{ksefNumber}/download", bearerToken: token, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ExportResponse> InitiateExportAsync(
        ExportRequest request,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var token = ResolveToken(accessToken);

        _logger.LogInformation("KSeF: inicjowanie eksportu faktur w formacie {Format}", request.Format);
        var json = await _http.PostAsJsonAsync(ExportsEndpoint, request, token, cancellationToken);
        return KSeFJsonSerializer.Deserialize<ExportResponse>(json)
            ?? throw new InvalidOperationException("KSeF API zwróciło pustą odpowiedź inicjacji eksportu.");
    }

    /// <inheritdoc />
    public async Task<ExportStatus> GetExportStatusAsync(
        string exportId,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(exportId);
        var token = ResolveToken(accessToken);

        _logger.LogDebug("KSeF: pobieranie statusu eksportu {ExportId}", exportId);
        var json = await _http.GetAsync($"{ExportsEndpoint}/{exportId}", bearerToken: token, cancellationToken: cancellationToken);
        return KSeFJsonSerializer.Deserialize<ExportStatus>(json)
            ?? throw new InvalidOperationException("KSeF API zwróciło pustą odpowiedź statusu eksportu.");
    }

    /// <inheritdoc />
    public async Task<Stream> DownloadExportAsync(
        string exportId,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(exportId);
        var token = ResolveToken(accessToken);

        _logger.LogInformation("KSeF: pobieranie wyniku eksportu {ExportId}", exportId);
        return await _http.GetStreamAsync($"{ExportsEndpoint}/{exportId}/download", bearerToken: token, cancellationToken: cancellationToken);
    }
}
