using System.Text.Json.Serialization;

namespace TBJ.Integrations.MF.KSeF.Models.Invoices;

/// <summary>
/// Podstawowe dane faktury zwracane przez KSeF API.
/// </summary>
public sealed class Invoice
{
    /// <summary>
    /// Numer KSeF faktury.
    /// </summary>
    [JsonPropertyName("ksefNumber")]
    public string KsefNumber { get; set; } = string.Empty;

    /// <summary>
    /// Numer faktury wystawiony przez sprzedawcę.
    /// </summary>
    [JsonPropertyName("invoiceNumber")]
    public string? InvoiceNumber { get; set; }

    /// <summary>
    /// Data wystawienia faktury.
    /// </summary>
    [JsonPropertyName("issueDate")]
    public DateTimeOffset? IssueDate { get; set; }

    /// <summary>
    /// Numer NIP sprzedawcy.
    /// </summary>
    [JsonPropertyName("sellerNip")]
    public string? SellerNip { get; set; }

    /// <summary>
    /// Numer NIP nabywcy.
    /// </summary>
    [JsonPropertyName("buyerNip")]
    public string? BuyerNip { get; set; }

    /// <summary>
    /// Kwota brutto faktury.
    /// </summary>
    [JsonPropertyName("grossAmount")]
    public decimal? GrossAmount { get; set; }

    /// <summary>
    /// Status faktury.
    /// </summary>
    [JsonPropertyName("status")]
    public InvoiceStatus? Status { get; set; }
}
