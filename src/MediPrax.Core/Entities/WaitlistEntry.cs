using MediPrax.Core.Enums;

namespace MediPrax.Core.Entities;

public class WaitlistEntry : BaseEntity
{
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public Guid? PreferredTherapistId { get; set; }
    public User? PreferredTherapist { get; set; }

    public DateOnly RequestDate { get; set; }
    public TherapyType? TherapyTypeRequested { get; set; }
    public WaitlistPriority Priority { get; set; }
    public List<DayOfWeek>? PreferredDays { get; set; }
    public string? PreferredTimeSlot { get; set; }
    public string? Notes { get; set; }
    public WaitlistStatus Status { get; set; } = WaitlistStatus.Waiting;
    public DateOnly? OfferedDate { get; set; }
}
