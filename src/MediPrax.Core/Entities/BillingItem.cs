using MediPrax.Core.Enums;

namespace MediPrax.Core.Entities;

public class BillingItem : BaseEntity
{
    public Guid EncounterId { get; set; }
    public Encounter Encounter { get; set; } = null!;

    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public required string GopCode { get; set; }
    public string? GopDescription { get; set; }
    public int Quantity { get; set; } = 1;
    public BillingType BillingType { get; set; }
    public string? Quarter { get; set; } // z.B. "2026-Q2"
    public bool KvdtExported { get; set; }
}
