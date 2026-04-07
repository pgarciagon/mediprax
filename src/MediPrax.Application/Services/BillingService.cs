using MediPrax.Application.Data;
using MediPrax.Application.DTOs;
using MediPrax.Application.Interfaces;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace MediPrax.Application.Services;

public class BillingService(DbContext context) : IBillingService
{
    private DbSet<BillingItem> BillingItems => context.Set<BillingItem>();

    public async Task<BillingItemDto> AddAsync(CreateBillingItemDto dto, CancellationToken ct = default)
    {
        var gopEntry = GopCatalog.GetByCode(dto.GopCode);

        var item = new BillingItem
        {
            EncounterId = dto.EncounterId,
            PatientId = dto.PatientId,
            GopCode = dto.GopCode,
            GopDescription = gopEntry?.Description,
            Quantity = dto.Quantity,
            BillingType = gopEntry?.Type ?? dto.BillingType,
            Quarter = GetCurrentQuarter()
        };

        BillingItems.Add(item);
        await context.SaveChangesAsync(ct);

        return MapToDto(item);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var item = await BillingItems.FindAsync([id], ct)
            ?? throw new KeyNotFoundException($"BillingItem {id} nicht gefunden.");

        if (item.KvdtExported)
            throw new InvalidOperationException("Exportierte Abrechnungspositionen können nicht gelöscht werden.");

        BillingItems.Remove(item);
        await context.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<BillingItemDto>> GetByEncounterAsync(Guid encounterId, CancellationToken ct = default)
    {
        var items = await BillingItems
            .Where(b => b.EncounterId == encounterId)
            .OrderBy(b => b.CreatedAt)
            .ToListAsync(ct);

        return items.Select(MapToDto).ToList();
    }

    public async Task<QuarterOverviewDto> GetQuarterOverviewAsync(string quarter, CancellationToken ct = default)
    {
        var items = await BillingItems
            .Include(b => b.Patient)
            .Where(b => b.Quarter == quarter)
            .OrderBy(b => b.Patient.LastName)
            .ThenBy(b => b.CreatedAt)
            .ToListAsync(ct);

        var grouped = items
            .GroupBy(b => b.PatientId)
            .Select(g =>
            {
                var patientItems = g.Select(MapToDto).ToList();
                return new PatientQuarterDto
                {
                    PatientId = g.Key,
                    PatientName = g.First().Patient.LastName + ", " + g.First().Patient.FirstName,
                    Items = patientItems,
                    TotalAmount = patientItems.Sum(i => i.Amount),
                    ItemCount = patientItems.Count
                };
            })
            .ToList();

        return new QuarterOverviewDto
        {
            Quarter = quarter,
            Patients = grouped,
            TotalItems = items.Count,
            TotalAmount = grouped.Sum(p => p.TotalAmount)
        };
    }

    public string GetCurrentQuarter()
    {
        var now = DateTime.UtcNow;
        var q = (now.Month - 1) / 3 + 1;
        return $"{now.Year}-Q{q}";
    }

    private static BillingItemDto MapToDto(BillingItem b)
    {
        var amount = b.BillingType == BillingType.GOA
            ? GopCatalog.CalculateGoaeAmount(b.GopCode, 2.3m) * b.Quantity // Standard-Steigerungsfaktor 2,3
            : GopCatalog.CalculateEbmAmount(b.GopCode) * b.Quantity;

        return new BillingItemDto
        {
            Id = b.Id,
            EncounterId = b.EncounterId,
            PatientId = b.PatientId,
            GopCode = b.GopCode,
            GopDescription = b.GopDescription,
            Quantity = b.Quantity,
            BillingType = b.BillingType,
            Quarter = b.Quarter,
            KvdtExported = b.KvdtExported,
            Amount = amount
        };
    }
}
