using MediPrax.Core.Enums;

namespace MediPrax.Application.DTOs;

public class AppointmentSeriesDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public Guid DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public Guid? TherapyCaseId { get; set; }
    public RecurrencePattern RecurrencePattern { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public int DurationMinutes { get; set; }
    public DateOnly SeriesStartDate { get; set; }
    public DateOnly? SeriesEndDate { get; set; }
    public int? MaxOccurrences { get; set; }
    public string? Notes { get; set; }
    public int GeneratedAppointments { get; set; }

    public string RecurrenceDisplay => RecurrencePattern switch
    {
        RecurrencePattern.Weekly => "Wöchentlich",
        RecurrencePattern.BiWeekly => "Zweiwöchentlich",
        RecurrencePattern.Monthly => "Monatlich",
        _ => RecurrencePattern.ToString()
    };

    public string DayOfWeekDisplay => DayOfWeek switch
    {
        DayOfWeek.Monday => "Montag",
        DayOfWeek.Tuesday => "Dienstag",
        DayOfWeek.Wednesday => "Mittwoch",
        DayOfWeek.Thursday => "Donnerstag",
        DayOfWeek.Friday => "Freitag",
        DayOfWeek.Saturday => "Samstag",
        DayOfWeek.Sunday => "Sonntag",
        _ => DayOfWeek.ToString()
    };
}

public class CreateAppointmentSeriesDto
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid? TherapyCaseId { get; set; }
    public RecurrencePattern RecurrencePattern { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public int DurationMinutes { get; set; } = 50;
    public DateOnly SeriesStartDate { get; set; }
    public DateOnly? SeriesEndDate { get; set; }
    public int? MaxOccurrences { get; set; }
    public string? Notes { get; set; }
}
