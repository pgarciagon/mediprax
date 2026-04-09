namespace MediPrax.Core.Entities;

public class DoctorScheduleTemplate : BaseEntity
{
    public Guid DoctorId { get; set; }
    public User Doctor { get; set; } = null!;

    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int SlotDurationMinutes { get; set; } = 25;
    public bool IsActive { get; set; } = true;
}
