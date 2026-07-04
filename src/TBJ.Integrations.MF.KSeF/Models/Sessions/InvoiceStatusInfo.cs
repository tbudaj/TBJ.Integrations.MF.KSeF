using System.Text.Json.Serialization;

namespace TBJ.Integrations.MF.KSeF.Models.Sessions;

/// <summary>
/// Informacja o statusie faktury w sesji.
/// </summary>
public sealed class InvoiceStatusInfo
{
    /// <summary>
    /// Kod statusu faktury.
    /// </summary>
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Opis statusu faktury.
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
}
