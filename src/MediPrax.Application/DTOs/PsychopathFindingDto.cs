using MediPrax.Core.ValueObjects;

namespace MediPrax.Application.DTOs;

public class PsychopathFindingDto
{
    public Guid Id { get; set; }
    public Guid EncounterId { get; set; }
    public Guid PatientId { get; set; }
    public string AssessedByName { get; set; } = string.Empty;
    public DateOnly AssessmentDate { get; set; }
    public List<SymptomFinding> Findings { get; set; } = [];
    public string? NarrativeText { get; set; }
    public string? Notes { get; set; }
}

public class PsychopathFindingListItemDto
{
    public Guid Id { get; set; }
    public DateOnly AssessmentDate { get; set; }
    public string AssessedByName { get; set; } = string.Empty;
    public string? NarrativeTextPreview { get; set; }
}

public class CreatePsychopathFindingDto
{
    public Guid EncounterId { get; set; }
    public Guid PatientId { get; set; }
    public Guid AssessedById { get; set; }
    public DateOnly AssessmentDate { get; set; }
    public List<SymptomFinding> Findings { get; set; } = [];
    public string? NarrativeText { get; set; }
    public string? Notes { get; set; }
}

public class UpdatePsychopathFindingDto : CreatePsychopathFindingDto
{
    public Guid Id { get; set; }
}
