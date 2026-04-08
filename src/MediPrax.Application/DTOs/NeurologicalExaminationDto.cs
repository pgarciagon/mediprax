using MediPrax.Core.ValueObjects;

namespace MediPrax.Application.DTOs;

public class NeurologicalExaminationDto
{
    public Guid Id { get; set; }
    public Guid EncounterId { get; set; }
    public Guid PatientId { get; set; }
    public string ExaminedByName { get; set; } = string.Empty;
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

public class NeurologicalExamListItemDto
{
    public Guid Id { get; set; }
    public DateOnly ExamDate { get; set; }
    public string ExaminedByName { get; set; } = string.Empty;
    public string NarrativePreview { get; set; } = string.Empty; // First 200 chars
}

public class CreateNeurologicalExamDto
{
    public Guid EncounterId { get; set; }
    public Guid PatientId { get; set; }
    public Guid ExaminedById { get; set; }
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

public class UpdateNeurologicalExamDto : CreateNeurologicalExamDto
{
    public Guid Id { get; set; }
}
