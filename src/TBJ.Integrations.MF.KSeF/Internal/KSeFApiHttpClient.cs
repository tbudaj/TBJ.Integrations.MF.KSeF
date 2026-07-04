using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using TBJ.Integrations.MF.KSeF.Configuration;
using TBJ.Integrations.MF.KSeF.Models.Common;
using Microsoft.Extensions.Logging;

namespace TBJ.Integrations.MF.KSeF.Internal;

/// <summary>
/// Wewnętrzny wrapper nad <see cref="HttpClient"/> wyspecjalizowany do komunikacji
/// z KSeF API v2. Obsługuje wspólną logikę HTTP, nagłówki, błędy i logowanie.
/// </summary>
internal sealed class KSeFApiHttpClient
{
    private readonly HttpClient _http;
    private readonly KSeFApiOptions _options;
    private readonly ILogger<KSeFApiHttpClient> _logger;

    public KSeFApiHttpClient(HttpClient http, KSeFApiOptions options, ILogger<KSeFApiHttpClient> logger)
    {
        _http = http;
        _options = options;
        _logger = logger;
    }

    /// <summary>
    /// Wysyła żądanie GET pod wskazany endpoint i zwraca strumień odpowiedzi.
    /// Używane do pobierania plików (faktury, eksporty).
    /// </summary>
    /// <param name="endpoint">Ścieżka zasobu.</param>
    /// <param name="queryString">Query string.</param>
    /// <param name="bearerToken">Opcjonalny token Bearer.</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    /// <returns>Strumień treści odpowiedzi HTTP.</returns>
    /// <exception cref="KSeFApiException">Gdy API zwróci błąd.</exception>
    public async Task<Stream> GetStreamAsync(
        string endpoint,
        string? queryString = null,
        string? bearerToken = null,
        CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(endpoint, queryString);
        _logger.LogDebug("KSeF API: GET stream {Url}", url);

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        AddHeaders(request, bearerToken);

        var response = await SendRequestAsync(request, url, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            ThrowApiError(response, body, url);
        }

        return await response.Content.ReadAsStreamAsync(cancellationToken);
    }

    /// <summary>
    /// Wysyła żądanie GET pod wskazany endpoint i zwraca treść odpowiedzi jako string.
    /// </summary>
    /// <param name="endpoint">Ścieżka zasobu (np. <c>auth/sessions</c>).</param>
    /// <param name="queryString">Query string (np. <c>?pageSize=10</c>).</param>
    /// <param name="bearerToken">Opcjonalny token Bearer (accessToken lub authenticationToken).</param>
    /// <param name="continuationToken">Opcjonalny token kontynuacji paginacji.</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    /// <returns>Treść odpowiedzi HTTP jako string.</returns>
    /// <exception cref="KSeFApiException">Gdy API zwróci błąd.</exception>
    public async Task<string> GetAsync(
        string endpoint,
        string? queryString = null,
        string? bearerToken = null,
        string? continuationToken = null,
        CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(endpoint, queryString);
        _logger.LogDebug("KSeF API: GET {Url}", url);

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        AddHeaders(request, bearerToken, continuationToken);

        var response = await SendRequestAsync(request, url, cancellationToken);
        return await ReadSuccessBodyAsync(response, url, cancellationToken);
    }

    /// <summary>
    /// Wysyła żądanie POST z ciałem JSON i zwraca treść odpowiedzi jako string.
    /// </summary>
    public async Task<string> PostAsJsonAsync<TRequest>(
        string endpoint,
        TRequest body,
        string? bearerToken = null,
        CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(endpoint, null);
        var json = KSeFJsonSerializer.Serialize(body);
        _logger.LogDebug("KSeF API: POST {Url} — Body: {Body}", url, json);

        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        AddHeaders(request, bearerToken);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await SendRequestAsync(request, url, cancellationToken);
        return await ReadSuccessBodyAsync(response, url, cancellationToken);
    }

    /// <summary>
    /// Wysyła żądanie POST z ciałem XML i zwraca treść odpowiedzi jako string.
    /// </summary>
    public async Task<string> PostAsXmlAsync(
        string endpoint,
        string xml,
        string? bearerToken = null,
        CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(endpoint, null);
        _logger.LogDebug("KSeF API: POST {Url} — XML body length: {Length}", url, xml.Length);

        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        AddHeaders(request, bearerToken);
        request.Content = new StringContent(xml, Encoding.UTF8, "application/xml");

        var response = await SendRequestAsync(request, url, cancellationToken);
        return await ReadSuccessBodyAsync(response, url, cancellationToken);
    }

