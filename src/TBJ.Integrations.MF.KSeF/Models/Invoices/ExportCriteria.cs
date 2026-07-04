using System.Text.Json.Serialization;

namespace TBJ.Integrations.MF.KSeF.Models.Invoices;

/// <summary>
/// Kryteria eksportu faktur.
/// </summary>
public sealed class ExportCriteria
{
    /// <summary>
    /// Data początkowa zakresu eksportu.
    /// </summary>
    [JsonPropertyName("dateFrom")]
    public DateTimeOffset? DateFrom { get; set; }

    /// <summary>
    /// Data końcowa zakresu eksportu.
    /// </summary>
    [JsonPropertyName("dateTo")]
    public DateTimeOffset? DateTo { get; set; }

    /// <summary>
    /// Numer NIP kontrahenta.
    /// </summary>
    [JsonPropertyName("partyNip")]
    public string? PartyNip { get; set; }

    /// <summary>
    /// Rodzaj faktur do wyeksportowania.
    /// </summary>
    [JsonPropertyName("invoiceType")]
    public string? InvoiceType { get; set; }
}
