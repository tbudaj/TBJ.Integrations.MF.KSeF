# TBJ.Integrations.MF.KSeF

Typowany klient .NET dla KSeF API v2 (Krajowy System e-Faktur, Ministerstwo Finansów).

## Instalacja

```bash
dotnet add package TBJ.Integrations.MF.KSeF
```

## Rejestracja w DI

```csharp
builder.Services.AddKSeFApi(builder.Configuration);
```

```json
{
  "KSeFApi": {
    "BaseUrl": "https://api-test.ksef.mf.gov.pl/v2",
    "Timeout": "00:01:00",
    "UseProblemDetailsErrorFormat": true,
    "MaxAuthenticationStatusPollAttempts": 60,
    "AuthenticationStatusPollDelay": "00:00:01"
  }
}
```

## Uwierzytelnianie

Biblioteka obsługuje dwa scenariusze dostarczania access tokenu:

- **Scenariusz A — konto tenanta:** token przekazywany per-żądanie jako parametr metody.
- **Scenariusz B — nasze konto:** `DefaultAccessToken` z `KSeFApiOptions` używany gdy parametr jest `null`.

```csharp
var session = await sessions.OpenOnlineSessionAsync(request, accessToken: tenantToken);
```

## Klienci

| Klient | Odpowiedzialność |
|---|---|
| `IAuthenticationClient` | Challenge, uwierzytelnianie XAdES/token, wymiana tokenów, sesje auth |
| `ISessionsClient` | Sesje online/batch, wysyłka faktur, UPO, statusy |
| `IInvoicesClient` | Wyszukiwanie, pobieranie, status, eksport faktur |

## Przykład wysyłki faktury

```csharp
var session = await sessions.OpenOnlineSessionAsync(
    new OpenSessionRequest
    {
        ContextIdentifier = new ContextIdentifier
        {
            Type = ContextIdentifierType.Nip,
            Identifier = "1234567890"
        }
    },
    accessToken);

try
{
    await using var stream = File.OpenRead("faktura.xml");
    var result = await sessions.SendInvoiceAsync(
        session.SessionIdentifier.SessionIdentifierValue,
        stream,
        new InvoiceMetadata { Format = "FA_V2" },
        accessToken);

    var upo = await sessions.GetSessionUpoAsync(
        session.SessionIdentifier.SessionIdentifierValue,
        accessToken);
}
finally
{
    await sessions.CloseOnlineSessionAsync(
        session.SessionIdentifier.SessionIdentifierValue,
        accessToken);
}
```

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

## Cykl życia serwisów

| Serwis | Lifetime | Uzasadnienie |
|---|---|---|
| `IAuthenticationClient` | `Scoped` | Per-request |
| `ISessionsClient` | `Scoped` | Per-request |
| `IInvoicesClient` | `Scoped` | Per-request |
| `HttpClient` | przez `IHttpClientFactory` | Automatyczna rotacja handlerów |
