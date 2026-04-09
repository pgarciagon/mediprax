using MediPrax.Core.Enums;

namespace MediPrax.Application.DTOs;

public class ScheduleBlockDto
{
    public Guid Id { get; set; }
    public Guid DoctorId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int SlotDurationMinutes { get; set; }
    public bool IsActive { get; set; }
}

public class CreateScheduleBlockDto
{
    public Guid DoctorId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int SlotDurationMinutes { get; set; } = 25;
}

public class AbsenceDto
{
    public Guid Id { get; set; }
    public Guid DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public AbsenceType AbsenceType { get; set; }
    public string? Reason { get; set; }
    public string? SubstituteName { get; set; }
    public bool IsFullDay => !StartTime.HasValue;

    public string TypeDisplay => AbsenceType switch
    {
        AbsenceType.Urlaub => "Urlaub",
        AbsenceType.Fortbildung => "Fortbildung",
        AbsenceType.Krank => "Krank",
        AbsenceType.Sperrzeit => "Sperrzeit",
        _ => AbsenceType.ToString()
    };
}

public class CreateAbsenceDto
{
    public Guid DoctorId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public AbsenceType AbsenceType { get; set; }
    public string? Reason { get; set; }
    public Guid? SubstituteId { get; set; }
}

public class FreeSlotDto
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public int DurationMinutes { get; set; }
    public Guid DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;

    public string TimeDisplay => $"{Start.ToLocalTime():HH:mm} – {End.ToLocalTime():HH:mm}";
    public string DateDisplay => $"{Start.ToLocalTime():ddd dd.MM.yyyy}";
}

public class AvailabilityCheckResult
{
    public bool IsAvailable { get; set; }
    public string? ConflictReason { get; set; }
    public bool IsWarningOnly { get; set; } // true = allow with confirmation
}
