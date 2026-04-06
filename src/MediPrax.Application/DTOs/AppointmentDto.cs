using MediPrax.Core.Enums;

namespace MediPrax.Application.DTOs;

public class AppointmentDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public Guid DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public int DurationMinutes { get; set; }
    public AppointmentStatus Status { get; set; }
    public string? Notes { get; set; }

    public DateTime EndTime => StartTime.AddMinutes(DurationMinutes);
    public string TimeRange => $"{StartTime:HH:mm}–{EndTime:HH:mm}";
}

public class CreateAppointmentDto
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public DateTime StartTime { get; set; }
    public int DurationMinutes { get; set; } = 10;
    public string? Notes { get; set; }
}

public class DayScheduleDto
{
    public DateOnly Date { get; set; }
    public Guid DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public IReadOnlyList<AppointmentDto> Appointments { get; set; } = [];
}

public class WeekScheduleDto
{
    public DateOnly WeekStart { get; set; }
    public IReadOnlyList<DayScheduleDto> Days { get; set; } = [];
}

public class WaitingRoomEntryDto
{
    public Guid AppointmentId { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public int DurationMinutes { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public AppointmentStatus Status { get; set; }
    public string? Notes { get; set; }

    public string StatusDisplay => Status switch
    {
        AppointmentStatus.Scheduled => "Erwartet",
        AppointmentStatus.CheckedIn => "Im Wartezimmer",
        AppointmentStatus.InProgress => "In Behandlung",
        AppointmentStatus.Completed => "Fertig",
        _ => Status.ToString()
    };
}
