using TBJ.Integrations.MF.KSeF.Abstractions;
using TBJ.Integrations.MF.KSeF.Configuration;
using TBJ.Integrations.MF.KSeF.Internal;
using TBJ.Integrations.MF.KSeF.Models.Authentication;
using TBJ.Integrations.MF.KSeF.Pagination;
using Microsoft.Extensions.Logging;

namespace TBJ.Integrations.MF.KSeF.Clients;

/// <summary>
/// Implementacja <see cref="IAuthenticationClient"/> — obsługuje uwierzytelnianie
/// i zarządzanie sesjami w KSeF API v2.
/// </summary>
internal sealed class AuthenticationClient : IAuthenticationClient
{
    private readonly KSeFApiHttpClient _http;
    private readonly KSeFApiOptions _options;
    private readonly ILogger<AuthenticationClient> _logger;

    private const string ChallengeEndpoint = "auth/challenge";
    private const string XadesSignatureEndpoint = "auth/xades-signature";
    private const string KsefTokenEndpoint = "auth/ksef-token";
    private const string AuthStatusEndpoint = "auth";
    private const string TokenRedeemEndpoint = "auth/token/redeem";
    private const string TokenRefreshEndpoint = "auth/token/refresh";
    private const string SessionsEndpoint = "auth/sessions";
    private const string CurrentSessionEndpoint = "auth/sessions/current";

    public AuthenticationClient(KSeFApiHttpClient http, KSeFApiOptions options, ILogger<AuthenticationClient> logger)
    {
        _http = http;
        _options = options;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<AuthenticationChallengeResponse> GetChallengeAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("KSeF: pobieranie challenge");
        var json = await _http.PostAsJsonAsync(ChallengeEndpoint, new object(), cancellationToken: cancellationToken);
        return KSeFJsonSerializer.Deserialize<AuthenticationChallengeResponse>(json)
            ?? throw new InvalidOperationException("KSeF API zwróciło pustą odpowiedź challenge.");
    }

    /// <inheritdoc />
    public async Task<AuthenticationTokenResponse> AuthenticateWithXadesAsync(
        string signedAuthTokenRequest,
        bool? verifyCertificateChain = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(signedAuthTokenRequest);

        _logger.LogInformation("KSeF: uwierzytelnianie XAdES");
        var endpoint = XadesSignatureEndpoint;
        if (verifyCertificateChain.HasValue)
            endpoint += $"?verifyCertificateChain={verifyCertificateChain.Value.ToString().ToLowerInvariant()}";

        var json = await _http.PostAsXmlAsync(endpoint, signedAuthTokenRequest, cancellationToken: cancellationToken);
        return KSeFJsonSerializer.Deserialize<AuthenticationTokenResponse>(json)
            ?? throw new InvalidOperationException("KSeF API zwróciło pustą odpowiedź uwierzytelniania XAdES.");
    }

    /// <inheritdoc />
    public async Task<AuthenticationTokenResponse> AuthenticateWithTokenAsync(
        KsefTokenRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        _logger.LogInformation("KSeF: uwierzytelnianie tokenem KSeF dla kontekstu {ContextType}", request.ContextIdentifier.Type);
        var json = await _http.PostAsJsonAsync(KsefTokenEndpoint, request, cancellationToken: cancellationToken);
        return KSeFJsonSerializer.Deserialize<AuthenticationTokenResponse>(json)
            ?? throw new InvalidOperationException("KSeF API zwróciło pustą odpowiedź uwierzytelniania tokenem.");
    }

    /// <inheritdoc />
    public async Task<AuthenticationStatusResponse> GetStatusAsync(
        string referenceNumber,
        string authenticationToken,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(referenceNumber);
        ArgumentException.ThrowIfNullOrWhiteSpace(authenticationToken);

        _logger.LogDebug("KSeF: sprawdzanie statusu uwierzytelniania {ReferenceNumber}", referenceNumber);
        var json = await _http.GetAsync($"{AuthStatusEndpoint}/{referenceNumber}", bearerToken: authenticationToken, cancellationToken: cancellationToken);
        return KSeFJsonSerializer.Deserialize<AuthenticationStatusResponse>(json)
            ?? throw new InvalidOperationException("KSeF API zwróciło pustą odpowiedź statusu uwierzytelniania.");
    }

