using System.Text.Json.Serialization;

namespace TBJ.Integrations.MF.KSeF.Models.Authentication;

/// <summary>
/// Opcjonalna polityka autoryzacji dla tokena KSeF.
/// </summary>
public sealed class AuthorizationPolicy
{
    /// <summary>
    /// Lista dozwolonych adresów IP, z których można używać tokena.
    /// </summary>
    [JsonPropertyName("allowedIps")]
    public IReadOnlyList<string>? AllowedIps { get; set; }
}
