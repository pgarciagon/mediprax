using MediPrax.Core.Enums;

namespace MediPrax.Core.Entities;

public class TherapySession : BaseEntity
{
    public Guid TherapyCaseId { get; set; }
    public TherapyCase TherapyCase { get; set; } = null!;

    public Guid? EncounterId { get; set; }
    public Encounter? Encounter { get; set; }

    public Guid? AppointmentId { get; set; }
    public Appointment? Appointment { get; set; }

    public int SessionNumber { get; set; }
    public DateOnly SessionDate { get; set; }
    public int DurationMinutes { get; set; } // 25 or 50
    public SessionType SessionType { get; set; }
    public bool IsVideoSession { get; set; }
    public string? Notes { get; set; }
    public string? BilledGop { get; set; }
}
