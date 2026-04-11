namespace MediPrax.Core.Enums;

/// <summary>
/// Whether a diagnosis is a permanent patient-level diagnosis or specific to one encounter.
/// </summary>
public enum DiagnosisType
{
    /// <summary>Permanent diagnosis that auto-inherits to new encounters</summary>
    Dauerdiagnose = 0,
    /// <summary>Diagnosis specific to a single encounter</summary>
    Encounterdiagnose = 1
}
