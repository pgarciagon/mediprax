namespace MediPrax.Core.Enums;

/// <summary>
/// ICD-10 Diagnosesicherheit (KBV §295 SGB V, KVDT field 6003).
/// </summary>
public enum DiagnosisCertainty
{
    /// <summary>Gesicherte Diagnose</summary>
    G = 0,
    /// <summary>Verdachtsdiagnose</summary>
    V = 1,
    /// <summary>Zustand nach (Z.n.)</summary>
    Z = 2,
    /// <summary>Ausschluss</summary>
    A = 3
}
