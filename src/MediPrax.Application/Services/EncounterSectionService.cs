using MediPrax.Application.DTOs;
using MediPrax.Application.Interfaces;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace MediPrax.Application.Services;

public class EncounterSectionService(DbContext context) : IEncounterSectionService
{
    private DbSet<EncounterSection> Sections => context.Set<EncounterSection>();
    private DbSet<Encounter> Encounters => context.Set<Encounter>();

    public async Task<EncounterSectionDto> CreateAsync(Guid encounterId, CreateEncounterSectionDto dto, CancellationToken ct = default)
    {
        var maxOrder = await Sections
            .Where(s => s.EncounterId == encounterId)
            .MaxAsync(s => (int?)s.SortOrder, ct) ?? -1;

        var section = new EncounterSection
        {
            EncounterId = encounterId,
            SectionType = dto.SectionType,
            Content = dto.Content,
            SortOrder = maxOrder + 1,
            AuthorId = dto.AuthorId
        };

        Sections.Add(section);
        await context.SaveChangesAsync(ct);

        return MapToDto(section);
    }

    public async Task<IReadOnlyList<EncounterSectionDto>> GetByEncounterAsync(Guid encounterId, CancellationToken ct = default)
    {
        return await Sections
            .Include(s => s.Author)
            .Where(s => s.EncounterId == encounterId)
            .OrderBy(s => s.SortOrder)
            .Select(s => new EncounterSectionDto
            {
                Id = s.Id,
                EncounterId = s.EncounterId,
                SectionType = s.SectionType,
                Content = s.Content,
                SortOrder = s.SortOrder,
                AuthorId = s.AuthorId,
                AuthorName = s.Author != null ? s.Author.FirstName + " " + s.Author.LastName : null,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            })
            .ToListAsync(ct);
    }

    public async Task<EncounterSectionDto> UpdateAsync(UpdateEncounterSectionDto dto, CancellationToken ct = default)
    {
        var section = await Sections
            .FirstOrDefaultAsync(s => s.Id == dto.Id, ct)
            ?? throw new KeyNotFoundException($"EncounterSection {dto.Id} not found");

        section.Content = dto.Content;
        await context.SaveChangesAsync(ct);

        return MapToDto(section);
    }

    public async Task DeleteAsync(Guid sectionId, CancellationToken ct = default)
    {
        var section = await Sections
            .FirstOrDefaultAsync(s => s.Id == sectionId, ct)
            ?? throw new KeyNotFoundException($"EncounterSection {sectionId} not found");

        Sections.Remove(section); // Soft delete via SaveChangesAsync override
        await context.SaveChangesAsync(ct);
    }

    public async Task<string> GetCombinedTextAsync(Guid encounterId, CancellationToken ct = default)
    {
        var sections = await Sections
            .Where(s => s.EncounterId == encounterId)
            .OrderBy(s => s.SortOrder)
            .ToListAsync(ct);

        if (sections.Count == 0)
            return string.Empty;

        var parts = sections
            .Where(s => !string.IsNullOrWhiteSpace(s.Content))
            .Select(s => $"{GetSectionLabel(s.SectionType)}:\n{s.Content}");

        return string.Join("\n\n", parts);
    }

    public async Task MigrateNotesToSectionsAsync(Guid encounterId, Guid? authorId = null, CancellationToken ct = default)
    {
        var encounter = await Encounters.FirstOrDefaultAsync(e => e.Id == encounterId, ct)
            ?? throw new KeyNotFoundException($"Encounter {encounterId} not found");

        if (string.IsNullOrWhiteSpace(encounter.Notes))
            return;

        // Check if sections already exist
        var existingCount = await Sections.CountAsync(s => s.EncounterId == encounterId, ct);
        if (existingCount > 0)
            return;

        var parsed = ParseNotesIntoSections(encounter.Notes);

        int order = 0;
        foreach (var (sectionType, content) in parsed)
        {
            Sections.Add(new EncounterSection
            {
                EncounterId = encounterId,
                SectionType = sectionType,
                Content = content,
                SortOrder = order++,
                AuthorId = authorId
            });
        }

        // Clear the old Notes field
        encounter.Notes = null;
        await context.SaveChangesAsync(ct);
    }

