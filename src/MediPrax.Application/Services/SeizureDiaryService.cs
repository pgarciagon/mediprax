using MediPrax.Application.DTOs;
using MediPrax.Application.Interfaces;
using MediPrax.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediPrax.Application.Services;

public class SeizureDiaryService(DbContext context) : ISeizureDiaryService
{
    private DbSet<SeizureDiary> Entries => context.Set<SeizureDiary>();

    public async Task<IReadOnlyList<SeizureDiaryDto>> GetByPatientAsync(Guid patientId, CancellationToken ct = default)
    {
        return await Entries
            .Include(e => e.Patient)
            .Where(e => e.PatientId == patientId)
            .OrderByDescending(e => e.SeizureDate)
            .Select(e => MapToDto(e))
            .ToListAsync(ct);
    }

    public async Task<SeizureDiaryDto> CreateAsync(CreateSeizureDiaryDto dto, CancellationToken ct = default)
    {
        var entry = new SeizureDiary
        {
            PatientId = dto.PatientId,
            SeizureDate = DateTime.SpecifyKind(dto.SeizureDate, DateTimeKind.Utc),
            SeizureType = dto.SeizureType,
            DurationSeconds = dto.DurationSeconds,
            Trigger = dto.Trigger,
            AuraPresent = dto.AuraPresent,
            AuraDescription = dto.AuraDescription,
            ConsciousnessImpaired = dto.ConsciousnessImpaired,
            PostictalState = dto.PostictalState,
            MedicationAtTime = dto.MedicationAtTime,
            Notes = dto.Notes
        };

        Entries.Add(entry);
        await context.SaveChangesAsync(ct);

        var created = await Entries.Include(e => e.Patient).FirstAsync(e => e.Id == entry.Id, ct);
        return MapToDto(created);
    }

    public async Task UpdateAsync(Guid id, CreateSeizureDiaryDto dto, CancellationToken ct = default)
    {
        var entry = await Entries.FindAsync([id], ct)
            ?? throw new KeyNotFoundException($"SeizureDiary {id} not found");

        entry.SeizureDate = DateTime.SpecifyKind(dto.SeizureDate, DateTimeKind.Utc);
        entry.SeizureType = dto.SeizureType;
        entry.DurationSeconds = dto.DurationSeconds;
        entry.Trigger = dto.Trigger;
        entry.AuraPresent = dto.AuraPresent;
        entry.AuraDescription = dto.AuraDescription;
        entry.ConsciousnessImpaired = dto.ConsciousnessImpaired;
        entry.PostictalState = dto.PostictalState;
        entry.MedicationAtTime = dto.MedicationAtTime;
        entry.Notes = dto.Notes;
        await context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entry = await Entries.FindAsync([id], ct)
            ?? throw new KeyNotFoundException($"SeizureDiary {id} not found");
        Entries.Remove(entry);
        await context.SaveChangesAsync(ct);
    }

    private static SeizureDiaryDto MapToDto(SeizureDiary e) => new()
    {
        Id = e.Id,
        PatientId = e.PatientId,
        PatientName = e.Patient?.LastName + ", " + e.Patient?.FirstName ?? string.Empty,
        SeizureDate = e.SeizureDate,
        SeizureType = e.SeizureType,
        DurationSeconds = e.DurationSeconds,
        Trigger = e.Trigger,
        AuraPresent = e.AuraPresent,
        AuraDescription = e.AuraDescription,
        ConsciousnessImpaired = e.ConsciousnessImpaired,
        PostictalState = e.PostictalState,
        MedicationAtTime = e.MedicationAtTime,
        Notes = e.Notes
    };
}
