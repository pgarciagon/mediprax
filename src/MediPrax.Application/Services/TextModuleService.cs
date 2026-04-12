using System.Globalization;
using System.Text.Json;
using MediPrax.Application.DTOs;
using MediPrax.Application.Interfaces;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace MediPrax.Application.Services;

public class TextModuleService(
    DbContext context,
    IPatientDiagnosisService diagnosisService,
    IMedicationService medicationService,
    IPsychopathFindingService psychopathService,
    INeurologicalExamService neuroExamService) : ITextModuleService
{
    private DbSet<TextModule> TextModules => context.Set<TextModule>();
    private DbSet<Patient> Patients => context.Set<Patient>();
    private DbSet<User> Users => context.Set<User>();
    private DbSet<Encounter> Encounters => context.Set<Encounter>();

    public async Task<IReadOnlyList<TextModuleDto>> GetAllAsync(string? category = null, string? sortBy = null, CancellationToken ct = default)
    {
        var query = TextModules.Include(t => t.CreatedBy).AsQueryable();
        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(t => t.Category == category);

        query = sortBy switch
        {
            "usage" => query.OrderByDescending(t => t.UsageCount).ThenBy(t => t.Title),
            "recent" => query.OrderByDescending(t => t.LastUsedAt).ThenBy(t => t.Title),
            _ => query.OrderBy(t => t.Category).ThenBy(t => t.Title)
        };

        return await query.Select(t => MapToDto(t)).ToListAsync(ct);
    }

    public async Task<TextModuleDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var module = await TextModules.Include(t => t.CreatedBy)
            .FirstOrDefaultAsync(t => t.Id == id, ct);
        return module is null ? null : MapToDto(module);
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

    public async Task<IReadOnlyList<TextModuleDto>> SearchAsync(string term, EncounterSectionType? section = null, CancellationToken ct = default)
    {
        var query = TextModules
            .Include(t => t.CreatedBy)
            .AsQueryable();

        // When term is empty (user typed just '#'), show all available modules
        if (!string.IsNullOrEmpty(term))
            query = query.Where(t => t.Shortcut.Contains(term) || t.Title.Contains(term));

        if (section.HasValue)
            query = query.Where(t => t.TargetSection == null || t.TargetSection == section.Value);

        return await query
            .OrderByDescending(t => t.UsageCount)
            .ThenBy(t => t.Shortcut)
            .Take(15)
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
            IsGlobal = dto.IsGlobal,
            TargetSection = dto.TargetSection
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
        module.TargetSection = dto.TargetSection;

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

    public async Task IncrementUsageAsync(Guid id, CancellationToken ct = default)
    {
        var module = await TextModules.FindAsync([id], ct);
        if (module is null) return;
        module.UsageCount++;
        module.LastUsedAt = DateTime.UtcNow;
        await context.SaveChangesAsync(ct);
    }

    public async Task<string> ResolveVariablesAsync(string content, Guid patientId, Guid? encounterId = null, Guid? doctorId = null, CancellationToken ct = default)
    {
        var result = content;
        var deDE = new CultureInfo("de-DE");

        // Date variables — always available
        if (result.Contains("{Datum}"))
            result = result.Replace("{Datum}", DateTime.Today.ToString("dd.MM.yyyy"));
        if (result.Contains("{Datum.Lang}"))
            result = result.Replace("{Datum.Lang}", DateTime.Today.ToString("d. MMMM yyyy", deDE));

        // Patient variables
        if (result.Contains("{Patient."))
        {
            var patient = await Patients.FindAsync([patientId], ct);
            if (patient is not null)
            {
                result = result.Replace("{Patient.Name}", patient.FullName);
                result = result.Replace("{Patient.Geburtsdatum}", patient.DateOfBirth.ToString("dd.MM.yyyy"));
                var age = DateTime.Today.Year - patient.DateOfBirth.Year;
                if (patient.DateOfBirth.DayOfYear > DateTime.Today.DayOfYear) age--;
                result = result.Replace("{Patient.Alter}", age.ToString());
                result = result.Replace("{Patient.Geschlecht}", patient.Gender switch
                {
                    "M" or "m" or "male" or "maennlich" => "Herr",
                    "W" or "w" or "F" or "f" or "female" or "weiblich" => "Frau",
                    _ => patient.Gender ?? ""
                });
            }
        }

        // Doctor variables
        if (result.Contains("{Arzt.") && doctorId.HasValue)
        {
            var doctor = await Users.FindAsync([doctorId.Value], ct);
            if (doctor is not null)
            {
                result = result.Replace("{Arzt.Name}", doctor.FullName);
                result = result.Replace("{Arzt.Titel}", doctor.AcademicTitle ?? "");
            }
        }

        // Encounter variables
        if (result.Contains("{Encounter.") && encounterId.HasValue)
        {
            var encounter = await Encounters.FindAsync([encounterId.Value], ct);
            if (encounter is not null)
            {
                result = result.Replace("{Encounter.Datum}", encounter.EncounterDate.ToString("dd.MM.yyyy"));
                result = result.Replace("{Encounter.Dauer}", encounter.DurationMinutes?.ToString() ?? "");
            }
        }

        // Diagnosis variables — lazy-loaded only when needed
        if (result.Contains("{Diagnosen}"))
        {
            var diagnoses = await diagnosisService.GetByPatientAsync(patientId, false, ct);
            var active = diagnoses.Where(d => d.Status == DiagnosisStatus.Active);
            var text = string.Join("\n", active.Select(d => $"{d.Icd10Code} {d.Icd10Description}"));
            result = result.Replace("{Diagnosen}", text);
        }

        if (result.Contains("{Dauerdiagnosen}"))
        {
            var dauer = await diagnosisService.GetDauerdiagnosenAsync(patientId, ct);
            var text = string.Join("\n", dauer.Select(d => $"{d.Icd10Code} {d.Icd10Description}"));
            result = result.Replace("{Dauerdiagnosen}", text);
        }

        // Medication
        if (result.Contains("{Medikation}"))
        {
            var meds = await medicationService.GetByPatientAsync(patientId, true, ct);
            var text = string.Join("\n", meds.Select(m =>
            {
                var line = m.Name;
                if (!string.IsNullOrEmpty(m.Staerke)) line += $" {m.Staerke}";
                if (!string.IsNullOrEmpty(m.Dosierung)) line += $" — {m.Dosierung}";
                return line;
            }));
            result = result.Replace("{Medikation}", text);
        }

        // Last psychopathological finding
        if (result.Contains("{LetzterBefund.Psychopath}"))
        {
            var findings = await psychopathService.GetByPatientAsync(patientId, ct);
            var latest = findings.FirstOrDefault();
            result = result.Replace("{LetzterBefund.Psychopath}", latest?.NarrativeTextPreview ?? "");
        }

        // Last neurological exam
        if (result.Contains("{LetzterBefund.Neuro}"))
        {
            var exams = await neuroExamService.GetByPatientAsync(patientId, ct);
            var latest = exams.FirstOrDefault();
            result = result.Replace("{LetzterBefund.Neuro}", latest?.NarrativePreview ?? "");
        }

        return result;
    }

    public async Task<byte[]> ExportAsync(string? category = null, CancellationToken ct = default)
    {
        var query = TextModules.AsQueryable();
        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(t => t.Category == category);

        var modules = await query.OrderBy(t => t.Category).ThenBy(t => t.Title).ToListAsync(ct);

        var export = new TextModuleExportDto
        {
            Modules = modules.Select(m => new TextModuleExportItem
            {
                Shortcut = m.Shortcut,
                Title = m.Title,
                Content = m.Content,
                Category = m.Category,
                TargetSection = m.TargetSection?.ToString(),
                IsGlobal = m.IsGlobal
            }).ToList()
        };

        return JsonSerializer.SerializeToUtf8Bytes(export, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }

    public async Task<int> ImportAsync(byte[] jsonData, Guid createdById, CancellationToken ct = default)
    {
        var export = JsonSerializer.Deserialize<TextModuleExportDto>(jsonData, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        });

        if (export?.Modules is null or { Count: 0 })
            return 0;

        var count = 0;
        foreach (var item in export.Modules)
        {
            // Skip if shortcut already exists
            var exists = await TextModules.AnyAsync(t => t.Shortcut == item.Shortcut, ct);
            if (exists) continue;

            EncounterSectionType? targetSection = null;
            if (!string.IsNullOrEmpty(item.TargetSection) &&
                Enum.TryParse<EncounterSectionType>(item.TargetSection, out var parsed))
            {
                targetSection = parsed;
            }

            TextModules.Add(new TextModule
            {
                Shortcut = item.Shortcut,
                Title = item.Title,
                Content = item.Content,
                Category = item.Category,
                IsGlobal = item.IsGlobal,
                TargetSection = targetSection,
                CreatedById = createdById
            });
            count++;
        }

        if (count > 0)
            await context.SaveChangesAsync(ct);

        return count;
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
        UsageCount = t.UsageCount,
        TargetSection = t.TargetSection,
        LastUsedAt = t.LastUsedAt
    };
}
