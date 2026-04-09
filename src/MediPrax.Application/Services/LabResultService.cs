using MediPrax.Application.DTOs;
using MediPrax.Application.Interfaces;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;
using MediPrax.Core.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace MediPrax.Application.Services;

public class LabResultService(DbContext context) : ILabResultService
{
    private DbSet<LabResult> LabResults => context.Set<LabResult>();

    public async Task<LabResultDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var lr = await LabResults
            .Include(l => l.Patient)
            .Include(l => l.OrderedBy)
            .FirstOrDefaultAsync(l => l.Id == id, ct);

        return lr is null ? null : MapToDto(lr);
    }

    public async Task<IReadOnlyList<LabResultListItemDto>> GetByPatientAsync(Guid patientId, CancellationToken ct = default)
    {
        return await LabResults
            .Where(l => l.PatientId == patientId)
            .OrderByDescending(l => l.OrderDate)
            .Select(l => MapToListItem(l))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<LabResultListItemDto>> GetPendingResultsAsync(CancellationToken ct = default)
    {
        return await LabResults
            .Where(l => l.Status == LabResultStatus.Ordered)
            .OrderBy(l => l.OrderDate)
            .Select(l => MapToListItem(l))
            .ToListAsync(ct);
    }

    public async Task<LabResultDto> CreateAsync(CreateLabResultDto dto, CancellationToken ct = default)
    {
        var labResult = new LabResult
        {
            PatientId = dto.PatientId,
            OrderedById = dto.OrderedById,
            LabName = dto.LabName,
            OrderDate = dto.OrderDate,
            Notes = dto.Notes,
            Status = LabResultStatus.Ordered,
            ImportSource = "Manual"
        };

        LabResults.Add(labResult);
        await context.SaveChangesAsync(ct);

        var created = await LabResults
            .Include(l => l.Patient)
            .Include(l => l.OrderedBy)
            .FirstAsync(l => l.Id == labResult.Id, ct);

        return MapToDto(created);
    }

    public async Task<LabResultDto> AddValuesAsync(AddLabValuesDto dto, CancellationToken ct = default)
    {
        var labResult = await LabResults
            .Include(l => l.Patient)
            .Include(l => l.OrderedBy)
            .FirstOrDefaultAsync(l => l.Id == dto.LabResultId, ct)
            ?? throw new KeyNotFoundException("Laborergebnis nicht gefunden.");

        // Auto-calculate abnormal and critical flags
        foreach (var value in dto.Values)
        {
            CalculateFlags(value);
        }

        labResult.Values = dto.Values;
        labResult.ResultDate = dto.ResultDate;
        labResult.Status = LabResultStatus.Received;

        await context.SaveChangesAsync(ct);

        return MapToDto(labResult);
    }

    public async Task MarkReviewedAsync(Guid id, Guid reviewedById, CancellationToken ct = default)
    {
        var labResult = await LabResults.FindAsync([id], ct)
            ?? throw new KeyNotFoundException("Laborergebnis nicht gefunden.");

        labResult.Status = LabResultStatus.Reviewed;
        labResult.ReviewedById = reviewedById;
        labResult.ReviewedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var labResult = await LabResults.FindAsync([id], ct)
            ?? throw new KeyNotFoundException("Laborergebnis nicht gefunden.");

        labResult.IsDeleted = true;
        labResult.DeletedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(ct);
    }

    private static void CalculateFlags(LabValue value)
    {
        if (!decimal.TryParse(value.Value, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var numericValue))
        {
            return;
        }

        bool abnormal = false;

        if (value.ReferenceRangeLow.HasValue && numericValue < value.ReferenceRangeLow.Value)
            abnormal = true;

        if (value.ReferenceRangeHigh.HasValue && numericValue > value.ReferenceRangeHigh.Value)
            abnormal = true;

        value.IsAbnormal = abnormal;

        // Critical if more than 50% outside reference range
        if (abnormal)
        {
            if (value.ReferenceRangeLow.HasValue && numericValue < value.ReferenceRangeLow.Value)
            {
                var deviation = value.ReferenceRangeLow.Value - numericValue;
                if (value.ReferenceRangeLow.Value != 0 && deviation / value.ReferenceRangeLow.Value > 0.5m)
                    value.IsCritical = true;
            }

            if (value.ReferenceRangeHigh.HasValue && numericValue > value.ReferenceRangeHigh.Value)
            {
                var deviation = numericValue - value.ReferenceRangeHigh.Value;
                if (value.ReferenceRangeHigh.Value != 0 && deviation / value.ReferenceRangeHigh.Value > 0.5m)
                    value.IsCritical = true;
            }
        }
    }

    private static string GetStatusDisplay(LabResultStatus status) => status switch
    {
        LabResultStatus.Ordered => "Angefordert",
        LabResultStatus.Received => "Eingegangen",
        LabResultStatus.Reviewed => "Zur Kenntnis genommen",
        _ => status.ToString()
    };

    private static LabResultDto MapToDto(LabResult lr) => new()
    {
        Id = lr.Id,
        PatientId = lr.PatientId,
        PatientName = lr.Patient.LastName + ", " + lr.Patient.FirstName,
        OrderedByName = lr.OrderedBy is not null ? lr.OrderedBy.FirstName + " " + lr.OrderedBy.LastName : null,
        LabName = lr.LabName,
        OrderDate = lr.OrderDate,
        ResultDate = lr.ResultDate,
        Status = lr.Status,
        StatusDisplay = GetStatusDisplay(lr.Status),
        IsReviewed = lr.Status == LabResultStatus.Reviewed,
        ReviewedByName = null, // Loaded separately if needed
        Values = lr.Values,
        Notes = lr.Notes,
        ImportSource = lr.ImportSource
    };

    private static LabResultListItemDto MapToListItem(LabResult lr) => new()
    {
        Id = lr.Id,
        OrderDate = lr.OrderDate,
        ResultDate = lr.ResultDate,
        LabName = lr.LabName,
        Status = lr.Status,
        StatusDisplay = GetStatusDisplay(lr.Status),
        ValueCount = lr.Values.Count,
        HasAbnormalValues = lr.Values.Any(v => v.IsAbnormal),
        HasCriticalValues = lr.Values.Any(v => v.IsCritical)
    };
}
