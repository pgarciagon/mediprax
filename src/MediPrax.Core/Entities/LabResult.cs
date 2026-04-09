using MediPrax.Core.Enums;
using MediPrax.Core.ValueObjects;

namespace MediPrax.Core.Entities;

public class LabResult : BaseEntity
{
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public Guid? OrderedById { get; set; }
    public User? OrderedBy { get; set; }

    public string LabName { get; set; } = string.Empty;
    public DateOnly OrderDate { get; set; }
    public DateOnly? ResultDate { get; set; }
    public LabResultStatus Status { get; set; } = LabResultStatus.Ordered;

    public Guid? ReviewedById { get; set; }
    public DateTime? ReviewedAt { get; set; }

    public List<LabValue> Values { get; set; } = [];
    public string? Notes { get; set; }
    public string? ImportSource { get; set; }  // "LDT" or "Manual"
}
