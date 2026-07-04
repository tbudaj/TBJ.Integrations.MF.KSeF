# TBJ.Integrations.MF.KSeF

[![build](https://github.com/tbudaj/TBJ.Integrations.MF.KSeF/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/tbudaj/TBJ.Integrations.MF.KSeF/actions/workflows/build-and-test.yml)
[![NuGet](https://img.shields.io/nuget/v/TBJ.Integrations.MF.KSeF)](https://www.nuget.org/packages/TBJ.Integrations.MF.KSeF)

Biblioteka klienta .NET 8, 9, 10 dla **KSeF API v2** (Krajowy System e-Faktur, Ministerstwo Finansów).

Projekt jest wzorowany na `TBJ.Integrations.NFZ.UmwApi` i dostarcza typowanych abstrakcji do komunikacji z KSeF: uwierzytelnianie, sesje wystawiania faktur (online i batch), wyszukiwanie/pobieranie faktur oraz eksporty.

---

## Spis treści

- [Wymagania](#wymagania)
- [Rejestracja w DI](#rejestracja-w-di)
- [Zakres funkcjonalności](#zakres-funkcjonalności)
- [Uwierzytelnianie i tokeny](#uwierzytelnianie-i-tokeny)
  - [Jak uzyskać token dostępowy](#jak-uzyskać-token-dostępowy)
  - [Tokeny są per-tenant](#tokeny-są-per-tenant)
- [Przykłady użycia](#przykłady-użycia)
  - [AuthenticationClient](#authenticationclient)
  - [SessionsClient](#sessionsclient)
  - [InvoicesClient](#invoicesclient)
- [Paginacja](#paginacja)
- [Obsługa błędów](#obsługa-błędów)
- [Konfiguracja](#konfiguracja)
- [Znane ograniczenia i kolejne kroki](#znane-ograniczenia-i-kolejne-kroki)

---

## Wymagania

- .NET 10
- Konto/podmiot zarejestrowany w KSeF (TEST / DEMO / PROD)
- Token KSeF lub kwalifikowany podpis elektroniczny (XAdES) do uwierzytelniania

---

## Rejestracja w DI

Biblioteka rejestruje tylko **infrastrukturę HTTP i klienty**.

```csharp
builder.Services.AddKSeFApi(opt =>
{
    opt.BaseUrl = "https://api-test.ksef.mf.gov.pl/v2";
    opt.Timeout = TimeSpan.FromSeconds(60);
});
```

lub z `appsettings.json`:

```csharp
builder.Services.AddKSeFApi(builder.Configuration);
```

```json
{
  "KSeFApi": {
    "BaseUrl": "https://api-test.ksef.mf.gov.pl/v2",
    "Timeout": "00:01:00",
    "UseProblemDetailsErrorFormat": true
  }
}
```

---

## Model uwierzytelniania (wielotenantowość)

Biblioteka obsługuje dwa scenariusze dostarczania access tokenu:

### Scenariusz A — konto tenanta (token per-request)

Token dostępowy (`accessToken`) pobierany z kontekstu tenanta i przekazywany jako parametr do każdej metody. To preferowany tryb dla aplikacji wielotenantowych.

```csharp
// Token pobrany z bazy danych tenanta
string accessToken = await tokenStore.GetTokenAsync(tenantId);

var sessions = serviceProvider.GetRequiredService<ISessionsClient>();
var session = await sessions.OpenOnlineSessionAsync(request, accessToken: accessToken);
```

### Scenariusz B — nasze konto (domyślny token z konfiguracji)

Jeśli `accessToken` nie jest przekazany (parametr `null`), klienty automatycznie użyją `DefaultAccessToken` z `KSeFApiOptions`.

```csharp
// appsettings.json
{
  "KSeFApi": {
    "BaseUrl": "https://api.ksef.mf.gov.pl/v2",
    "DefaultAccessToken": "eyJ..."  // token naszego konta
  }
}

// Rejestracja
builder.Services.AddKSeFApi(builder.Configuration);

// Wywołanie — accessToken=null → używany DefaultAccessToken z opcji
var sessions = serviceProvider.GetRequiredService<ISessionsClient>();
var session = await sessions.OpenOnlineSessionAsync(request);  // bez accessToken
```

> **Uwaga:** Token KSeF ma ograniczony czas życia. W środowiskach produkcyjnych rozważ dynamiczne odświeżanie tokenu (`RefreshTokensAsync`) zamiast statycznej wartości w konfiguracji.

---

## Zakres funkcjonalności

| Obszar | Funkcje |
|--------|---------|
| **Uwierzytelnianie** | `GetChallengeAsync`, `AuthenticateWithXadesAsync`, `AuthenticateWithTokenAsync`, `WaitForAuthenticationCompletedAsync`, `RedeemTokensAsync`, `RefreshTokensAsync`, zarządzanie sesjami auth |
| **Sesje online** | otwieranie, zamykanie, wysyłka pojedynczych faktur, status, UPO, lista faktur, faktury odrzucone |
| **Sesje batch** | otwieranie sesji wsadowej, zamykanie, wysyłka paczki faktur |
| **Faktury** | wyszukiwanie, pobieranie szczegółów, status, UPO, pobieranie pliku, eksport |

---

## Uwierzytelnianie i tokeny

### Jak uzyskać token dostępowy

KSeF API używa dwuetapowego uwierzytelniania:

1. **Rozpoczęcie uwierzytelniania** — metodą XAdES lub tokenem KSeF:

```csharp
var authClient = serviceProvider.GetRequiredService<IAuthenticationClient>();

// Metoda 1: XAdES
// Krok A: pobierz challenge (ważny ~10 min)
var challenge = await authClient.GetChallengeAsync();

// Krok B: w aplikacji nadrzędnej zbuduj i podpisz dokument AuthTokenRequest,
//         w którym challenge jest wpisany wewnątrz XML
string signedXml = BuildAndSignAuthTokenRequest(
    challenge.Challenge,
    contextIdentifier: new ContextIdentifier { Type = ContextIdentifierType.Nip, Identifier = "1234567890" },
    subjectIdentifierType: SubjectIdentifierType.CertificateSubject);

// Krok C: wyślij podpisany dokument do KSeF
var initResponse = await authClient.AuthenticateWithXadesAsync(signedXml);

// Metoda 2: Token KSeF (zaszyfrowany kluczem publicznym MF)
var initResponse = await authClient.AuthenticateWithTokenAsync(new KsefTokenRequest
{
    Token = encryptedToken,      // zaszyfrowany RSA-OAEP token z MCU
    Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
    ContextIdentifier = new ContextIdentifier { Type = ContextIdentifierType.Nip, Identifier = "1234567890" },
    SubjectIdentifierType = SubjectIdentifierType.CertificateSubject,
});
```

Po inicjalizacji otrzymujesz `authenticationToken` oraz `referenceNumber`.

> **Uwaga:** `BuildAndSignAuthTokenRequest` w przykładzie powyżej to funkcja po stronie aplikacji nadrzędnej. Obecna wersja biblioteki przyjmuje gotowy podpisany XML; w kolejnej iteracji planowany jest wbudowany helper do budowy dokumentu `AuthTokenRequest` zgodnie ze schematem KSeF.

2. **Polling statusu** — operacja jest asynchroniczna:

```csharp
var status = await authClient.WaitForAuthenticationCompletedAsync(
    initResponse.ReferenceNumber,
    initResponse.AuthenticationToken);
```

3. **Wymiana na tokeny dostępowe**:

```csharp
var tokens = await authClient.RedeemTokensAsync(
    initResponse.ReferenceNumber,
    initResponse.AuthenticationToken);

string accessToken = tokens.AccessToken;
string refreshToken = tokens.RefreshToken;
```

4. **Odświeżanie tokena** (przed wygaśnięciem):

```csharp
var refreshed = await authClient.RefreshTokensAsync(refreshToken);
accessToken = refreshed.AccessToken;
```

### Tokeny są per-tenant

**`accessToken` i `refreshToken` NIE są przechowywane w tej bibliotece.** Są tożsamością konkretnego podmiotu (tenanta) w KSeF. Aplikacja nadrzędna powinna:

- przechowywać tokeny w konfiguracji/bazie danych powiązanej z tenantem,
- pobrać odpowiedni token dla aktualnego kontekstu,
- przekazać go jako parametr metodom klientów KSeF.

---

## Kompletny przykład wysyłki faktury z aplikacji nadrzędnej

Poniżej znajduje się przykładowa usługa w aplikacji nadrzędnej, która:

1. Pobiera token KSeF i NIP dla aktualnego tenanta z bazy danych.
2. Otwiera sesję online.
3. Wysyła pojedynczą fakturę (plik XML).
4. Pobiera UPO dla sesji.
5. Zamyka sesję w bloku `finally`.

```csharp
public class KSeFInvoiceService
{
    private readonly ISessionsClient _sessions;
    private readonly IAuthenticationClient _auth;
    private readonly ITenantKSeFTokenStore _tokenStore;
    private readonly ILogger<KSeFInvoiceService> _logger;

    public KSeFInvoiceService(
        ISessionsClient sessions,
        IAuthenticationClient auth,
        ITenantKSeFTokenStore tokenStore,
        ILogger<KSeFInvoiceService> logger)
    {
        _sessions = sessions;
        _auth = auth;
        _tokenStore = tokenStore;
        _logger = logger;
    }

    /// <summary>
    /// Wysyła fakturę do KSeF w imieniu wskazanego tenanta.
    /// </summary>
    public async Task<SendInvoiceResponse> SendInvoiceAsync(
        int tenantId,
        Stream invoiceXml,
        CancellationToken ct = default)
    {
        // 1. Pobierz konfigurację KSeF dla tenanta z bazy.
        var tenantConfig = await _tokenStore.GetTenantConfigAsync(tenantId, ct);
        var accessToken = tenantConfig.AccessToken;
        var refreshToken = tenantConfig.RefreshToken;
        var nip = tenantConfig.Nip;

        // 2. Spróbuj odświeżyć token, jeśli jest bliski wygaśnięcia.
        if (tenantConfig.AccessTokenExpiresAt < DateTimeOffset.UtcNow.AddMinutes(5))
        {
            _logger.LogInformation("Odświeżanie tokena KSeF dla tenant {TenantId}", tenantId);
            var refreshed = await _auth.RefreshTokensAsync(refreshToken, ct);
            accessToken = refreshed.AccessToken;
            refreshToken = refreshed.RefreshToken;

            await _tokenStore.SaveTokensAsync(
                tenantId,
                accessToken,
                refreshToken,
                DateTimeOffset.UtcNow.AddSeconds(refreshed.ExpiresIn),
                ct);
        }

        // 3. Otwórz sesję online.
        var session = await _sessions.OpenOnlineSessionAsync(
            new OpenSessionRequest
            {
                ContextIdentifier = new ContextIdentifier
                {
                    Type = ContextIdentifierType.Nip,
                    Identifier = nip,
                },
            },
            accessToken,
            ct);

        var sessionId = session.SessionIdentifier.SessionIdentifierValue;
        _logger.LogInformation(
            "Otwarto sesję KSeF {SessionId} dla tenant {TenantId}",
            sessionId,
            tenantId);

        try
        {
            // 4. Wyślij fakturę.
            var result = await _sessions.SendInvoiceAsync(
                sessionId,
                invoiceXml,
                new InvoiceMetadata { Format = "FA_V2" },
                accessToken,
                ct);

            _logger.LogInformation(
                "Wysłano fakturę {KsefNumber} w sesji {SessionId}",
                result.KsefNumber,
                sessionId);

            // 5. Pobierz UPO dla sesji.
            var upo = await _sessions.GetSessionUpoAsync(sessionId, accessToken, ct);
            _logger.LogInformation(
                "UPO dla sesji {SessionId}: {Upo}",
                sessionId,
                upo.Upo);

            return result;
        }
        catch (KSeFApiException ex)
        {
            _logger.LogError(
                ex,
                "Błąd KSeF podczas wysyłki faktury dla tenant {TenantId}: HTTP {StatusCode}, kod {KSeFCode}",
                tenantId,
                ex.StatusCode,
                ex.KsefExceptionCode);
            throw;
        }
        finally
        {
            // 6. Zamknij sesję niezależnie od wyniku.
            try
            {
                await _sessions.CloseOnlineSessionAsync(sessionId, accessToken, ct);
                _logger.LogInformation("Zamknięto sesję {SessionId}", sessionId);
            }
            catch (Exception closeEx)
            {
                _logger.LogWarning(closeEx, "Nie udało się zamknąć sesji {SessionId}", sessionId);
            }
        }
    }
}
```

### Przykładowy interfejs repozytorium tenanta

```csharp
public interface ITenantKSeFTokenStore
{
    Task<TenantKSeFConfig> GetTenantConfigAsync(int tenantId, CancellationToken ct);
    Task SaveTokensAsync(
        int tenantId,
        string accessToken,
        string refreshToken,
        DateTimeOffset accessTokenExpiresAt,
        CancellationToken ct);
}

public class TenantKSeFConfig
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string Nip { get; set; } = string.Empty;
    public DateTimeOffset AccessTokenExpiresAt { get; set; }
}
```

### Wywołanie z kontrolera

```csharp
[ApiController]
[Route("api/[controller]")]
public class InvoicesController : ControllerBase
{
    private readonly KSeFInvoiceService _ksefService;

    [HttpPost("send")]
    public async Task<IActionResult> SendInvoice(IFormFile invoice, CancellationToken ct)
    {
        // tenantId pochodzi z kontekstu użytkownika / nagłówka
        int tenantId = GetCurrentTenantId();

        await using var stream = invoice.OpenReadStream();
        var result = await _ksefService.SendInvoiceAsync(tenantId, stream, ct);

        return Ok(new { result.KsefNumber, result.ReferenceNumber });
    }
}
```

---

## Przykłady użycia

### AuthenticationClient

```csharp
var authClient = serviceProvider.GetRequiredService<IAuthenticationClient>();

// Pobierz listę aktywnych sesji
await foreach (var session in authClient.GetAllSessionsAsync(pageSize: 25))
{
    Console.WriteLine(session.ReferenceNumber);
}

// Unieważnij aktualną sesję
await authClient.TerminateCurrentSessionAsync(accessToken);
```

### SessionsClient

```csharp
var sessions = serviceProvider.GetRequiredService<ISessionsClient>();

// Sesja online
var openResponse = await sessions.OpenOnlineSessionAsync(
    new OpenSessionRequest { /* ... */ },
    accessToken);

await sessions.SendInvoiceAsync(
    openResponse.SessionIdentifier.SessionIdentifierValue,
    invoiceStream,
    new InvoiceMetadata { Format = "FA_V2" },
    accessToken);

var upo = await sessions.GetSessionUpoAsync(
    openResponse.SessionIdentifier.SessionIdentifierValue,
    accessToken);

await sessions.CloseOnlineSessionAsync(
    openResponse.SessionIdentifier.SessionIdentifierValue,
    accessToken);

// Sesja wsadowa
var batch = await sessions.OpenBatchSessionAsync(
    new OpenBatchSessionRequest
    {
        ContextIdentifier = new ContextIdentifier { /* ... */ },
        Invoices = new[]
        {
            new BatchInvoice
            {
                Invoice = encryptedInvoiceBase64,
                Metadata = new InvoiceMetadata { Format = "FA_V2" },
            },
        },
    },
    accessToken);

await sessions.CloseBatchSessionAsync(
    batch.SessionIdentifier.SessionIdentifierValue,
    accessToken);
```

### InvoicesClient

```csharp
var invoices = serviceProvider.GetRequiredService<IInvoicesClient>();

// Wyszukaj faktury
var page = await invoices.QueryInvoicesAsync(
    dateFrom: DateTimeOffset.UtcNow.AddDays(-30),
    dateTo: DateTimeOffset.UtcNow,
    pageSize: 25,
    accessToken: accessToken);

// Iteruj przez wszystkie strony
await foreach (var invoice in invoices.GetAllInvoicesAsync(
    dateFrom: DateTimeOffset.UtcNow.AddDays(-30),
    accessToken: accessToken))
{
    Console.WriteLine(invoice.KsefNumber);
}

// Pobierz plik faktury
await using var stream = await invoices.DownloadInvoiceAsync(ksefNumber, accessToken);
await stream.CopyToAsync(File.Create("faktura.xml"));

// Eksport
var export = await invoices.InitiateExportAsync(
    new ExportRequest
    {
        Criteria = new ExportCriteria { DateFrom = DateTimeOffset.UtcNow.AddDays(-30) },
        Format = ExportFormat.Json,
    },
    accessToken);

// Pobierz wynik eksportu
await using var exportStream = await invoices.DownloadExportAsync(export.ExportId, accessToken);
```

---

## Paginacja

KSeF API nie używa numerów stron. Lista wyników jest paginowana przez **token kontynuacji** (`continuationToken`):

```csharp
var page = await invoices.QueryInvoicesAsync(pageSize: 25, accessToken: accessToken);

while (page.HasNextPage)
{
    page = await invoices.QueryInvoicesAsync(
        pageSize: 25,
        continuationToken: page.ContinuationToken,
        accessToken: accessToken);
}
```

Wygodniej jest użyć `IAsyncEnumerable<T>`:

```csharp
await foreach (var invoice in invoices.GetAllInvoicesAsync(accessToken: accessToken))
{
    // ...
}
```

---

## Obsługa błędów

Błędy KSeF są reprezentowane jako `KSeFApiException`:

```csharp
try
{
    await invoices.GetInvoiceAsync(ksefNumber, accessToken);
}
catch (KSeFApiException ex)
{
    Console.WriteLine($"HTTP {ex.StatusCode}, KSeF code {ex.KsefExceptionCode}: {ex.Description}");
}
```

Biblioteka domyślnie wysyła nagłówek `X-Error-Format: problem-details`, więc błędy są parsowane z formatu RFC 7807. Jeśli API zwróci starszy format `ExceptionResponse`, zostanie on również poprawnie odczytany.

---

## Konfiguracja

| Właściwość | Domyślnie | Opis |
|------------|-----------|------|
| `BaseUrl` | `https://api-test.ksef.mf.gov.pl/v2` | Bazowy adres KSeF API |
| `Timeout` | 60 s | Timeout żądań HTTP |
| `UseProblemDetailsErrorFormat` | `true` | Czy żądać błędów w formacie Problem Details |
| `MaxAuthenticationStatusPollAttempts` | 60 | Maksymalna liczba prób pollingu statusu auth |
| `AuthenticationStatusPollDelay` | 1 s | Odstęp między próbami pollingu |
| `DefaultAccessToken` | `null` | Domyślny token naszego konta (Scenariusz B). Gdy null, token musi być przekazany per-żądanie. |

---

## Znane ograniczenia i kolejne kroki

1. **XAdES** — biblioteka przyjmuje gotowy podpisany XML. Należy dodać osobny helper do budowy `AuthTokenRequest` i podpisywania XAdES.
2. **Szyfrowanie tokena KSeF** — token musi być zaszyfrowany kluczem publicznym MF (RSA-OAEP). Klucze należy pobrać z `/public-key-certificates`.
3. **Szyfrowanie faktur** — wysyłane pliki faktur muszą być zaszyfrowane kluczem sesji. Obecnie `SendInvoiceAsync` przyjmuje gotowy strumień.
4. **Modele** — część modeli jest uproszczona; po pierwszych rzeczywistych wywołaniach warto zweryfikować je z dokumentacją KSeF.
