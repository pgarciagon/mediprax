using MediPrax.Application.DTOs;
using MediPrax.Application.Interfaces;
using MediPrax.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediPrax.Application.Services;

public class HeadacheDiaryService(DbContext context) : IHeadacheDiaryService
{
    private DbSet<HeadacheDiary> Entries => context.Set<HeadacheDiary>();

    public async Task<IReadOnlyList<HeadacheDiaryDto>> GetByPatientAsync(Guid patientId, CancellationToken ct = default)
    {
        return await Entries
            .Include(e => e.Patient)
            .Where(e => e.PatientId == patientId)
            .OrderByDescending(e => e.Date)
            .Select(e => MapToDto(e))
            .ToListAsync(ct);
    }

    public async Task<HeadacheDiaryDto> CreateAsync(CreateHeadacheDiaryDto dto, CancellationToken ct = default)
    {
        var entry = new HeadacheDiary
        {
            PatientId = dto.PatientId,
            Date = dto.Date,
            StartTime = dto.StartTime,
            DurationHours = dto.DurationHours,
            Intensity = dto.Intensity,
            Type = dto.Type,
            Location = dto.Location,
            AuraPresent = dto.AuraPresent,
            Nausea = dto.Nausea,
            Photophobia = dto.Photophobia,
            Phonophobia = dto.Phonophobia,
            Triggers = dto.Triggers,
            MedicationTaken = dto.MedicationTaken,
            MedicationEffective = dto.MedicationEffective
        };

        Entries.Add(entry);
        await context.SaveChangesAsync(ct);

        var created = await Entries.Include(e => e.Patient).FirstAsync(e => e.Id == entry.Id, ct);
        return MapToDto(created);
    }

    public async Task UpdateAsync(Guid id, CreateHeadacheDiaryDto dto, CancellationToken ct = default)
    {
        var entry = await Entries.FindAsync([id], ct)
            ?? throw new KeyNotFoundException($"HeadacheDiary {id} not found");

        entry.Date = dto.Date;
        entry.StartTime = dto.StartTime;
        entry.DurationHours = dto.DurationHours;
        entry.Intensity = dto.Intensity;
        entry.Type = dto.Type;
        entry.Location = dto.Location;
        entry.AuraPresent = dto.AuraPresent;
        entry.Nausea = dto.Nausea;
        entry.Photophobia = dto.Photophobia;
        entry.Phonophobia = dto.Phonophobia;
        entry.Triggers = dto.Triggers;
        entry.MedicationTaken = dto.MedicationTaken;
        entry.MedicationEffective = dto.MedicationEffective;
        await context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entry = await Entries.FindAsync([id], ct)
            ?? throw new KeyNotFoundException($"HeadacheDiary {id} not found");
        Entries.Remove(entry);
        await context.SaveChangesAsync(ct);
    }

    private static HeadacheDiaryDto MapToDto(HeadacheDiary e) => new()
    {
        Id = e.Id,
        PatientId = e.PatientId,
        PatientName = e.Patient?.LastName + ", " + e.Patient?.FirstName ?? string.Empty,
        Date = e.Date,
        StartTime = e.StartTime,
        DurationHours = e.DurationHours,
        Intensity = e.Intensity,
        Type = e.Type,
        Location = e.Location,
        AuraPresent = e.AuraPresent,
        Nausea = e.Nausea,
        Photophobia = e.Photophobia,
        Phonophobia = e.Phonophobia,
        Triggers = e.Triggers,
        MedicationTaken = e.MedicationTaken,
        MedicationEffective = e.MedicationEffective
    };
}
