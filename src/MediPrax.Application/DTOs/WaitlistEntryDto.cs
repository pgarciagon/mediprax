using MediPrax.Core.Enums;

namespace MediPrax.Application.DTOs;

public class WaitlistEntryDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public Guid? PreferredTherapistId { get; set; }
    public string? PreferredTherapistName { get; set; }
    public DateOnly RequestDate { get; set; }
    public TherapyType? TherapyTypeRequested { get; set; }
    public WaitlistPriority Priority { get; set; }
    public List<DayOfWeek>? PreferredDays { get; set; }
    public string? PreferredTimeSlot { get; set; }
    public string? Notes { get; set; }
    public WaitlistStatus Status { get; set; }
    public DateOnly? OfferedDate { get; set; }

    public int WaitDays => DateOnly.FromDateTime(DateTime.Today).DayNumber - RequestDate.DayNumber;

    public string PriorityDisplay => Priority switch
    {
        WaitlistPriority.Normal => "Normal",
        WaitlistPriority.Urgent => "Dringend",
        _ => Priority.ToString()
    };

    public string StatusDisplay => Status switch
    {
        WaitlistStatus.Waiting => "Wartend",
        WaitlistStatus.Offered => "Angeboten",
        WaitlistStatus.Scheduled => "Eingeplant",
        WaitlistStatus.Cancelled => "Storniert",
        _ => Status.ToString()
    };
}

public class CreateWaitlistEntryDto
{
    public Guid PatientId { get; set; }
    public Guid? PreferredTherapistId { get; set; }
    public TherapyType? TherapyTypeRequested { get; set; }
    public WaitlistPriority Priority { get; set; }
    public List<DayOfWeek>? PreferredDays { get; set; }
    public string? PreferredTimeSlot { get; set; }
    public string? Notes { get; set; }
}

public class UpdateWaitlistEntryDto
{
    public Guid? PreferredTherapistId { get; set; }
    public TherapyType? TherapyTypeRequested { get; set; }
    public WaitlistPriority Priority { get; set; }
    public List<DayOfWeek>? PreferredDays { get; set; }
    public string? PreferredTimeSlot { get; set; }
    public string? Notes { get; set; }
}

public class WaitlistStatisticsDto
{
    public int TotalWaiting { get; set; }
    public double AverageWaitDays { get; set; }
    public int UrgentCount { get; set; }
    public IReadOnlyList<TherapistQueueDto> TherapistQueues { get; set; } = [];
}

public class TherapistQueueDto
{
    public Guid TherapistId { get; set; }
    public string TherapistName { get; set; } = string.Empty;
    public int QueueLength { get; set; }
}
