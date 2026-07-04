using System.Text.Json.Serialization;

namespace TBJ.Integrations.MF.KSeF.Models.Sessions;

/// <summary>
/// Metadane faktury przesyłanej w sesji online.
/// </summary>
public sealed class InvoiceMetadata
{
    /// <summary>
    /// Format faktury (np. <c>FA_V2</c>).
    /// </summary>
    [JsonPropertyName("format")]
    public string Format { get; set; } = string.Empty;

    /// <summary>
    /// Opcjonalny numer faktury.
    /// </summary>
    [JsonPropertyName("invoiceNumber")]
    public string? InvoiceNumber { get; set; }

    /// <summary>
    /// Data wystawienia faktury.
    /// </summary>
    [JsonPropertyName("issueDate")]
    public DateTimeOffset? IssueDate { get; set; }
}
