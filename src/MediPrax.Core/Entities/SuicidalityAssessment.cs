using MediPrax.Core.Enums;

namespace MediPrax.Core.Entities;

public class SuicidalityAssessment : BaseEntity
{
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public Guid? EncounterId { get; set; }
    public Encounter? Encounter { get; set; }

    public Guid? AssessedById { get; set; }
    public User? AssessedBy { get; set; }

    public DateOnly AssessmentDate { get; set; }
    public SuicidalityRiskLevel RiskLevel { get; set; }
    public bool SuicidalIdeation { get; set; }
    public bool SuicidalPlans { get; set; }
    public bool SuicidalIntent { get; set; }
    public bool PriorAttempts { get; set; }
    public string? PriorAttemptsDetails { get; set; }
    public List<string> RiskFactors { get; set; } = [];
    public List<string> ProtectiveFactors { get; set; } = [];
    public string? SafetyPlan { get; set; }
    public List<string> ActionsTaken { get; set; } = [];
    public string? Notes { get; set; }
}
