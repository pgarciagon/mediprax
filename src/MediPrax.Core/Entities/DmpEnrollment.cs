using MediPrax.Core.Enums;

namespace MediPrax.Core.Entities;

public class DmpEnrollment : BaseEntity
{
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public required string DmpType { get; set; }
    public DateOnly EnrollmentDate { get; set; }
    public DateOnly? DisenrollmentDate { get; set; }
    public DmpStatus Status { get; set; } = DmpStatus.Active;

    public ICollection<DmpDocumentation> Documentations { get; set; } = [];
}
