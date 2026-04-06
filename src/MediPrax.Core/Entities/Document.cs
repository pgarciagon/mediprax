using MediPrax.Core.Enums;

namespace MediPrax.Core.Entities;

public class Document : BaseEntity
{
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public Guid? EncounterId { get; set; }
    public Encounter? Encounter { get; set; }

    public DocumentType DocType { get; set; }
    public required string Title { get; set; }
    public string? Content { get; set; }
    public byte[]? PdfData { get; set; }
    public string? KimMessageId { get; set; }
}
