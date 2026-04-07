using MediPrax.Core.Enums;

namespace MediPrax.Core.Entities;

/// <summary>
/// Wiedervorlage — Follow-up reminder for a patient.
/// </summary>
public class Recall : BaseEntity
{
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public Guid CreatedById { get; set; }
    public User CreatedBy { get; set; } = null!;

    public required DateOnly DueDate { get; set; }
    public required string Reason { get; set; }
    public string? Notes { get; set; }
    public RecallStatus Status { get; set; } = RecallStatus.Open;
    public DateOnly? CompletedDate { get; set; }
}
