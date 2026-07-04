using System.Text.Json;
using System.Text.Json.Serialization;

namespace TBJ.Integrations.MF.KSeF.Internal;

/// <summary>
/// Wewnętrzny serializer JSON dla KSeF API v2.
/// Używa konwencji camelCase i ignoruje właściwości o wartości <c>null</c>.
/// </summary>
internal static class KSeFJsonSerializer
{
    /// <summary>
    /// Domyślne opcje serializacji dla KSeF API.
    /// </summary>
    public static JsonSerializerOptions Options { get; } = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
    };

    /// <summary>
    /// Deserializuje ciąg JSON do obiektu danego typu.
    /// </summary>
    public static T? Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, Options);
    }

    /// <summary>
    /// Serializuje obiekt do ciągu JSON.
    /// </summary>
    public static string Serialize<T>(T value)
    {
        return JsonSerializer.Serialize(value, Options);
    }
}
