using MediPrax.Application.DTOs;
using MediPrax.Application.Interfaces;
using MediPrax.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediPrax.Application.Services;

public class Icd10CodeService(DbContext context) : IIcd10CodeService
{
    private DbSet<Icd10Code> Codes => context.Set<Icd10Code>();

    public async Task<IReadOnlyList<Icd10CodeDto>> SearchAsync(string term, int maxResults = 15, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(term) || term.Length < 2)
            return [];

        var lower = term.ToLower();
        return await Codes
            .Where(c => c.IsActive && (c.Code.ToLower().Contains(lower) || c.Description.ToLower().Contains(lower)))
            .OrderBy(c => c.Code)
            .Take(maxResults)
            .Select(c => MapToDto(c))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Icd10CodeDto>> GetByCategoryAsync(string category, CancellationToken ct = default)
    {
        return await Codes
            .Where(c => c.IsActive && c.Category == category)
            .OrderBy(c => c.Code)
            .Select(c => MapToDto(c))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Icd10CodeDto>> GetAllAsync(CancellationToken ct = default)
    {
        return await Codes
            .OrderBy(c => c.Code)
            .Select(c => MapToDto(c))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<string>> GetCategoriesAsync(CancellationToken ct = default)
    {
        return await Codes
            .Where(c => c.Category != null)
            .Select(c => c.Category!)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync(ct);
    }

    public async Task<string?> GetDescriptionAsync(string code, CancellationToken ct = default)
    {
        return await Codes
            .Where(c => c.Code == code)
            .Select(c => c.Description)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<Icd10CodeDto> CreateAsync(CreateIcd10CodeDto dto, CancellationToken ct = default)
    {
        var entity = new Icd10Code
        {
            Code = dto.Code.Trim().ToUpper(),
            Description = dto.Description.Trim(),
            Category = dto.Category?.Trim(),
            IsActive = true
        };
        Codes.Add(entity);
        await context.SaveChangesAsync(ct);
        return MapToDto(entity);
    }

    public async Task<Icd10CodeDto> UpdateAsync(UpdateIcd10CodeDto dto, CancellationToken ct = default)
    {
        var entity = await Codes.FindAsync([dto.Id], ct)
            ?? throw new KeyNotFoundException($"ICD-10 Code {dto.Id} not found");
        entity.Code = dto.Code.Trim().ToUpper();
        entity.Description = dto.Description.Trim();
        entity.Category = dto.Category?.Trim();
        entity.IsActive = dto.IsActive;
        await context.SaveChangesAsync(ct);
        return MapToDto(entity);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await Codes.FindAsync([id], ct)
            ?? throw new KeyNotFoundException($"ICD-10 Code {id} not found");
        Codes.Remove(entity);
        await context.SaveChangesAsync(ct);
    }

    public async Task<int> SeedFromCatalogAsync(CancellationToken ct = default)
    {
        var existing = await Codes.Select(c => c.Code).ToHashSetAsync(ct);
        var entries = GetSeedData().Where(e => !existing.Contains(e.Code)).ToList();

        if (entries.Count == 0) return 0;

        Codes.AddRange(entries);
        await context.SaveChangesAsync(ct);
        return entries.Count;
    }

    private static Icd10CodeDto MapToDto(Icd10Code c) => new()
    {
        Id = c.Id,
        Code = c.Code,
        Description = c.Description,
        Category = c.Category,
        IsActive = c.IsActive
    };

    private static List<Icd10Code> GetSeedData() =>
    [
        // Organische Störungen (F00-F09)
        new() { Code = "F00.0", Description = "Demenz bei Alzheimer-Krankheit, mit frühem Beginn", Category = "Organische Störungen" },
        new() { Code = "F00.1", Description = "Demenz bei Alzheimer-Krankheit, mit spätem Beginn", Category = "Organische Störungen" },
        new() { Code = "F01.0", Description = "Vaskuläre Demenz mit akutem Beginn", Category = "Organische Störungen" },
        new() { Code = "F01.1", Description = "Multiinfarkt-Demenz", Category = "Organische Störungen" },
        new() { Code = "F05.0", Description = "Delir ohne Demenz", Category = "Organische Störungen" },
        new() { Code = "F06.7", Description = "Leichte kognitive Störung", Category = "Organische Störungen" },

        // Störungen durch psychotrope Substanzen (F10-F19)
        new() { Code = "F10.0", Description = "Psychische Störungen durch Alkohol — Akute Intoxikation", Category = "Substanzstörungen" },
        new() { Code = "F10.1", Description = "Psychische Störungen durch Alkohol — Schädlicher Gebrauch", Category = "Substanzstörungen" },
        new() { Code = "F10.2", Description = "Psychische Störungen durch Alkohol — Abhängigkeitssyndrom", Category = "Substanzstörungen" },
        new() { Code = "F11.2", Description = "Psychische Störungen durch Opioide — Abhängigkeitssyndrom", Category = "Substanzstörungen" },
        new() { Code = "F12.1", Description = "Psychische Störungen durch Cannabis — Schädlicher Gebrauch", Category = "Substanzstörungen" },
        new() { Code = "F12.2", Description = "Psychische Störungen durch Cannabis — Abhängigkeitssyndrom", Category = "Substanzstörungen" },
        new() { Code = "F13.2", Description = "Psychische Störungen durch Sedativa — Abhängigkeitssyndrom", Category = "Substanzstörungen" },
        new() { Code = "F15.1", Description = "Psychische Störungen durch Stimulanzien — Schädlicher Gebrauch", Category = "Substanzstörungen" },
        new() { Code = "F15.2", Description = "Psychische Störungen durch Stimulanzien — Abhängigkeitssyndrom", Category = "Substanzstörungen" },
        new() { Code = "F17.1", Description = "Psychische Störungen durch Tabak — Schädlicher Gebrauch", Category = "Substanzstörungen" },
        new() { Code = "F19.1", Description = "Psychische Störungen durch multiplen Substanzgebrauch — Schädlicher Gebrauch", Category = "Substanzstörungen" },
        new() { Code = "F19.2", Description = "Psychische Störungen durch multiplen Substanzgebrauch — Abhängigkeitssyndrom", Category = "Substanzstörungen" },

        // Schizophrenie (F20-F29)
        new() { Code = "F20.0", Description = "Paranoide Schizophrenie", Category = "Schizophrenie" },
        new() { Code = "F20.1", Description = "Hebephrene Schizophrenie", Category = "Schizophrenie" },
        new() { Code = "F20.5", Description = "Schizophrenes Residuum", Category = "Schizophrenie" },
        new() { Code = "F22.0", Description = "Wahnhafte Störung", Category = "Schizophrenie" },
        new() { Code = "F23.0", Description = "Akute polymorphe psychotische Störung ohne Schizophrenie", Category = "Schizophrenie" },
        new() { Code = "F25.0", Description = "Schizoaffektive Störung, gegenwärtig manisch", Category = "Schizophrenie" },
        new() { Code = "F25.1", Description = "Schizoaffektive Störung, gegenwärtig depressiv", Category = "Schizophrenie" },

        // Affektive Störungen (F30-F39)
        new() { Code = "F30.0", Description = "Hypomanie", Category = "Affektive Störungen" },
        new() { Code = "F30.1", Description = "Manie ohne psychotische Symptome", Category = "Affektive Störungen" },
        new() { Code = "F31.0", Description = "Bipolare affektive Störung, gegenwärtig hypomanisch", Category = "Affektive Störungen" },
        new() { Code = "F31.3", Description = "Bipolare affektive Störung, gegenwärtig leichte depressive Episode", Category = "Affektive Störungen" },
        new() { Code = "F31.4", Description = "Bipolare affektive Störung, gegenwärtig schwere depressive Episode", Category = "Affektive Störungen" },
        new() { Code = "F32.0", Description = "Leichte depressive Episode", Category = "Affektive Störungen" },
        new() { Code = "F32.1", Description = "Mittelgradige depressive Episode", Category = "Affektive Störungen" },
        new() { Code = "F32.2", Description = "Schwere depressive Episode ohne psychotische Symptome", Category = "Affektive Störungen" },
        new() { Code = "F32.3", Description = "Schwere depressive Episode mit psychotischen Symptomen", Category = "Affektive Störungen" },
        new() { Code = "F33.0", Description = "Rezidivierende depressive Störung, leichte Episode", Category = "Affektive Störungen" },
        new() { Code = "F33.1", Description = "Rezidivierende depressive Störung, mittelgradige Episode", Category = "Affektive Störungen" },
        new() { Code = "F33.2", Description = "Rezidivierende depressive Störung, schwere Episode", Category = "Affektive Störungen" },
        new() { Code = "F33.3", Description = "Rezidivierende depressive Störung, schwere Episode mit psychotischen Symptomen", Category = "Affektive Störungen" },
        new() { Code = "F34.1", Description = "Dysthymia", Category = "Affektive Störungen" },

        // Neurotische Störungen (F40-F48)
        new() { Code = "F40.0", Description = "Agoraphobie", Category = "Neurotische Störungen" },
        new() { Code = "F40.1", Description = "Soziale Phobien", Category = "Neurotische Störungen" },
        new() { Code = "F40.2", Description = "Spezifische (isolierte) Phobien", Category = "Neurotische Störungen" },
        new() { Code = "F41.0", Description = "Panikstörung", Category = "Neurotische Störungen" },
        new() { Code = "F41.1", Description = "Generalisierte Angststörung", Category = "Neurotische Störungen" },
        new() { Code = "F41.2", Description = "Angst und depressive Störung, gemischt", Category = "Neurotische Störungen" },
        new() { Code = "F42.0", Description = "Vorwiegend Zwangsgedanken", Category = "Neurotische Störungen" },
        new() { Code = "F42.1", Description = "Vorwiegend Zwangshandlungen", Category = "Neurotische Störungen" },
        new() { Code = "F43.0", Description = "Akute Belastungsreaktion", Category = "Neurotische Störungen" },
        new() { Code = "F43.1", Description = "Posttraumatische Belastungsstörung", Category = "Neurotische Störungen" },
        new() { Code = "F43.2", Description = "Anpassungsstörungen", Category = "Neurotische Störungen" },
        new() { Code = "F44.0", Description = "Dissoziative Amnesie", Category = "Neurotische Störungen" },
        new() { Code = "F45.0", Description = "Somatisierungsstörung", Category = "Neurotische Störungen" },
        new() { Code = "F45.2", Description = "Hypochondrische Störung", Category = "Neurotische Störungen" },
        new() { Code = "F48.0", Description = "Neurasthenie", Category = "Neurotische Störungen" },

        // Verhaltensauffälligkeiten (F50-F59)
        new() { Code = "F50.0", Description = "Anorexia nervosa", Category = "Verhaltensauffälligkeiten" },
        new() { Code = "F50.2", Description = "Bulimia nervosa", Category = "Verhaltensauffälligkeiten" },
        new() { Code = "F51.0", Description = "Nichtorganische Insomnie", Category = "Verhaltensauffälligkeiten" },

        // Persönlichkeitsstörungen (F60-F69)
        new() { Code = "F60.0", Description = "Paranoide Persönlichkeitsstörung", Category = "Persönlichkeitsstörungen" },
        new() { Code = "F60.1", Description = "Schizoide Persönlichkeitsstörung", Category = "Persönlichkeitsstörungen" },
        new() { Code = "F60.2", Description = "Dissoziale Persönlichkeitsstörung", Category = "Persönlichkeitsstörungen" },
        new() { Code = "F60.3", Description = "Emotional instabile Persönlichkeitsstörung", Category = "Persönlichkeitsstörungen" },
        new() { Code = "F60.4", Description = "Histrionische Persönlichkeitsstörung", Category = "Persönlichkeitsstörungen" },
        new() { Code = "F60.5", Description = "Anankastische Persönlichkeitsstörung", Category = "Persönlichkeitsstörungen" },
        new() { Code = "F60.6", Description = "Ängstliche (vermeidende) Persönlichkeitsstörung", Category = "Persönlichkeitsstörungen" },
        new() { Code = "F60.7", Description = "Abhängige Persönlichkeitsstörung", Category = "Persönlichkeitsstörungen" },
        new() { Code = "F61", Description = "Kombinierte und sonstige Persönlichkeitsstörungen", Category = "Persönlichkeitsstörungen" },

        // Entwicklungsstörungen (F80-F89)
        new() { Code = "F84.0", Description = "Frühkindlicher Autismus", Category = "Entwicklungsstörungen" },
        new() { Code = "F84.5", Description = "Asperger-Syndrom", Category = "Entwicklungsstörungen" },

        // ADHS (F90)
        new() { Code = "F90.0", Description = "Einfache Aktivitäts- und Aufmerksamkeitsstörung", Category = "ADHS" },
        new() { Code = "F90.1", Description = "Hyperkinetische Störung des Sozialverhaltens", Category = "ADHS" },

        // Neurologie (G00-G99)
        new() { Code = "G20.0", Description = "Primäres Parkinson-Syndrom mit keiner oder geringer Beeinträchtigung", Category = "Neurologie" },
        new() { Code = "G20.1", Description = "Primäres Parkinson-Syndrom mit mäßiger bis schwerer Beeinträchtigung", Category = "Neurologie" },
        new() { Code = "G25.0", Description = "Essentieller Tremor", Category = "Neurologie" },
        new() { Code = "G30.0", Description = "Alzheimer-Krankheit mit frühem Beginn", Category = "Neurologie" },
        new() { Code = "G30.1", Description = "Alzheimer-Krankheit mit spätem Beginn", Category = "Neurologie" },
        new() { Code = "G35.0", Description = "Erstmanifestation einer Multiplen Sklerose", Category = "Neurologie" },
        new() { Code = "G35.1", Description = "Multiple Sklerose mit vorherrschend schubförmigem Verlauf", Category = "Neurologie" },
        new() { Code = "G40.0", Description = "Lokalisationsbezogene Epilepsie, idiopathisch", Category = "Neurologie" },
        new() { Code = "G40.2", Description = "Lokalisationsbezogene Epilepsie, symptomatisch", Category = "Neurologie" },
        new() { Code = "G43.0", Description = "Migräne ohne Aura", Category = "Neurologie" },
        new() { Code = "G43.1", Description = "Migräne mit Aura", Category = "Neurologie" },
        new() { Code = "G43.3", Description = "Komplizierte Migräne", Category = "Neurologie" },
        new() { Code = "G44.2", Description = "Spannungskopfschmerz", Category = "Neurologie" },
        new() { Code = "G47.3", Description = "Schlafapnoe", Category = "Neurologie" },
        new() { Code = "G50.0", Description = "Trigeminusneuralgie", Category = "Neurologie" },
        new() { Code = "G51.0", Description = "Fazialisparese", Category = "Neurologie" },
        new() { Code = "G62.1", Description = "Alkoholpolyneuropathie", Category = "Neurologie" },
        new() { Code = "G62.9", Description = "Polyneuropathie, nicht näher bezeichnet", Category = "Neurologie" },
        new() { Code = "G80.0", Description = "Spastische tetraplegische Zerebralparese", Category = "Neurologie" },
        new() { Code = "G93.3", Description = "Chronisches Müdigkeitssyndrom", Category = "Neurologie" },

        // Schlaganfall (I60-I69)
        new() { Code = "I63.0", Description = "Hirninfarkt durch Thrombose präzerebraler Arterien", Category = "Schlaganfall" },
        new() { Code = "I63.3", Description = "Hirninfarkt durch Thrombose zerebraler Arterien", Category = "Schlaganfall" },
        new() { Code = "I63.9", Description = "Hirninfarkt, nicht näher bezeichnet", Category = "Schlaganfall" },
        new() { Code = "I67.2", Description = "Zerebrale Atherosklerose", Category = "Schlaganfall" },

        // Symptome (R-Codes)
        new() { Code = "R41.3", Description = "Sonstige Amnesie", Category = "Symptome" },
        new() { Code = "R41.8", Description = "Sonstige Symptome, kognitive Funktionen betreffend", Category = "Symptome" },
        new() { Code = "R45.1", Description = "Ruhelosigkeit und Erregung", Category = "Symptome" },
        new() { Code = "R51", Description = "Kopfschmerz", Category = "Symptome" },
        new() { Code = "R56.0", Description = "Fieberkrämpfe", Category = "Symptome" },

        // Z-Codes (Faktoren)
        new() { Code = "Z73.0", Description = "Ausgebranntsein (Burn-out)", Category = "Faktoren" },
        new() { Code = "Z76.5", Description = "Malingering (Simulation)", Category = "Faktoren" },
    ];
}
