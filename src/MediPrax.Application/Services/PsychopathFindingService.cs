using MediPrax.Application.DTOs;
using MediPrax.Application.Interfaces;
using MediPrax.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediPrax.Application.Services;

public class PsychopathFindingService(DbContext context) : IPsychopathFindingService
{
    private DbSet<PsychopathologicalFinding> Findings => context.Set<PsychopathologicalFinding>();

    public async Task<PsychopathFindingDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var f = await Findings
            .Include(x => x.Patient)
            .Include(x => x.AssessedBy)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        return f is null ? null : MapToDto(f);
    }

    public async Task<PsychopathFindingDto?> GetByEncounterAsync(Guid encounterId, CancellationToken ct = default)
    {
        var f = await Findings
            .Include(x => x.Patient)
            .Include(x => x.AssessedBy)
            .FirstOrDefaultAsync(x => x.EncounterId == encounterId, ct);

        return f is null ? null : MapToDto(f);
    }

    public async Task<IReadOnlyList<PsychopathFindingListItemDto>> GetByPatientAsync(Guid patientId, CancellationToken ct = default)
    {
        return await Findings
            .Include(x => x.AssessedBy)
            .Where(x => x.PatientId == patientId)
            .OrderByDescending(x => x.AssessmentDate)
            .Select(x => new PsychopathFindingListItemDto
            {
                Id = x.Id,
                AssessmentDate = x.AssessmentDate,
                AssessedByName = x.AssessedBy.FirstName + " " + x.AssessedBy.LastName,
                NarrativeTextPreview = x.NarrativeText != null
                    ? (x.NarrativeText.Length > 200 ? x.NarrativeText.Substring(0, 200) : x.NarrativeText)
                    : null
            })
            .ToListAsync(ct);
    }

    public async Task<PsychopathFindingDto> CreateAsync(CreatePsychopathFindingDto dto, CancellationToken ct = default)
    {
        var finding = new PsychopathologicalFinding
        {
            EncounterId = dto.EncounterId,
            PatientId = dto.PatientId,
            AssessedById = dto.AssessedById,
            AssessmentDate = dto.AssessmentDate,
            Findings = dto.Findings,
            NarrativeText = dto.NarrativeText,
            Notes = dto.Notes
        };

        Findings.Add(finding);
        await context.SaveChangesAsync(ct);

        var created = await Findings
            .Include(x => x.Patient)
            .Include(x => x.AssessedBy)
            .FirstAsync(x => x.Id == finding.Id, ct);

        return MapToDto(created);
    }

    public async Task<PsychopathFindingDto> UpdateAsync(UpdatePsychopathFindingDto dto, CancellationToken ct = default)
    {
        var finding = await Findings
            .Include(x => x.Patient)
            .Include(x => x.AssessedBy)
            .FirstOrDefaultAsync(x => x.Id == dto.Id, ct)
            ?? throw new KeyNotFoundException($"Psychopathologischer Befund {dto.Id} nicht gefunden.");

        finding.Findings = dto.Findings;
        finding.NarrativeText = dto.NarrativeText;
        finding.Notes = dto.Notes;
        finding.AssessmentDate = dto.AssessmentDate;

        await context.SaveChangesAsync(ct);
        return MapToDto(finding);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var finding = await Findings.FindAsync([id], ct)
            ?? throw new KeyNotFoundException("Psychopathologischer Befund nicht gefunden.");
        Findings.Remove(finding);
        await context.SaveChangesAsync(ct);
    }

    private static PsychopathFindingDto MapToDto(PsychopathologicalFinding f) => new()
    {
        Id = f.Id,
        EncounterId = f.EncounterId,
        PatientId = f.PatientId,
        AssessedByName = f.AssessedBy?.FirstName + " " + f.AssessedBy?.LastName,
        AssessmentDate = f.AssessmentDate,
        Findings = f.Findings,
        NarrativeText = f.NarrativeText,
        Notes = f.Notes
    };
}
