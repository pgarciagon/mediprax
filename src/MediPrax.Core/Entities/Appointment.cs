using MediPrax.Core.Enums;

namespace MediPrax.Core.Entities;

public class Appointment : BaseEntity
{
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public Guid DoctorId { get; set; }
    public User Doctor { get; set; } = null!;

    public required DateTime StartTime { get; set; }
    public int DurationMinutes { get; set; } = 10;
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;
    public string? Notes { get; set; }

    public Encounter? Encounter { get; set; }
}
