using MediPrax.Core.ValueObjects;

namespace MediPrax.Core.Entities;

public class PsychopathologicalFinding : BaseEntity
{
    public Guid EncounterId { get; set; }
    public Encounter Encounter { get; set; } = null!;

    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public Guid AssessedById { get; set; }
    public User AssessedBy { get; set; } = null!;

    public required DateOnly AssessmentDate { get; set; }
    public List<SymptomFinding> Findings { get; set; } = [];
    public string? NarrativeText { get; set; }
    public string? Notes { get; set; }
}
