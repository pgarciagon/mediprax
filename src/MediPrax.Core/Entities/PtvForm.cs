using MediPrax.Core.Enums;

namespace MediPrax.Core.Entities;

public class PtvForm : BaseEntity
{
    public Guid TherapyCaseId { get; set; }
    public TherapyCase TherapyCase { get; set; } = null!;

    public PtvFormType FormType { get; set; }
    public DateOnly CreatedDate { get; set; }
    public Dictionary<string, string> FormData { get; set; } = new();
    public byte[]? PdfData { get; set; }
    public PtvFormStatus Status { get; set; }
}
