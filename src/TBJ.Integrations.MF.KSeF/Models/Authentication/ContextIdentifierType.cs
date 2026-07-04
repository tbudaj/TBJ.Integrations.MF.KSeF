namespace TBJ.Integrations.MF.KSeF.Models.Authentication;

/// <summary>
/// Rodzaj identyfikatora kontekstu używanego przy uwierzytelnianiu w KSeF.
/// </summary>
public enum ContextIdentifierType
{
    /// <summary>
    /// Numer identyfikacji podatkowej (NIP).
    /// </summary>
    Nip,

    /// <summary>
    /// Wewnętrzny identyfikator KSeF.
    /// </summary>
    InternalId,

    /// <summary>
    /// NIP dla podatników VAT UE.
    /// </summary>
    NipVatUe
}
