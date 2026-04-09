using MediPrax.Application.Data;
using MediPrax.Application.Interfaces;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace MediPrax.Application.Services;

public class GopSuggestionService(DbContext context) : IGopSuggestionService
{
    private DbSet<Encounter> Encounters => context.Set<Encounter>();
    private DbSet<BillingItem> BillingItems => context.Set<BillingItem>();

    public async Task<IReadOnlyList<GopSuggestion>> GetSuggestionsAsync(Guid encounterId, CancellationToken ct = default)
    {
        var encounter = await Encounters
            .Include(e => e.Patient)
            .Include(e => e.BillingItems)
            .FirstOrDefaultAsync(e => e.Id == encounterId, ct);

        if (encounter is null) return [];

        var suggestions = new List<GopSuggestion>();
        var existingCodes = encounter.BillingItems.Select(b => b.GopCode).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var icdCodes = encounter.Icd10Codes ?? [];

        // Determine specialty from existing GOPs or ICD codes
        var hasNeurologyCodes = existingCodes.Any(c => c.StartsWith("16", StringComparison.Ordinal));
        var hasPsychiatryCodes = existingCodes.Any(c => c.StartsWith("21", StringComparison.Ordinal));
        var hasFCodes = icdCodes.Any(c => c.StartsWith("F", StringComparison.OrdinalIgnoreCase));
        var hasGCodes = icdCodes.Any(c => c.StartsWith("G", StringComparison.OrdinalIgnoreCase));

        // Check if this is the first encounter of the quarter for this patient
        var currentQuarter = GetCurrentQuarter();
        var quarterItems = await BillingItems
            .Where(b => b.PatientId == encounter.PatientId && b.Quarter == currentQuarter && b.EncounterId != encounterId)
            .ToListAsync(ct);

        var hasQuarterGrundpauschale = quarterItems.Any(b =>
            b.GopCode.StartsWith("162", StringComparison.Ordinal) ||
            b.GopCode.StartsWith("212", StringComparison.Ordinal));

        // Suggest Grundpauschale if first encounter in quarter
        if (!hasQuarterGrundpauschale)
        {
            if (hasGCodes || hasNeurologyCodes)
            {
                var age = DateTime.Today.Year - encounter.Patient.DateOfBirth.Year;
                var grundCode = age >= 60 ? "16211" : "16210";
                var entry = GopCatalog.GetByCode(grundCode);
                if (entry is not null && !existingCodes.Contains(grundCode))
                {
                    suggestions.Add(new GopSuggestion
                    {
                        GopCode = grundCode,
                        Description = entry.Description,
                        Reason = "Erster Kontakt im Quartal — Neurologie-Grundpauschale empfohlen",
                        BillingType = BillingType.EBM
                    });
                }
            }

            if (hasFCodes || hasPsychiatryCodes)
            {
                var age = DateTime.Today.Year - encounter.Patient.DateOfBirth.Year;
                var grundCode = age >= 60 ? "21211" : "21210";
                var entry = GopCatalog.GetByCode(grundCode);
                if (entry is not null && !existingCodes.Contains(grundCode))
                {
                    suggestions.Add(new GopSuggestion
                    {
                        GopCode = grundCode,
                        Description = entry.Description,
                        Reason = "Erster Kontakt im Quartal — Psychiatrie-Grundpauschale empfohlen",
                        BillingType = BillingType.EBM
                    });
                }
            }
        }

        // Suggest timed GOPs if encounter duration >= 10 minutes
        if (encounter.DurationMinutes.HasValue && encounter.DurationMinutes.Value >= 10)
        {
            if ((hasGCodes || hasNeurologyCodes) && !existingCodes.Contains("16220"))
            {
                var entry = GopCatalog.GetByCode("16220");
                if (entry is not null)
                {
                    suggestions.Add(new GopSuggestion
                    {
                        GopCode = "16220",
                        Description = entry.Description,
                        Reason = $"Gesprächsdauer {encounter.DurationMinutes} Min. — Neurologisches Gespräch abrechenbar",
                        BillingType = BillingType.EBM
                    });
                }
            }

            if ((hasFCodes || hasPsychiatryCodes) && !existingCodes.Contains("21220"))
            {
                var entry = GopCatalog.GetByCode("21220");
                if (entry is not null)
                {
                    suggestions.Add(new GopSuggestion
                    {
                        GopCode = "21220",
                        Description = entry.Description,
                        Reason = $"Gesprächsdauer {encounter.DurationMinutes} Min. — Psychiatrisches Gespräch abrechenbar",
                        BillingType = BillingType.EBM
                    });
                }
            }
        }

        // Suggest specialty-specific codes based on ICD codes
        if (hasFCodes)
        {
            // Psychiatry-specific suggestions
            if (!existingCodes.Contains("21213"))
            {
                var entry = GopCatalog.GetByCode("21213");
                if (entry is not null)
                {
                    suggestions.Add(new GopSuggestion
                    {
                        GopCode = "21213",
                        Description = entry.Description,
                        Reason = "ICD F-Diagnose — Psychiatrische Zusatzpauschale prüfen",
                        BillingType = BillingType.EBM
                    });
                }
            }
        }

        if (hasGCodes)
        {
            // Neurology-specific suggestions
            if (!existingCodes.Contains("16230"))
            {
                var entry = GopCatalog.GetByCode("16230");
                if (entry is not null)
                {
                    suggestions.Add(new GopSuggestion
                    {
                        GopCode = "16230",
                        Description = entry.Description,
                        Reason = "ICD G-Diagnose — Neurologische Untersuchung prüfen",
                        BillingType = BillingType.EBM
                    });
                }
            }
        }

        return suggestions;
    }

    private static string GetCurrentQuarter()
    {
        var now = DateTime.UtcNow;
        var q = (now.Month - 1) / 3 + 1;
        return $"{now.Year}-Q{q}";
    }
}
