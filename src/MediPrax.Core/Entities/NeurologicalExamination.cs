using MediPrax.Core.ValueObjects;

namespace MediPrax.Core.Entities;

public class NeurologicalExamination : BaseEntity
{
    public Guid EncounterId { get; set; }
    public Encounter Encounter { get; set; } = null!;

    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public Guid ExaminedById { get; set; }
    public User ExaminedBy { get; set; } = null!;

    public DateOnly ExamDate { get; set; }

    public CranialNerveFindings CranialNerves { get; set; } = new();
    public MotorFindings MotorSystem { get; set; } = new();
    public ReflexFindings Reflexes { get; set; } = new();
    public SensoryFindings SensorySystem { get; set; } = new();
    public CoordinationFindings Coordination { get; set; } = new();
    public GaitFindings Gait { get; set; } = new();
    public MeningealFindings MeningealSigns { get; set; } = new();

    public string? NarrativeText { get; set; }
    public string? Notes { get; set; }
}
