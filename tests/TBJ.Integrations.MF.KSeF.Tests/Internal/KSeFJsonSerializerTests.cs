using TBJ.Integrations.MF.KSeF.Internal;
using TBJ.Integrations.MF.KSeF.Models.Authentication;

namespace TBJ.Integrations.MF.KSeF.Tests.Internal;

/// <summary>
/// Testy serializacji/deserializacji JSON dla KSeF API.
/// </summary>
public class KSeFJsonSerializerTests
{
    [Fact]
    public void Serialize_IgnorujeWartosciNull()
    {
        // Arrange
        var request = new TokenRefreshRequest { RefreshToken = "token" };

        // Act
        var json = KSeFJsonSerializer.Serialize(request);

        // Assert
        Assert.Contains("\"refreshToken\":\"token\"", json);
    }

    [Fact]
    public void Deserialize_AuthenticationChallengeResponse_ZwracaObiekt()
    {
        // Arrange
        const string json = "{ \"challenge\": \"abc\", \"timestamp\": 1234567890 }";

        // Act
        var result = KSeFJsonSerializer.Deserialize<AuthenticationChallengeResponse>(json);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("abc", result.Challenge);
        Assert.Equal(1234567890, result.Timestamp);
    }
}
