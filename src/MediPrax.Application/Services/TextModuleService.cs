using MediPrax.Application.DTOs;
using MediPrax.Application.Interfaces;
using MediPrax.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediPrax.Application.Services;

public class TextModuleService(DbContext context) : ITextModuleService
{
    private DbSet<TextModule> TextModules => context.Set<TextModule>();
    private DbSet<Patient> Patients => context.Set<Patient>();

    public async Task<IReadOnlyList<TextModuleDto>> GetAllAsync(string? category = null, CancellationToken ct = default)
    {
        var query = TextModules.Include(t => t.CreatedBy).AsQueryable();
        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(t => t.Category == category);
        return await query.OrderBy(t => t.Category).ThenBy(t => t.Title)
            .Select(t => MapToDto(t))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<TextModuleDto>> SearchByShortcutAsync(string shortcut, CancellationToken ct = default)
    {
        return await TextModules
            .Include(t => t.CreatedBy)
            .Where(t => t.Shortcut.StartsWith(shortcut))
            .OrderBy(t => t.Shortcut)
            .Select(t => MapToDto(t))
            .ToListAsync(ct);
    }

    public async Task<TextModuleDto> CreateAsync(CreateTextModuleDto dto, CancellationToken ct = default)
    {
        var module = new TextModule
        {
            CreatedById = dto.CreatedById,
            Shortcut = dto.Shortcut,
            Title = dto.Title,
            Content = dto.Content,
            Category = dto.Category,
            IsGlobal = dto.IsGlobal
        };
        TextModules.Add(module);
        await context.SaveChangesAsync(ct);

        var created = await TextModules.Include(t => t.CreatedBy)
            .FirstAsync(t => t.Id == module.Id, ct);
        return MapToDto(created);
    }

    public async Task<TextModuleDto> UpdateAsync(Guid id, CreateTextModuleDto dto, CancellationToken ct = default)
    {
        var module = await TextModules.Include(t => t.CreatedBy)
            .FirstOrDefaultAsync(t => t.Id == id, ct)
            ?? throw new KeyNotFoundException("Textbaustein nicht gefunden.");

        module.Shortcut = dto.Shortcut;
        module.Title = dto.Title;
        module.Content = dto.Content;
        module.Category = dto.Category;
        module.IsGlobal = dto.IsGlobal;

        await context.SaveChangesAsync(ct);
        return MapToDto(module);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var module = await TextModules.FindAsync([id], ct)
            ?? throw new KeyNotFoundException("Textbaustein nicht gefunden.");
        TextModules.Remove(module);
        await context.SaveChangesAsync(ct);
    }

    public async Task<string> ResolveVariablesAsync(string content, Guid patientId, CancellationToken ct = default)
    {
        var patient = await Patients.FindAsync([patientId], ct);

        var result = content;
        result = result.Replace("{Datum}", DateTime.Today.ToString("dd.MM.yyyy"));

        if (patient is not null)
        {
            result = result.Replace("{Patient.Name}", patient.FullName);
            result = result.Replace("{Patient.Geburtsdatum}", patient.DateOfBirth.ToString("dd.MM.yyyy"));
            var age = DateTime.Today.Year - patient.DateOfBirth.Year;
            if (patient.DateOfBirth.DayOfYear > DateTime.Today.DayOfYear) age--;
            result = result.Replace("{Patient.Alter}", age.ToString());
        }

        return result;
    }

    private static TextModuleDto MapToDto(TextModule t) => new()
    {
        Id = t.Id,
        CreatedById = t.CreatedById,
        CreatedByName = t.CreatedBy?.FullName ?? string.Empty,
        Shortcut = t.Shortcut,
        Title = t.Title,
        Content = t.Content,
        Category = t.Category,
        IsGlobal = t.IsGlobal,
        UsageCount = t.UsageCount
    };
}
