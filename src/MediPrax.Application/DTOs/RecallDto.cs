using MediPrax.Core.Enums;

namespace MediPrax.Application.DTOs;

public class RecallDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public DateOnly DueDate { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public RecallStatus Status { get; set; }
    public string CreatedByName { get; set; } = string.Empty;

    public bool IsOverdue => Status == RecallStatus.Open && DueDate < DateOnly.FromDateTime(DateTime.Today);

    public string StatusDisplay => Status switch
    {
        RecallStatus.Open => "Offen",
        RecallStatus.Scheduled => "Geplant",
        RecallStatus.Completed => "Erledigt",
        RecallStatus.Cancelled => "Abgesagt",
        _ => Status.ToString()
    };
}

public class CreateRecallDto
{
    public Guid PatientId { get; set; }
    public Guid CreatedById { get; set; }
    public DateOnly DueDate { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
