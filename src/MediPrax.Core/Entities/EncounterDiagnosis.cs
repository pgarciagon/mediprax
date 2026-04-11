namespace MediPrax.Core.Entities;

public class EncounterDiagnosis : BaseEntity
{
    public Guid EncounterId { get; set; }
    public Encounter Encounter { get; set; } = null!;

    public Guid PatientDiagnosisId { get; set; }
    public PatientDiagnosis PatientDiagnosis { get; set; } = null!;

    /// <summary>
    /// Whether this diagnosis was first documented in this encounter (vs inherited from Dauerdiagnosen).
    /// </summary>
    public bool IsNewInThisEncounter { get; set; }
}
