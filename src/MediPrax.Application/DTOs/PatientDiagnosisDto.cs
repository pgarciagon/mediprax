using MediPrax.Core.Enums;

namespace MediPrax.Application.DTOs;

public class PatientDiagnosisDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string Icd10Code { get; set; } = string.Empty;
    public string Icd10Description { get; set; } = string.Empty;
    public DiagnosisCertainty Certainty { get; set; }
    public DiagnosisLaterality? Laterality { get; set; }
    public DiagnosisType DiagnosisType { get; set; }
    public DiagnosisStatus Status { get; set; }
    public DateOnly? OnsetDate { get; set; }
    public DateOnly? ResolvedDate { get; set; }
    public string? Notes { get; set; }
    public Guid CreatedByDoctorId { get; set; }
    public string? CreatedByDoctorName { get; set; }
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Formatted display: "F32.1 (G) Mittelgradige depressive Episode"
    /// </summary>
    public string DisplayText => $"{Icd10Code} ({Certainty}){(Laterality.HasValue ? $" {Laterality}" : "")} {Icd10Description}";

    /// <summary>
    /// Short format for chips: "F32.1 (G)"
    /// </summary>
    public string ShortDisplay => $"{Icd10Code} ({Certainty}){(Laterality.HasValue ? $" {Laterality}" : "")}";
}

public class CreatePatientDiagnosisDto
{
    public Guid PatientId { get; set; }
    public required string Icd10Code { get; set; }
    public DiagnosisCertainty Certainty { get; set; } = DiagnosisCertainty.G;
    public DiagnosisLaterality? Laterality { get; set; }
    public DiagnosisType DiagnosisType { get; set; } = DiagnosisType.Encounterdiagnose;
    public DateOnly? OnsetDate { get; set; }
    public string? Notes { get; set; }
    public Guid CreatedByDoctorId { get; set; }
}

public class UpdatePatientDiagnosisDto
{
    public Guid Id { get; set; }
    public DiagnosisCertainty Certainty { get; set; }
    public DiagnosisLaterality? Laterality { get; set; }
    public DiagnosisType DiagnosisType { get; set; }
    public DiagnosisStatus Status { get; set; }
    public DateOnly? ResolvedDate { get; set; }
    public string? Notes { get; set; }
}

public class EncounterDiagnosisDto
{
    public Guid Id { get; set; }
    public Guid EncounterId { get; set; }
    public Guid PatientDiagnosisId { get; set; }
    public bool IsNewInThisEncounter { get; set; }

    // Flattened from PatientDiagnosis
    public string Icd10Code { get; set; } = string.Empty;
    public string Icd10Description { get; set; } = string.Empty;
    public DiagnosisCertainty Certainty { get; set; }
    public DiagnosisLaterality? Laterality { get; set; }
    public DiagnosisType DiagnosisType { get; set; }
}
