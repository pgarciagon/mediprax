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

    // Arztbrief-specific fields
    public ArztbriefStatus? ArztbriefStatus { get; set; }
    public Guid? DoctorId { get; set; }
    public string? RecipientName { get; set; }
    public string? RecipientAddress { get; set; }
    public string? Subject { get; set; }
    public List<string>? Diagnoses { get; set; }
}
