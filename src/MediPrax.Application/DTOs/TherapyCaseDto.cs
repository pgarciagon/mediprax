using MediPrax.Core.Enums;

namespace MediPrax.Application.DTOs;

public class TherapyCaseDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public Guid TherapistId { get; set; }
    public string TherapistName { get; set; } = string.Empty;
    public TherapyType TherapyType { get; set; }
    public string TherapyTypeName { get; set; } = string.Empty;
    public TherapyCaseStatus Status { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public int ApprovedSessions { get; set; }
    public int CompletedSessions { get; set; }
    public int RemainingSessions { get; set; }
    public int SessionDurationMinutes { get; set; }
    public bool IsGroupTherapy { get; set; }
    public GutachterStatus? GutachterStatus { get; set; }
    public List<string> Diagnoses { get; set; } = [];
    public string? Notes { get; set; }
    public string WarningLevel { get; set; } = "None"; // None, Yellow, Red
}

public class TherapyCaseListItemDto
{
    public Guid Id { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string TherapyTypeName { get; set; } = string.Empty;
    public TherapyCaseStatus Status { get; set; }
    public int CompletedSessions { get; set; }
    public int ApprovedSessions { get; set; }
    public int RemainingSessions { get; set; }
    public DateOnly StartDate { get; set; }
    public string WarningLevel { get; set; } = "None";
}

public class CreateTherapyCaseDto
{
    public Guid PatientId { get; set; }
    public Guid TherapistId { get; set; }
    public TherapyType TherapyType { get; set; }
    public DateOnly StartDate { get; set; }
    public int ApprovedSessions { get; set; }
    public int SessionDurationMinutes { get; set; }
    public bool IsGroupTherapy { get; set; }
    public List<string> Diagnoses { get; set; } = [];
    public string? Notes { get; set; }
}

public class UpdateTherapyCaseDto : CreateTherapyCaseDto
{
    public Guid Id { get; set; }
    public TherapyCaseStatus Status { get; set; }
    public GutachterStatus? GutachterStatus { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? InsuranceApprovalRef { get; set; }
}
