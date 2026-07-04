namespace TBJ.Integrations.MF.KSeF.Models.Sessions;

/// <summary>
/// Typ sesji wystawiania faktur w KSeF.
/// </summary>
public enum SessionType
{
    /// <summary>
    /// Sesja interaktywna — pojedyncze faktury.
    /// </summary>
    Online,

    /// <summary>
    /// Sesja wsadowa — paczka faktur.
    /// </summary>
    Batch
}
