namespace MediPrax.Core.Enums;

/// <summary>
/// Status of a patient-level diagnosis.
/// </summary>
public enum DiagnosisStatus
{
    /// <summary>Currently active diagnosis</summary>
    Active = 0,
    /// <summary>Previously treated, now in medical history</summary>
    Anamnestisch = 1,
    /// <summary>No longer relevant</summary>
    Inactive = 2
}
