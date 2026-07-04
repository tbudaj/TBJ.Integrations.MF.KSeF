using System.Text.Json.Serialization;

namespace TBJ.Integrations.MF.KSeF.Models.Invoices;

/// <summary>
/// Urzędowe Potwierdzenie Odbioru dla faktury.
/// </summary>
public sealed class InvoiceUpo
{
    /// <summary>
    /// Numer KSeF faktury.
    /// </summary>
    [JsonPropertyName("ksefNumber")]
    public string KsefNumber { get; set; } = string.Empty;

    /// <summary>
    /// Treść UPO (Base64).
    /// </summary>
    [JsonPropertyName("upo")]
    public string? Upo { get; set; }

    /// <summary>
    /// Data wygenerowania UPO.
    /// </summary>
    [JsonPropertyName("generationDate")]
    public DateTimeOffset? GenerationDate { get; set; }
}
