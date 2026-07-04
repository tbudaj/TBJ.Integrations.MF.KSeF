namespace TBJ.Integrations.MF.KSeF.Models.Authentication;

/// <summary>
/// Sposób identyfikacji podmiotu na podstawie certyfikatu.
/// </summary>
public enum SubjectIdentifierType
{
    /// <summary>
    /// Identyfikacja na podstawie tematu certyfikatu (Subject DN).
    /// </summary>
    CertificateSubject,

    /// <summary>
    /// Identyfikacja na podstawie odcisku certyfikatu (fingerprint).
    /// </summary>
    CertificateFingerprint
}
