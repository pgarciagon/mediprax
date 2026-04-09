using MediPrax.Application.DTOs;
using MediPrax.Application.Interfaces;
using MediPrax.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediPrax.Application.Services;

public class ParkinsonDocumentationService(DbContext context) : IParkinsonDocumentationService
{
    private DbSet<ParkinsonDocumentation> Entries => context.Set<ParkinsonDocumentation>();

    public async Task<IReadOnlyList<ParkinsonDocumentationDto>> GetByPatientAsync(Guid patientId, CancellationToken ct = default)
    {
        return await Entries
            .Include(e => e.Patient)
            .Where(e => e.PatientId == patientId)
            .OrderByDescending(e => e.DocumentationDate)
            .Select(e => MapToDto(e))
            .ToListAsync(ct);
    }

    public async Task<ParkinsonDocumentationDto> CreateAsync(CreateParkinsonDocumentationDto dto, CancellationToken ct = default)
    {
        var entry = new ParkinsonDocumentation
        {
            PatientId = dto.PatientId,
            DocumentationDate = dto.DocumentationDate,
            HoehnYahrStage = dto.HoehnYahrStage,
            Tremor = dto.Tremor,
            Rigidity = dto.Rigidity,
            Bradykinesia = dto.Bradykinesia,
            PosturalInstability = dto.PosturalInstability,
            OnOffFluctuations = dto.OnOffFluctuations,
            Dyskinesia = dto.Dyskinesia,
            FreezeOfGait = dto.FreezeOfGait,
            NonMotorSymptoms = dto.NonMotorSymptoms,
            Notes = dto.Notes
        };

        Entries.Add(entry);
        await context.SaveChangesAsync(ct);

        var created = await Entries.Include(e => e.Patient).FirstAsync(e => e.Id == entry.Id, ct);
        return MapToDto(created);
    }

    public async Task UpdateAsync(Guid id, CreateParkinsonDocumentationDto dto, CancellationToken ct = default)
    {
        var entry = await Entries.FindAsync([id], ct)
            ?? throw new KeyNotFoundException($"ParkinsonDocumentation {id} not found");

        entry.DocumentationDate = dto.DocumentationDate;
        entry.HoehnYahrStage = dto.HoehnYahrStage;
        entry.Tremor = dto.Tremor;
        entry.Rigidity = dto.Rigidity;
        entry.Bradykinesia = dto.Bradykinesia;
        entry.PosturalInstability = dto.PosturalInstability;
        entry.OnOffFluctuations = dto.OnOffFluctuations;
        entry.Dyskinesia = dto.Dyskinesia;
        entry.FreezeOfGait = dto.FreezeOfGait;
        entry.NonMotorSymptoms = dto.NonMotorSymptoms;
        entry.Notes = dto.Notes;
        await context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entry = await Entries.FindAsync([id], ct)
            ?? throw new KeyNotFoundException($"ParkinsonDocumentation {id} not found");
        Entries.Remove(entry);
        await context.SaveChangesAsync(ct);
    }

    private static ParkinsonDocumentationDto MapToDto(ParkinsonDocumentation e) => new()
    {
        Id = e.Id,
        PatientId = e.PatientId,
        PatientName = e.Patient?.LastName + ", " + e.Patient?.FirstName ?? string.Empty,
        DocumentationDate = e.DocumentationDate,
        HoehnYahrStage = e.HoehnYahrStage,
        Tremor = e.Tremor,
        Rigidity = e.Rigidity,
        Bradykinesia = e.Bradykinesia,
        PosturalInstability = e.PosturalInstability,
        OnOffFluctuations = e.OnOffFluctuations,
        Dyskinesia = e.Dyskinesia,
        FreezeOfGait = e.FreezeOfGait,
        NonMotorSymptoms = e.NonMotorSymptoms,
        Notes = e.Notes
    };
}
