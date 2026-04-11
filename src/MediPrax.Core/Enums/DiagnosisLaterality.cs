namespace MediPrax.Core.Enums;

/// <summary>
/// ICD-10 Seitenlokalisation (KBV KVDT field 6004).
/// </summary>
public enum DiagnosisLaterality
{
    /// <summary>Rechts</summary>
    R = 0,
    /// <summary>Links</summary>
    L = 1,
    /// <summary>Beidseitig</summary>
    B = 2
}
