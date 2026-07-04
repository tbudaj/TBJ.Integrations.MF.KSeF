using System.Text.Json.Serialization;

namespace TBJ.Integrations.MF.KSeF.Models.Sessions;

/// <summary>
/// Status sesji wystawiania faktur.
/// </summary>
public sealed class SessionStatusResponse
{
    /// <summary>
    /// Numer referencyjny sesji.
    /// </summary>
    [JsonPropertyName("referenceNumber")]
    public string ReferenceNumber { get; set; } = string.Empty;

    /// <summary>
    /// Status sesji.
    /// </summary>
    [JsonPropertyName("status")]
    public SessionStatusInfo Status { get; set; } = new();

    /// <summary>
    /// Liczba faktur w sesji.
    /// </summary>
    [JsonPropertyName("invoiceCount")]
    public int? InvoiceCount { get; set; }

    /// <summary>
    /// Liczba faktur zaakceptowanych.
    /// </summary>
    [JsonPropertyName("acceptedInvoiceCount")]
    public int? AcceptedInvoiceCount { get; set; }

    /// <summary>
    /// Liczba faktur odrzuconych.
    /// </summary>
    [JsonPropertyName("rejectedInvoiceCount")]
    public int? RejectedInvoiceCount { get; set; }
}
