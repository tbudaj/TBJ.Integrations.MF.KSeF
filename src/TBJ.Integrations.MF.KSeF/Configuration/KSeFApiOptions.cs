namespace TBJ.Integrations.MF.KSeF.Configuration;

/// <summary>
/// Opcje konfiguracyjne biblioteki klienta KSeF API v2.
/// <para>
/// Dane uwierzytelniające (access token) mogą być dostarczane na dwa sposoby:
/// <list type="bullet">
/// <item><description>
/// <b>Per-żądanie (Scenariusz A, konto tenanta):</b> przez parametr <c>accessToken</c>
/// w każdej metodzie klienta. Pobierany z kontekstu tenanta (np. bazy danych).
/// </description></item>
/// <item><description>
/// <b>Z konfiguracji (Scenariusz B, nasze konto):</b> przez pole <see cref="DefaultAccessToken"/> —
/// używany automatycznie gdy parametr <c>accessToken</c> nie jest podany (null).
/// </description></item>
/// </list>
/// </para>
/// </summary>
public sealed class KSeFApiOptions
{
    /// <summary>
    /// Nazwa sekcji konfiguracyjnej w <c>appsettings.json</c>.
    /// </summary>
    public const string SectionName = "KSeFApi";

    /// <summary>
    /// Bazowy adres URL KSeF API v2.
    /// Domyślnie: <c>https://api-test.ksef.mf.gov.pl/v2</c> (środowisko testowe).
    /// </summary>
    public string BaseUrl { get; set; } = "https://api-test.ksef.mf.gov.pl/v2";

    /// <summary>
    /// Timeout dla żądań HTTP do KSeF API.
    /// Domyślnie: 60 sekund.
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(60);

    /// <summary>
    /// Określa, czy domyślnie żądać odpowiedzi błędów w formacie Problem Details
    /// (<c>application/problem+json</c>) poprzez nagłówek <c>X-Error-Format: problem-details</c>.
    /// Domyślnie: <c>true</c>.
    /// </summary>
    public bool UseProblemDetailsErrorFormat { get; set; } = true;

    /// <summary>
    /// Maksymalna liczba prób odpytania statusu asynchronicznej operacji uwierzytelniania.
    /// Domyślnie: 60.
    /// </summary>
    public int MaxAuthenticationStatusPollAttempts { get; set; } = 60;

    /// <summary>
    /// Odstęp między kolejnymi próbami odpytania statusu uwierzytelniania.
    /// Domyślnie: 1 sekunda.
    /// </summary>
    public TimeSpan AuthenticationStatusPollDelay { get; set; } = TimeSpan.FromSeconds(1);

    // ── Scenariusz B: domyślny access token naszego konta (opcjonalny) ──────────────

    /// <summary>
    /// Domyślny access token naszego konta KSeF (Scenariusz B).
    /// Używany gdy parametr <c>accessToken</c> nie jest podany w metodzie klienta.
    /// <para>
    /// Uwaga: token KSeF ma ograniczony czas życia. W środowiskach produkcyjnych
    /// rozważ dynamiczne odświeżanie tokenu zamiast statycznej wartości w konfiguracji.
    /// </para>
    /// </summary>
    public string? DefaultAccessToken { get; set; }
}
