using MediPrax.Core.Enums;

namespace MediPrax.Application.DTOs;

public class SuicidalityAssessmentDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public Guid EncounterId { get; set; }
    public string AssessedByName { get; set; } = string.Empty;
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

    public string RiskLevelDisplay => RiskLevel switch
    {
        SuicidalityRiskLevel.None => "Kein Risiko",
        SuicidalityRiskLevel.Low => "Gering",
        SuicidalityRiskLevel.Moderate => "Mittel",
        SuicidalityRiskLevel.High => "Hoch",
        SuicidalityRiskLevel.Acute => "Akut",
        _ => RiskLevel.ToString()
    };

    public string RiskLevelColor => RiskLevel switch
    {
        SuicidalityRiskLevel.None => "success",
        SuicidalityRiskLevel.Low => "info",
        SuicidalityRiskLevel.Moderate => "warning",
        SuicidalityRiskLevel.High => "danger",
        SuicidalityRiskLevel.Acute => "danger",
        _ => "secondary"
    };
}

public class CreateSuicidalityAssessmentDto
{
    public Guid PatientId { get; set; }
    public Guid EncounterId { get; set; }
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
