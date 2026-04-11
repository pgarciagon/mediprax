using MediPrax.Core.Enums;

namespace MediPrax.Application.DTOs;

public class EncounterDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public Guid DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public Guid? AppointmentId { get; set; }
    public DateOnly EncounterDate { get; set; }
    public string? Notes { get; set; }
    public List<Icd10EntryDto> Icd10Codes { get; set; } = [];
    public int? DurationMinutes { get; set; }
    public EncounterStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<EncounterSectionDto> Sections { get; set; } = [];
}

public class Icd10EntryDto
{
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class CreateEncounterDto
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid? AppointmentId { get; set; }
    public DateOnly EncounterDate { get; set; }
    public string? Notes { get; set; }
    public List<string> Icd10Codes { get; set; } = [];
    public int? DurationMinutes { get; set; }
    public List<CreateEncounterSectionDto>? Sections { get; set; }
}

public class UpdateEncounterDto : CreateEncounterDto
{
    public Guid Id { get; set; }
    public EncounterStatus Status { get; set; }
}

public class EncounterListItemDto
{
    public Guid Id { get; set; }
    public DateOnly EncounterDate { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public List<string> Icd10Codes { get; set; } = [];
    public int? DurationMinutes { get; set; }
    public EncounterStatus Status { get; set; }
}