    /// <inheritdoc />
    public async Task<AuthenticationStatusResponse> WaitForAuthenticationCompletedAsync(
        string referenceNumber,
        string authenticationToken,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(referenceNumber);
        ArgumentException.ThrowIfNullOrWhiteSpace(authenticationToken);

        _logger.LogInformation(
            "KSeF: oczekiwanie na zakończenie uwierzytelniania {ReferenceNumber} (max {MaxAttempts} prób, co {Delay}s)",
            referenceNumber,
            _options.MaxAuthenticationStatusPollAttempts,
            _options.AuthenticationStatusPollDelay.TotalSeconds);

        for (var attempt = 0; attempt < _options.MaxAuthenticationStatusPollAttempts; attempt++)
        {
            var status = await GetStatusAsync(referenceNumber, authenticationToken, cancellationToken);

            if (status.Status.Code == 200)
            {
                _logger.LogInformation("KSeF: uwierzytelnianie {ReferenceNumber} zakończone sukcesem", referenceNumber);
                return status;
            }

            if (status.Status.Code >= 400)
            {
                _logger.LogWarning(
                    "KSeF: uwierzytelnianie {ReferenceNumber} zakończone błędem {StatusCode} — {Description}",
                    referenceNumber,
                    status.Status.Code,
                    status.Status.Description);
                return status;
            }

            _logger.LogDebug(
                "KSeF: uwierzytelnianie {ReferenceNumber} w trakcie (próba {Attempt}/{MaxAttempts})",
                referenceNumber,
                attempt + 1,
                _options.MaxAuthenticationStatusPollAttempts);

            await Task.Delay(_options.AuthenticationStatusPollDelay, cancellationToken);
        }

        throw new TimeoutException($"KSeF: przekroczono czas oczekiwania na zakończenie uwierzytelniania {referenceNumber}.");
    }

    /// <inheritdoc />
    public async Task<TokenRedeemResponse> RedeemTokensAsync(
        string referenceNumber,
        string authenticationToken,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(referenceNumber);
        ArgumentException.ThrowIfNullOrWhiteSpace(authenticationToken);

        _logger.LogInformation("KSeF: wymiana tokenów dla {ReferenceNumber}", referenceNumber);
        var request = new TokenRedeemRequest
        {
            ReferenceNumber = referenceNumber,
            AuthenticationToken = authenticationToken,
        };

        var json = await _http.PostAsJsonAsync(TokenRedeemEndpoint, request, cancellationToken: cancellationToken);
        return KSeFJsonSerializer.Deserialize<TokenRedeemResponse>(json)
            ?? throw new InvalidOperationException("KSeF API zwróciło pustą odpowiedź wymiany tokenów.");
    }

    /// <inheritdoc />
    public async Task<TokenRefreshResponse> RefreshTokensAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(refreshToken);

        _logger.LogInformation("KSeF: odświeżanie tokenów");
        var request = new TokenRefreshRequest { RefreshToken = refreshToken };
        var json = await _http.PostAsJsonAsync(TokenRefreshEndpoint, request, cancellationToken: cancellationToken);
        return KSeFJsonSerializer.Deserialize<TokenRefreshResponse>(json)
            ?? throw new InvalidOperationException("KSeF API zwróciło pustą odpowiedź odświeżenia tokenów.");
    }

    /// <inheritdoc />
    public async Task<IPage<AuthenticationSession>> GetSessionsPageAsync(
        string? continuationToken = null,
        int? pageSize = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("KSeF: pobieranie strony aktywnych sesji");
        var qs = QueryBuilder.Build(
            ("pageSize", pageSize?.ToString()));

        var json = await _http.GetAsync(
            SessionsEndpoint,
            qs,
            continuationToken: continuationToken,
            cancellationToken: cancellationToken);

        var response = KSeFJsonSerializer.Deserialize<AuthenticationListResponse>(json)
            ?? throw new InvalidOperationException("KSeF API zwróciło pustą odpowiedź listy sesji.");

        return new Page<AuthenticationSession>(response.Items, response.ContinuationToken);
    }

    /// <inheritdoc />
    public IAsyncEnumerable<AuthenticationSession> GetAllSessionsAsync(
        int? pageSize = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("KSeF: iteracja przez wszystkie aktywne sesje");
        return KSeFContinuationEnumerable.IterateAll(
            (token, ct) => GetSessionsPageAsync(token, pageSize, ct),
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task TerminateCurrentSessionAsync(
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(accessToken);

        _logger.LogInformation("KSeF: unieważnianie aktualnej sesji");
        await _http.DeleteAsync(CurrentSessionEndpoint, bearerToken: accessToken, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public async Task TerminateSessionAsync(
        string referenceNumber,
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(referenceNumber);
        ArgumentException.ThrowIfNullOrWhiteSpace(accessToken);

        _logger.LogInformation("KSeF: unieważnianie sesji {ReferenceNumber}", referenceNumber);
        await _http.DeleteAsync($"{SessionsEndpoint}/{referenceNumber}", bearerToken: accessToken, cancellationToken: cancellationToken);
    }
}
