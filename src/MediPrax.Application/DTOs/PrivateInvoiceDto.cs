using MediPrax.Core.Entities;
using MediPrax.Core.Enums;

namespace MediPrax.Application.DTOs;

public class PrivateInvoiceDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateOnly InvoiceDate { get; set; }
    public DateOnly DueDate { get; set; }
    public List<InvoiceItem> Items { get; set; } = [];
    public decimal TotalNet { get; set; }
    public decimal TotalGross { get; set; }
    public InvoiceStatus Status { get; set; }
    public decimal PaidAmount { get; set; }
    public DateOnly? PaidDate { get; set; }
    public int ReminderCount { get; set; }
    public DateTime? LastReminderDate { get; set; }
    public string? Notes { get; set; }

    public string StatusDisplay => Status switch
    {
        InvoiceStatus.Draft => "Entwurf",
        InvoiceStatus.Sent => "Versendet",
        InvoiceStatus.PartiallyPaid => "Teilzahlung",
        InvoiceStatus.Paid => "Bezahlt",
        InvoiceStatus.Overdue => "Überfällig",
        InvoiceStatus.Cancelled => "Storniert",
        _ => Status.ToString()
    };
}

public class CreatePrivateInvoiceDto
{
    public Guid PatientId { get; set; }
    public DateOnly InvoiceDate { get; set; }
    public DateOnly DueDate { get; set; }
    public List<InvoiceItem> Items { get; set; } = [];
    public string? Notes { get; set; }
}
