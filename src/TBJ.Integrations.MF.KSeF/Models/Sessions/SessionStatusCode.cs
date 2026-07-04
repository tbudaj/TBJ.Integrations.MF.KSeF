namespace TBJ.Integrations.MF.KSeF.Models.Sessions;

/// <summary>
/// Status sesji wystawiania faktur w KSeF.
/// </summary>
public enum SessionStatusCode
{
    /// <summary>
    /// Sesja otwarta.
    /// </summary>
    Open,

    /// <summary>
    /// Sesja w trakcie przetwarzania.
    /// </summary>
    Processing,

    /// <summary>
    /// Sesja zakończona sukcesem.
    /// </summary>
    Completed,

    /// <summary>
    /// Sesja anulowana.
    /// </summary>
    Cancelled,

    /// <summary>
    /// Sesja zakończona błędem.
    /// </summary>
    Failed
}
