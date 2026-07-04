using TBJ.Integrations.MF.KSeF.Abstractions;
using TBJ.Integrations.MF.KSeF.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TBJ.Integrations.MF.KSeF.Tests;

/// <summary>
/// Testy rejestracji DI dla biblioteki klienta KSeF API v2.
/// </summary>
public class KSeFApiExtensionsTests
{
    [Fact]
    public void AddKSeFApi_RejestrujeWszystkichKlientow()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        services.AddKSeFApi(opt =>
        {
            opt.BaseUrl = "https://api-test.ksef.mf.gov.pl/v2";
            opt.Timeout = TimeSpan.FromSeconds(60);
        });

        var provider = services.BuildServiceProvider();

        // Assert
        Assert.NotNull(provider.GetService<IAuthenticationClient>());
        Assert.NotNull(provider.GetService<ISessionsClient>());
        Assert.NotNull(provider.GetService<IInvoicesClient>());
    }

    [Fact]
    public void AddKSeFApi_ZKonfiguracja_RejestrujeOpcje()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["KSeFApi:BaseUrl"] = "https://api-test.ksef.mf.gov.pl/v2",
                ["KSeFApi:Timeout"] = "00:01:30",
                ["KSeFApi:DefaultAccessToken"] = "eyJtest"
            })
            .Build();

        // Act
        services.AddKSeFApi(configuration);
        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<KSeFApiOptions>();

        // Assert
        Assert.Equal("https://api-test.ksef.mf.gov.pl/v2", options.BaseUrl);
        Assert.Equal(TimeSpan.FromSeconds(90), options.Timeout);
        Assert.Equal("eyJtest", options.DefaultAccessToken);
    }

    [Fact]
    public void AddKSeFApi_BrakBaseUrl_RzucaWyjatek()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            services.AddKSeFApi(opt => opt.BaseUrl = string.Empty));
    }
}