    public async Task SaveAllAsync(Guid encounterId, IReadOnlyList<CreateEncounterSectionDto> sections, CancellationToken ct = default)
    {
        // Remove existing sections for this encounter
        var existing = await Sections
            .Where(s => s.EncounterId == encounterId)
            .ToListAsync(ct);

        if (existing.Count > 0)
            Sections.RemoveRange(existing);

        // Add new sections
        int order = 0;
        foreach (var dto in sections)
        {
            if (string.IsNullOrWhiteSpace(dto.Content))
                continue;

            Sections.Add(new EncounterSection
            {
                EncounterId = encounterId,
                SectionType = dto.SectionType,
                Content = dto.Content,
                SortOrder = order++,
                AuthorId = dto.AuthorId
            });
        }

        await context.SaveChangesAsync(ct);
    }

    public static List<(EncounterSectionType Type, string Content)> ParseNotesIntoSections(string notes)
    {
        var result = new List<(EncounterSectionType, string)>();
        var sectionHeaders = new Dictionary<string, EncounterSectionType>(StringComparer.OrdinalIgnoreCase)
        {
            ["Anamnese"] = EncounterSectionType.Anamnese,
            ["A:"] = EncounterSectionType.Anamnese,
            ["Befund"] = EncounterSectionType.Befund,
            ["B:"] = EncounterSectionType.Befund,
            ["Diagnose"] = EncounterSectionType.Diagnose,
            ["D:"] = EncounterSectionType.Diagnose,
            ["Diagnosen"] = EncounterSectionType.Diagnose,
            ["Therapie"] = EncounterSectionType.Therapie,
            ["T:"] = EncounterSectionType.Therapie,
            ["Procedere"] = EncounterSectionType.Procedere,
            ["P:"] = EncounterSectionType.Procedere,
            ["Prozedere"] = EncounterSectionType.Procedere,
            ["Sonstiges"] = EncounterSectionType.Sonstiges,
        };

        var lines = notes.Split('\n');
        EncounterSectionType? currentType = null;
        var currentContent = new List<string>();

        foreach (var line in lines)
        {
            var trimmed = line.Trim().TrimEnd(':');
            var matched = false;

            foreach (var (header, type) in sectionHeaders)
            {
                if (trimmed.Equals(header.TrimEnd(':'), StringComparison.OrdinalIgnoreCase)
                    || trimmed.Equals(header, StringComparison.OrdinalIgnoreCase))
                {
                    // Flush previous section
                    if (currentType.HasValue)
                    {
                        var text = string.Join("\n", currentContent).Trim();
                        if (!string.IsNullOrWhiteSpace(text))
                            result.Add((currentType.Value, text));
                    }

                    currentType = type;
                    currentContent.Clear();
                    matched = true;
                    break;
                }
            }

            if (!matched)
                currentContent.Add(line);
        }

        // Flush last section
        if (currentType.HasValue)
        {
            var text = string.Join("\n", currentContent).Trim();
            if (!string.IsNullOrWhiteSpace(text))
                result.Add((currentType.Value, text));
        }
        else
        {
            // No structured headers found — put everything in Sonstiges
            var text = string.Join("\n", currentContent).Trim();
            if (!string.IsNullOrWhiteSpace(text))
                result.Add((EncounterSectionType.Sonstiges, text));
        }

        return result;
    }

    private static string GetSectionLabel(EncounterSectionType type) => type switch
    {
        EncounterSectionType.Anamnese => "Anamnese",
        EncounterSectionType.Befund => "Befund",
        EncounterSectionType.Diagnose => "Diagnose",
        EncounterSectionType.Therapie => "Therapie",
        EncounterSectionType.Procedere => "Procedere",
        EncounterSectionType.Sonstiges => "Sonstiges",
        _ => type.ToString()
    };

    private static EncounterSectionDto MapToDto(EncounterSection s) => new()
    {
        Id = s.Id,
        EncounterId = s.EncounterId,
        SectionType = s.SectionType,
        Content = s.Content,
        SortOrder = s.SortOrder,
        AuthorId = s.AuthorId,
        AuthorName = s.Author != null ? s.Author.FirstName + " " + s.Author.LastName : null,
        CreatedAt = s.CreatedAt,
        UpdatedAt = s.UpdatedAt
    };
}
