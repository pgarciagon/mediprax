using MediPrax.Core.Enums;

namespace MediPrax.Core.Entities;

public class DoctorAbsence : BaseEntity
{
    public Guid DoctorId { get; set; }
    public User Doctor { get; set; } = null!;

    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public TimeOnly? StartTime { get; set; } // null = full day
    public TimeOnly? EndTime { get; set; }   // null = full day
    public AbsenceType AbsenceType { get; set; }
    public string? Reason { get; set; }

    public Guid? SubstituteId { get; set; }
    public User? Substitute { get; set; }

    public bool IsFullDay => !StartTime.HasValue;
}
