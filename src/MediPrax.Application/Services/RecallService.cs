using MediPrax.Application.DTOs;
using MediPrax.Application.Interfaces;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace MediPrax.Application.Services;

public class RecallService(DbContext context) : IRecallService
{
    private DbSet<Recall> Recalls => context.Set<Recall>();

    public async Task<IReadOnlyList<RecallDto>> GetByPatientAsync(Guid patientId, CancellationToken ct = default)
    {
        return await Recalls
            .Include(r => r.Patient).Include(r => r.CreatedBy)
            .Where(r => r.PatientId == patientId)
            .OrderBy(r => r.DueDate)
            .Select(r => MapToDto(r))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<RecallDto>> GetPendingAsync(CancellationToken ct = default)
    {
        return await Recalls
            .Include(r => r.Patient).Include(r => r.CreatedBy)
            .Where(r => r.Status == RecallStatus.Open || r.Status == RecallStatus.Scheduled)
            .OrderBy(r => r.DueDate)
            .Select(r => MapToDto(r))
            .ToListAsync(ct);
    }

    public async Task<RecallDto> CreateAsync(CreateRecallDto dto, CancellationToken ct = default)
    {
        var recall = new Recall
        {
            PatientId = dto.PatientId, CreatedById = dto.CreatedById,
            DueDate = dto.DueDate, Reason = dto.Reason, Notes = dto.Notes
        };
        Recalls.Add(recall);
        await context.SaveChangesAsync(ct);

        var created = await Recalls.Include(r => r.Patient).Include(r => r.CreatedBy)
            .FirstAsync(r => r.Id == recall.Id, ct);
        return MapToDto(created);
    }

    public async Task UpdateStatusAsync(Guid id, RecallStatus status, CancellationToken ct = default)
    {
        var recall = await Recalls.FindAsync([id], ct)
            ?? throw new KeyNotFoundException("Wiedervorlage nicht gefunden.");
        recall.Status = status;
        if (status == RecallStatus.Completed)
            recall.CompletedDate = DateOnly.FromDateTime(DateTime.Today);
        await context.SaveChangesAsync(ct);
    }

    private static RecallDto MapToDto(Recall r) => new()
    {
        Id = r.Id, PatientId = r.PatientId,
        PatientName = r.Patient.LastName + ", " + r.Patient.FirstName,
        DueDate = r.DueDate, Reason = r.Reason, Notes = r.Notes,
        Status = r.Status,
        CreatedByName = r.CreatedBy.FirstName + " " + r.CreatedBy.LastName
    };
}
