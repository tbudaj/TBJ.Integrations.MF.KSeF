using TBJ.Integrations.MF.KSeF.Abstractions;
using TBJ.Integrations.MF.KSeF.Clients;
using TBJ.Integrations.MF.KSeF.Configuration;
using TBJ.Integrations.MF.KSeF.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TBJ.Integrations.MF.KSeF;

/// <summary>
/// Rozszerzenia DI dla biblioteki klienta KSeF API v2.
/// </summary>
public static class KSeFApiExtensions
{
    /// <summary>
    /// Rejestruje klientów KSeF API (<see cref="IAuthenticationClient"/>,
    /// <see cref="ISessionsClient"/>, <see cref="IInvoicesClient"/>) w kontenerze DI.
    /// </summary>
    /// <param name="services">Kolekcja serwisów.</param>
    /// <param name="configure">Opcjonalna akcja konfiguracji (BaseUrl, Timeout, MaxAuthenticationStatusPollAttempts, DefaultAccessToken).</param>
    /// <returns>Kolekcja serwisów (fluent API).</returns>
    /// <example>
    /// <code>
    /// builder.Services.AddKSeFApi(opt =>
    /// {
    ///     opt.BaseUrl = "https://api-test.ksef.mf.gov.pl/v2";
    ///     opt.Timeout = TimeSpan.FromSeconds(60);
    /// });
    /// </code>
    /// </example>
    public static IServiceCollection AddKSeFApi(
        this IServiceCollection services,
        Action<KSeFApiOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        var options = new KSeFApiOptions();
        configure?.Invoke(options);

        ValidateOptions(options);

        services.AddSingleton(options);

        services.AddHttpClient<KSeFApiHttpClient>((sp, client) =>
        {
            var baseUrl = options.BaseUrl.TrimEnd('/') + "/";
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = options.Timeout;
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        services.AddScoped<IAuthenticationClient>(sp =>
            new AuthenticationClient(
                sp.GetRequiredService<KSeFApiHttpClient>(),
                sp.GetRequiredService<KSeFApiOptions>(),
                sp.GetRequiredService<ILogger<AuthenticationClient>>()));

        services.AddScoped<ISessionsClient>(sp =>
            new SessionsClient(
                sp.GetRequiredService<KSeFApiHttpClient>(),
                sp.GetRequiredService<KSeFApiOptions>(),
                sp.GetRequiredService<ILogger<SessionsClient>>()));

        services.AddScoped<IInvoicesClient>(sp =>
            new InvoicesClient(
                sp.GetRequiredService<KSeFApiHttpClient>(),
                sp.GetRequiredService<KSeFApiOptions>(),
                sp.GetRequiredService<ILogger<InvoicesClient>>()));

        return services;
    }

    /// <summary>
    /// Rejestruje klientów KSeF API z konfiguracją z sekcji <see cref="KSeFApiOptions.SectionName"/>
    /// w <c>IConfiguration</c>.
    /// </summary>
    /// <remarks>
    /// Wymaga obecności sekcji <c>KSeFApi</c> w konfiguracji aplikacji.
    /// </remarks>
    public static IServiceCollection AddKSeFApi(
        this IServiceCollection services,
        Microsoft.Extensions.Configuration.IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var section = configuration.GetSection(KSeFApiOptions.SectionName);
        return services.AddKSeFApi(opt =>
        {
            var baseUrl = section["BaseUrl"];
            if (!string.IsNullOrWhiteSpace(baseUrl))
                opt.BaseUrl = baseUrl;

            var timeout = section["Timeout"];
            if (!string.IsNullOrWhiteSpace(timeout) && TimeSpan.TryParse(timeout, out var ts))
                opt.Timeout = ts;

            var useProblemDetails = section["UseProblemDetailsErrorFormat"];
            if (!string.IsNullOrWhiteSpace(useProblemDetails) && bool.TryParse(useProblemDetails, out var problemDetails))
                opt.UseProblemDetailsErrorFormat = problemDetails;

            var maxAttempts = section["MaxAuthenticationStatusPollAttempts"];
            if (!string.IsNullOrWhiteSpace(maxAttempts) && int.TryParse(maxAttempts, out var attempts))
                opt.MaxAuthenticationStatusPollAttempts = attempts;

            var pollDelay = section["AuthenticationStatusPollDelay"];
            if (!string.IsNullOrWhiteSpace(pollDelay) && TimeSpan.TryParse(pollDelay, out var delay))
                opt.AuthenticationStatusPollDelay = delay;

            // Scenariusz B: domyślny access token naszego konta (opcjonalny)
            var defaultAccessToken = section[nameof(KSeFApiOptions.DefaultAccessToken)];
            if (!string.IsNullOrWhiteSpace(defaultAccessToken))
                opt.DefaultAccessToken = defaultAccessToken;
        });
    }

    private static void ValidateOptions(KSeFApiOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.BaseUrl))
            throw new InvalidOperationException($"Brak konfiguracji {nameof(KSeFApiOptions.BaseUrl)} dla KSeF API.");

        if (!Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out _))
            throw new InvalidOperationException($"Nieprawidłowy URL '{options.BaseUrl}' w konfiguracji {nameof(KSeFApiOptions.BaseUrl)}.");

        if (options.Timeout <= TimeSpan.Zero)
            throw new InvalidOperationException($"Wartość {nameof(KSeFApiOptions.Timeout)} musi być większa od zera.");

        if (options.MaxAuthenticationStatusPollAttempts <= 0)
            throw new InvalidOperationException($"Wartość {nameof(KSeFApiOptions.MaxAuthenticationStatusPollAttempts)} musi być większa od zera.");

        if (options.AuthenticationStatusPollDelay <= TimeSpan.Zero)
            throw new InvalidOperationException($"Wartość {nameof(KSeFApiOptions.AuthenticationStatusPollDelay)} musi być większa od zera.");
    }
}