    /// <summary>
    /// Wysyła żądanie POST z ciałem multipart/form-data.
    /// </summary>
    public async Task<string> PostMultipartAsync(
        string endpoint,
        MultipartFormDataContent content,
        string? bearerToken = null,
        CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(endpoint, null);
        _logger.LogDebug("KSeF API: POST {Url} — multipart/form-data", url);

        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        AddHeaders(request, bearerToken);
        request.Content = content;

        var response = await SendRequestAsync(request, url, cancellationToken);
        return await ReadSuccessBodyAsync(response, url, cancellationToken);
    }

    /// <summary>
    /// Wysyła żądanie DELETE pod wskazany endpoint.
    /// </summary>
    public async Task DeleteAsync(
        string endpoint,
        string? bearerToken = null,
        CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(endpoint, null);
        _logger.LogDebug("KSeF API: DELETE {Url}", url);

        using var request = new HttpRequestMessage(HttpMethod.Delete, url);
        AddHeaders(request, bearerToken);

        var response = await SendRequestAsync(request, url, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
        {
            _logger.LogInformation("KSeF API: DELETE {Url} — HTTP {StatusCode}", url, (int)response.StatusCode);
            return;
        }

        await ReadSuccessBodyAsync(response, url, cancellationToken);
    }

    private async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage request, string url, CancellationToken cancellationToken)
    {
        HttpResponseMessage response;
        try
        {
            response = await _http.SendAsync(request, cancellationToken);
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException || !cancellationToken.IsCancellationRequested)
        {
            throw new KSeFApiException($"Przekroczono limit czasu oczekiwania na odpowiedź KSeF API dla {url}.", ex);
        }
        catch (HttpRequestException ex)
        {
            throw new KSeFApiException($"Błąd komunikacji z KSeF API dla {url}: {ex.Message}", ex);
        }

        _logger.LogInformation("KSeF API: {Method} {Url} — HTTP {StatusCode}", request.Method, url, (int)response.StatusCode);
        return response;
    }

    private async Task<string> ReadSuccessBodyAsync(HttpResponseMessage response, string url, CancellationToken cancellationToken)
    {
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        _logger.LogDebug("KSeF API: response body for {Url}: {Body}", url, body);

        if (!response.IsSuccessStatusCode)
        {
            ThrowApiError(response, body, url);
        }

        return body;
    }

    private void ThrowApiError(HttpResponseMessage response, string body, string url)
    {
        var statusCode = (int)response.StatusCode;

        if (!string.IsNullOrWhiteSpace(body))
        {
            try
            {
                var problem = KSeFJsonSerializer.Deserialize<ProblemDetails>(body);
                if (problem is { Status: not null } or { Code: not null })
                {
                    throw new KSeFApiException(
                        $"KSeF API zwróciło błąd HTTP {statusCode} dla {url}: {problem.Detail ?? problem.Title}",
                        statusCode,
                        problem.Code,
                        problem.Detail ?? problem.Title,
                        body);
                }
            }
            catch (System.Text.Json.JsonException)
            {
                // Ignorujemy błąd parsowania — spróbujemy ExceptionResponse.
            }

            try
            {
                var exception = KSeFJsonSerializer.Deserialize<ExceptionResponse>(body);
                if (exception is { ExceptionCode: not null })
                {
                    throw new KSeFApiException(
                        $"KSeF API zwróciło błąd HTTP {statusCode} dla {url}: {exception.ExceptionDescription}",
                        statusCode,
                        exception.ExceptionCode,
                        exception.ExceptionDescription,
                        exception.Details);
                }
            }
            catch (System.Text.Json.JsonException)
            {
                // Ignorujemy — zgłosimy ogólny wyjątek.
            }
        }

        throw new KSeFApiException(
            $"KSeF API zwróciło błąd HTTP {statusCode} dla {url}: {body}",
            statusCode,
            details: body);
    }

    private void AddHeaders(HttpRequestMessage request, string? bearerToken, string? continuationToken = null)
    {
        request.Headers.Add("Accept", "application/json");

        if (_options.UseProblemDetailsErrorFormat)
            request.Headers.Add("X-Error-Format", "problem-details");

        if (!string.IsNullOrWhiteSpace(bearerToken))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

        if (!string.IsNullOrWhiteSpace(continuationToken))
            request.Headers.Add("x-continuation-token", continuationToken);
    }

    private string BuildUrl(string endpoint, string? queryString)
    {
        return endpoint + (queryString ?? string.Empty);
    }
}
