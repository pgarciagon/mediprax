using MediPrax.Core.Enums;

namespace MediPrax.Core.Entities;

public class AppointmentSeries : BaseEntity
{
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public Guid DoctorId { get; set; }
    public User Doctor { get; set; } = null!;

    public Guid? TherapyCaseId { get; set; }
    public TherapyCase? TherapyCase { get; set; }

    public RecurrencePattern RecurrencePattern { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public int DurationMinutes { get; set; }
    public DateOnly SeriesStartDate { get; set; }
    public DateOnly? SeriesEndDate { get; set; }
    public int? MaxOccurrences { get; set; }
    public string? Notes { get; set; }

    public ICollection<Appointment> Appointments { get; set; } = [];
}
