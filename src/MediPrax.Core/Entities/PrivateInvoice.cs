using MediPrax.Core.Enums;

namespace MediPrax.Core.Entities;

public class InvoiceItem
{
    public string Description { get; set; } = string.Empty;
    public string? GoaePosition { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Factor { get; set; } = 2.3m;
    public int Quantity { get; set; } = 1;
    public decimal Total => UnitPrice * Factor * Quantity;
}

public class PrivateInvoice : BaseEntity
{
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public required string InvoiceNumber { get; set; }
    public DateOnly InvoiceDate { get; set; }
    public DateOnly DueDate { get; set; }

    public List<InvoiceItem> Items { get; set; } = [];
    public decimal TotalNet { get; set; }
    public decimal TotalGross { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;

    public decimal PaidAmount { get; set; }
    public DateOnly? PaidDate { get; set; }
    public byte[]? PdfData { get; set; }
    public int ReminderCount { get; set; }
    public DateTime? LastReminderDate { get; set; }

    public string? Notes { get; set; }
}
