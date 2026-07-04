using System.Text.Json.Serialization;

namespace TBJ.Integrations.MF.KSeF.Models.Invoices;

/// <summary>
/// Status faktury w KSeF.
/// </summary>
public sealed class InvoiceStatus
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
