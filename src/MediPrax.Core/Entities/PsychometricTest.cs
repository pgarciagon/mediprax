using MediPrax.Core.Enums;
using MediPrax.Core.ValueObjects;

namespace MediPrax.Core.Entities;

public class PsychometricTest : BaseEntity
{
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public Guid? EncounterId { get; set; }
    public Encounter? Encounter { get; set; }

    public Guid AdministeredById { get; set; }
    public User AdministeredBy { get; set; } = null!;

    public PsychometricTestType TestType { get; set; }
    public required DateOnly TestDate { get; set; }
    public List<TestResponse> Responses { get; set; } = [];
    public int TotalScore { get; set; }
    public string Interpretation { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public TestStatus Status { get; set; } = TestStatus.InProgress;
}
