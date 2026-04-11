using MediPrax.Core.Enums;

namespace MediPrax.Core.Entities;

public class PatientDiagnosis : BaseEntity
{
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public required string Icd10Code { get; set; }
    public required string Icd10Description { get; set; }

    public DiagnosisCertainty Certainty { get; set; } = DiagnosisCertainty.G;
    public DiagnosisLaterality? Laterality { get; set; }
    public DiagnosisType DiagnosisType { get; set; } = DiagnosisType.Encounterdiagnose;
    public DiagnosisStatus Status { get; set; } = DiagnosisStatus.Active;

    public DateOnly? OnsetDate { get; set; }
    public DateOnly? ResolvedDate { get; set; }
    public string? Notes { get; set; }

    public Guid CreatedByDoctorId { get; set; }
    public User CreatedByDoctor { get; set; } = null!;

    public ICollection<EncounterDiagnosis> EncounterDiagnoses { get; set; } = [];
}
