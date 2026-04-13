using MediPrax.Application.DTOs;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace MediPrax.Application.Services;

public interface IPrivateInvoiceService
{
    Task<PrivateInvoiceDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<PrivateInvoiceDto> CreateAsync(CreatePrivateInvoiceDto dto, CancellationToken ct = default);
    Task<IReadOnlyList<PrivateInvoiceDto>> GetByPatientAsync(Guid patientId, CancellationToken ct = default);
    Task UpdateStatusAsync(Guid id, InvoiceStatus status, CancellationToken ct = default);
    Task MarkAsPaidAsync(Guid id, decimal amount, DateOnly paidDate, CancellationToken ct = default);
    Task<IReadOnlyList<PrivateInvoiceDto>> GetOverdueAsync(CancellationToken ct = default);
}

public class PrivateInvoiceService(DbContext context) : IPrivateInvoiceService
{
    private DbSet<PrivateInvoice> Invoices => context.Set<PrivateInvoice>();

    public async Task<PrivateInvoiceDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var invoice = await Invoices.Include(i => i.Patient)
            .FirstOrDefaultAsync(i => i.Id == id, ct);
        return invoice is null ? null : MapToDto(invoice);
    }

    public async Task<PrivateInvoiceDto> CreateAsync(CreatePrivateInvoiceDto dto, CancellationToken ct = default)
    {
        var number = await GenerateInvoiceNumberAsync(ct);
        var totalNet = dto.Items.Sum(i => i.UnitPrice * i.Quantity);
        var totalGross = dto.Items.Sum(i => i.Total);

        var invoice = new PrivateInvoice
        {
            PatientId = dto.PatientId,
            InvoiceNumber = number,
            InvoiceDate = dto.InvoiceDate,
            DueDate = dto.DueDate,
            Items = dto.Items,
            TotalNet = totalNet,
            TotalGross = totalGross,
            Status = InvoiceStatus.Draft,
            Notes = dto.Notes
        };
        Invoices.Add(invoice);
        await context.SaveChangesAsync(ct);

        var created = await Invoices.Include(i => i.Patient)
            .FirstAsync(i => i.Id == invoice.Id, ct);
        return MapToDto(created);
    }

    public async Task<IReadOnlyList<PrivateInvoiceDto>> GetByPatientAsync(Guid patientId, CancellationToken ct = default)
    {
        return await Invoices.Include(i => i.Patient)
            .Where(i => i.PatientId == patientId)
            .OrderByDescending(i => i.InvoiceDate)
            .Select(i => MapToDto(i))
            .ToListAsync(ct);
    }

    public async Task UpdateStatusAsync(Guid id, InvoiceStatus status, CancellationToken ct = default)
    {
        var invoice = await Invoices.FindAsync([id], ct)
            ?? throw new KeyNotFoundException("Rechnung nicht gefunden.");
        invoice.Status = status;
        await context.SaveChangesAsync(ct);
    }

    public async Task MarkAsPaidAsync(Guid id, decimal amount, DateOnly paidDate, CancellationToken ct = default)
    {
        var invoice = await Invoices.FindAsync([id], ct)
            ?? throw new KeyNotFoundException("Rechnung nicht gefunden.");
        invoice.PaidAmount = amount;
        invoice.PaidDate = paidDate;
        invoice.Status = amount >= invoice.TotalGross ? InvoiceStatus.Paid : InvoiceStatus.PartiallyPaid;
        await context.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<PrivateInvoiceDto>> GetOverdueAsync(CancellationToken ct = default)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        return await Invoices.Include(i => i.Patient)
            .Where(i => i.DueDate < today
                && i.Status != InvoiceStatus.Paid
                && i.Status != InvoiceStatus.Cancelled)
            .OrderBy(i => i.DueDate)
            .Select(i => MapToDto(i))
            .ToListAsync(ct);
    }

    private async Task<string> GenerateInvoiceNumberAsync(CancellationToken ct)
    {
        var year = DateTime.Today.Year;
        var count = await Invoices.CountAsync(i => i.InvoiceDate.Year == year, ct);
        return $"RE-{year}-{(count + 1):D4}";
    }

    private static PrivateInvoiceDto MapToDto(PrivateInvoice i) => new()
    {
        Id = i.Id,
        PatientId = i.PatientId,
        PatientName = i.Patient?.FullName ?? string.Empty,
        InvoiceNumber = i.InvoiceNumber,
        InvoiceDate = i.InvoiceDate,
        DueDate = i.DueDate,
        Items = i.Items,
        TotalNet = i.TotalNet,
        TotalGross = i.TotalGross,
        Status = i.Status,
        PaidAmount = i.PaidAmount,
        PaidDate = i.PaidDate,
        ReminderCount = i.ReminderCount,
        LastReminderDate = i.LastReminderDate,
        Notes = i.Notes
    };
}
