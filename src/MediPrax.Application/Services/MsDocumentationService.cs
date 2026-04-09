using MediPrax.Application.DTOs;
using MediPrax.Application.Interfaces;
using MediPrax.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediPrax.Application.Services;

public class MsDocumentationService(DbContext context) : IMsDocumentationService
{
    private DbSet<MsDocumentation> Entries => context.Set<MsDocumentation>();

    public async Task<IReadOnlyList<MsDocumentationDto>> GetByPatientAsync(Guid patientId, CancellationToken ct = default)
    {
        return await Entries
            .Include(e => e.Patient)
            .Where(e => e.PatientId == patientId)
            .OrderByDescending(e => e.DocumentationDate)
            .Select(e => MapToDto(e))
            .ToListAsync(ct);
    }

    public async Task<MsDocumentationDto> CreateAsync(CreateMsDocumentationDto dto, CancellationToken ct = default)
    {
        var entry = new MsDocumentation
        {
            PatientId = dto.PatientId,
            DocumentationDate = dto.DocumentationDate,
            EdssScore = dto.EdssScore,
            IsRelapse = dto.IsRelapse,
            RelapseDescription = dto.RelapseDescription,
            MriDate = dto.MriDate,
            MriFindings = dto.MriFindings,
            NewLesions = dto.NewLesions,
            GadEnhancing = dto.GadEnhancing,
            CurrentDmt = dto.CurrentDmt,
            Notes = dto.Notes
        };

        Entries.Add(entry);
        await context.SaveChangesAsync(ct);

        var created = await Entries.Include(e => e.Patient).FirstAsync(e => e.Id == entry.Id, ct);
        return MapToDto(created);
    }

    public async Task UpdateAsync(Guid id, CreateMsDocumentationDto dto, CancellationToken ct = default)
    {
        var entry = await Entries.FindAsync([id], ct)
            ?? throw new KeyNotFoundException($"MsDocumentation {id} not found");

        entry.DocumentationDate = dto.DocumentationDate;
        entry.EdssScore = dto.EdssScore;
        entry.IsRelapse = dto.IsRelapse;
        entry.RelapseDescription = dto.RelapseDescription;
        entry.MriDate = dto.MriDate;
        entry.MriFindings = dto.MriFindings;
        entry.NewLesions = dto.NewLesions;
        entry.GadEnhancing = dto.GadEnhancing;
        entry.CurrentDmt = dto.CurrentDmt;
        entry.Notes = dto.Notes;
        await context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entry = await Entries.FindAsync([id], ct)
            ?? throw new KeyNotFoundException($"MsDocumentation {id} not found");
        Entries.Remove(entry);
        await context.SaveChangesAsync(ct);
    }

    private static MsDocumentationDto MapToDto(MsDocumentation e) => new()
    {
        Id = e.Id,
        PatientId = e.PatientId,
        PatientName = e.Patient?.LastName + ", " + e.Patient?.FirstName ?? string.Empty,
        DocumentationDate = e.DocumentationDate,
        EdssScore = e.EdssScore,
        IsRelapse = e.IsRelapse,
        RelapseDescription = e.RelapseDescription,
        MriDate = e.MriDate,
        MriFindings = e.MriFindings,
        NewLesions = e.NewLesions,
        GadEnhancing = e.GadEnhancing,
        CurrentDmt = e.CurrentDmt,
        Notes = e.Notes
    };
}
