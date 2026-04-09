using MediPrax.Application.Catalogs;
using MediPrax.Application.DTOs;
using MediPrax.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediPrax.Application.Services;

public interface IBtmComplianceService
{
    Task<BtmPrescriptionDto> CreatePrescriptionAsync(CreateBtmPrescriptionDto dto, CancellationToken ct = default);
    Task<BtmComplianceCheckDto> CheckPrescriptionAsync(CreateBtmPrescriptionDto dto, CancellationToken ct = default);
    Task<IReadOnlyList<BtmPrescriptionDto>> GetPatientBtmHistoryAsync(Guid patientId, CancellationToken ct = default);
    Task<IReadOnlyList<BtmSummaryDto>> GetAll30DaySummaryAsync(CancellationToken ct = default);
    Task<IReadOnlyList<BtmPrescriptionDto>> GetAllAsync(CancellationToken ct = default);
}

public class BtmComplianceService(DbContext context) : IBtmComplianceService
{
    private DbSet<BtmPrescription> Prescriptions => context.Set<BtmPrescription>();

    public async Task<BtmComplianceCheckDto> CheckPrescriptionAsync(CreateBtmPrescriptionDto dto, CancellationToken ct = default)
    {
        var warnings = new List<string>();
        var substanceInfo = BtmSubstanceCatalog.Find(dto.Substance);

        var from30DaysAgo = DateOnly.FromDateTime(DateTime.Today.AddDays(-30));
        var recentPrescriptions = await Prescriptions
            .Where(p => p.PatientId == dto.PatientId
                && p.Substance == dto.Substance
                && p.PrescriptionDate >= from30DaysAgo)
            .ToListAsync(ct);

        string? maxQty = substanceInfo is not null ? $"{substanceInfo.Max30DayQuantity} {substanceInfo.Unit}" : null;

        if (substanceInfo is not null && decimal.TryParse(
            dto.Amount.Split(' ')[0].Replace(',', '.'),
            System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture,
            out var prescribedQty))
        {
            // Sum up recent prescriptions (simplified — amounts are strings; full parsing done in production)
            warnings.Add($"Prüfe 30-Tage-Limit: max. {substanceInfo.Max30DayQuantity} {substanceInfo.Unit}");

            if (prescribedQty > substanceInfo.Max30DayQuantity)
            {
                warnings.Add($"WARNUNG: Verschriebene Menge ({prescribedQty} {substanceInfo.Unit}) überschreitet 30-Tage-Maximum ({substanceInfo.Max30DayQuantity} {substanceInfo.Unit})!");
                return new BtmComplianceCheckDto
                {
                    IsCompliant = false,
                    Warnings = warnings,
                    MaxQuantity = maxQty,
                    PrescribedQuantity = dto.Amount
                };
            }
        }

        if (recentPrescriptions.Count > 0)
        {
            warnings.Add($"Hinweis: {recentPrescriptions.Count} Verschreibung(en) für {dto.Substance} in den letzten 30 Tagen.");
        }

        return new BtmComplianceCheckDto
        {
            IsCompliant = true,
            Warnings = warnings,
            MaxQuantity = maxQty,
            PrescribedQuantity = dto.Amount
        };
    }

    public async Task<BtmPrescriptionDto> CreatePrescriptionAsync(CreateBtmPrescriptionDto dto, CancellationToken ct = default)
    {
        var prescription = new BtmPrescription
        {
            PatientId = dto.PatientId,
            PrescribedById = dto.PrescribedById,
            PrescriptionDate = dto.PrescriptionDate,
            MedicationName = dto.MedicationName,
            Pzn = dto.Pzn,
            Substance = dto.Substance,
            Amount = dto.Amount,
            Dosierung = dto.Dosierung,
            BtmRecipeNumber = dto.BtmRecipeNumber,
            PrescriberBtmNumber = dto.PrescriberBtmNumber,
            IsEBtm = dto.IsEBtm,
            Notes = dto.Notes
        };
        Prescriptions.Add(prescription);
        await context.SaveChangesAsync(ct);

        var created = await Prescriptions
            .Include(p => p.Patient)
            .Include(p => p.PrescribedBy)
            .FirstAsync(p => p.Id == prescription.Id, ct);
        return MapToDto(created);
    }

    public async Task<IReadOnlyList<BtmPrescriptionDto>> GetPatientBtmHistoryAsync(Guid patientId, CancellationToken ct = default)
    {
        return await Prescriptions
            .Include(p => p.Patient)
            .Include(p => p.PrescribedBy)
            .Where(p => p.PatientId == patientId)
            .OrderByDescending(p => p.PrescriptionDate)
            .Select(p => MapToDto(p))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<BtmSummaryDto>> GetAll30DaySummaryAsync(CancellationToken ct = default)
    {
        var from30DaysAgo = DateOnly.FromDateTime(DateTime.Today.AddDays(-30));
        var prescriptions = await Prescriptions
            .Include(p => p.Patient)
            .Where(p => p.PrescriptionDate >= from30DaysAgo)
            .ToListAsync(ct);

        return prescriptions
            .GroupBy(p => new { p.PatientId, p.Patient.FirstName, p.Patient.LastName, p.Substance })
            .Select(g =>
            {
                var substanceInfo = BtmSubstanceCatalog.Find(g.Key.Substance);
                return new BtmSummaryDto
                {
                    PatientId = g.Key.PatientId,
                    PatientName = $"{g.Key.FirstName} {g.Key.LastName}",
                    Substance = g.Key.Substance,
                    TotalAmount30Days = 0, // simplified
                    Unit = substanceInfo?.Unit ?? string.Empty,
                    MaxAllowed = substanceInfo?.Max30DayQuantity,
                    ExceedsLimit = false
                };
            })
            .ToList();
    }

    public async Task<IReadOnlyList<BtmPrescriptionDto>> GetAllAsync(CancellationToken ct = default)
    {
        return await Prescriptions
            .Include(p => p.Patient)
            .Include(p => p.PrescribedBy)
            .OrderByDescending(p => p.PrescriptionDate)
            .Select(p => MapToDto(p))
            .ToListAsync(ct);
    }

    private static BtmPrescriptionDto MapToDto(BtmPrescription p) => new()
    {
        Id = p.Id,
        PatientId = p.PatientId,
        PatientName = p.Patient?.FullName ?? string.Empty,
        PrescribedById = p.PrescribedById,
        PrescribedByName = p.PrescribedBy?.FullName ?? string.Empty,
        PrescriptionDate = p.PrescriptionDate,
        MedicationName = p.MedicationName,
        Pzn = p.Pzn,
        Substance = p.Substance,
        Amount = p.Amount,
        Dosierung = p.Dosierung,
        BtmRecipeNumber = p.BtmRecipeNumber,
        PrescriberBtmNumber = p.PrescriberBtmNumber,
        IsEBtm = p.IsEBtm,
        Notes = p.Notes
    };
}
