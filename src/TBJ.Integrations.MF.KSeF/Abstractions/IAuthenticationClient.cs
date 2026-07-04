using TBJ.Integrations.MF.KSeF.Models.Authentication;
using TBJ.Integrations.MF.KSeF.Pagination;

namespace TBJ.Integrations.MF.KSeF.Abstractions;

/// <summary>
/// Klient operacji uwierzytelniania i zarządzania sesjami w KSeF API v2.
/// </summary>
public interface IAuthenticationClient
{
    /// <summary>
    /// Pobiera challenge wymagany do operacji uwierzytelniania.
    /// </summary>
    Task<AuthenticationChallengeResponse> GetChallengeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rozpoczyna operację uwierzytelniania za pomocą podpisanego dokumentu XML (XAdES).
    /// </summary>
    /// <param name="signedAuthTokenRequest">Dokument XML AuthTokenRequest podpisany podpisem XAdES.</param>
    /// <param name="verifyCertificateChain">Wymuszenie weryfikacji łańcucha certyfikatu.</param>
    Task<AuthenticationTokenResponse> AuthenticateWithXadesAsync(
        string signedAuthTokenRequest,
        bool? verifyCertificateChain = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Rozpoczyna operację uwierzytelniania za pomocą tokena KSeF.
    /// </summary>
    Task<AuthenticationTokenResponse> AuthenticateWithTokenAsync(
        KsefTokenRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sprawdza status operacji uwierzytelniania.
    /// </summary>
    /// <param name="referenceNumber">Numer referencyjny operacji uwierzytelniania.</param>
    /// <param name="authenticationToken">Tymczasowy token uwierzytelniania.</param>
    Task<AuthenticationStatusResponse> GetStatusAsync(
        string referenceNumber,
        string authenticationToken,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Oczekuje na zakończenie operacji uwierzytelniania, odpytując status
    /// przez skonfigurowany czas i liczbę prób.
    /// </summary>
    Task<AuthenticationStatusResponse> WaitForAuthenticationCompletedAsync(
        string referenceNumber,
        string authenticationToken,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Wymienia authenticationToken na tokeny dostępowe (accessToken i refreshToken).
    /// </summary>
    Task<TokenRedeemResponse> RedeemTokensAsync(
        string referenceNumber,
        string authenticationToken,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Odświeża accessToken przy użyciu refreshToken.
    /// </summary>
    Task<TokenRefreshResponse> RefreshTokensAsync(
        string refreshToken,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Pobiera jedną stronę aktywnych sesji uwierzytelnienia.
    /// </summary>
    /// <param name="continuationToken">Opcjonalny token kontynuacji.</param>
    /// <param name="pageSize">Rozmiar strony (domyślnie 10, maksymalnie 100).</param>
    Task<IPage<AuthenticationSession>> GetSessionsPageAsync(
        string? continuationToken = null,
        int? pageSize = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Iteruje przez wszystkie aktywne sesje uwierzytelnienia.
    /// </summary>
    IAsyncEnumerable<AuthenticationSession> GetAllSessionsAsync(
        int? pageSize = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Unieważnia aktualną sesję uwierzytelnienia (powiązaną z użytym tokenem).
    /// </summary>
    /// <param name="accessToken">Token dostępowy lub refreshToken.</param>
    Task TerminateCurrentSessionAsync(
        string accessToken,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Unieważnia wskazaną sesję uwierzytelnienia.
    /// </summary>
    /// <param name="referenceNumber">Numer referencyjny sesji.</param>
    /// <param name="accessToken">Token dostępowy.</param>
    Task TerminateSessionAsync(
        string referenceNumber,
        string accessToken,
        CancellationToken cancellationToken = default);
}
