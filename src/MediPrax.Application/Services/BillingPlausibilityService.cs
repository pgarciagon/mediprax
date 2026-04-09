using MediPrax.Application.Catalogs;
using MediPrax.Application.Interfaces;
using MediPrax.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediPrax.Application.Services;

public class BillingPlausibilityService(DbContext context) : IBillingPlausibilityService
{
    private DbSet<BillingItem> BillingItems => context.Set<BillingItem>();
    private DbSet<Encounter> Encounters => context.Set<Encounter>();

    public async Task<IReadOnlyList<PlausibilityWarning>> CheckEncounterAsync(Guid encounterId, CancellationToken ct = default)
    {
        var encounter = await Encounters
            .Include(e => e.Patient)
            .Include(e => e.BillingItems)
            .FirstOrDefaultAsync(e => e.Id == encounterId, ct);

        if (encounter is null) return [];

        var warnings = new List<PlausibilityWarning>();
        var items = encounter.BillingItems.ToList();
        var patientName = encounter.Patient?.FullName;

        // Check 1: Exclusion rules (same encounter)
        var codes = items.Select(i => i.GopCode).ToList();
        for (var i = 0; i < codes.Count; i++)
        {
            for (var j = i + 1; j < codes.Count; j++)
            {
                var rule = GopExclusionCatalog.FindRule(codes[i], codes[j]);
                if (rule is not null)
                {
                    warnings.Add(new PlausibilityWarning
                    {
                        Severity = PlausibilitySeverity.Error,
                        CheckType = "Ausschluss",
                        GopCode = $"{codes[i]}/{codes[j]}",
                        Message = rule.Reason,
                        PatientName = patientName,
                        EncounterId = encounterId
                    });
                }
            }
        }

        // Check 2: Duplicate GOPs in same encounter
        var duplicates = items
            .GroupBy(i => i.GopCode)
            .Where(g => g.Count() > 1);

        foreach (var dup in duplicates)
        {
            warnings.Add(new PlausibilityWarning
            {
                Severity = PlausibilitySeverity.Warning,
                CheckType = "Duplikat",
                GopCode = dup.Key,
                Message = $"GOP {dup.Key} ist im selben Kontakt {dup.Count()}x abgerechnet.",
                PatientName = patientName,
                EncounterId = encounterId
            });
        }

        // Check 3: Time documentation for timed GOPs (16220, 21220 require DurationMinutes >= 10)
        var timedGops = new[] { "16220", "21220" };
        foreach (var gop in timedGops)
        {
            if (items.Any(i => i.GopCode.Equals(gop, StringComparison.OrdinalIgnoreCase)))
            {
                if (!encounter.DurationMinutes.HasValue || encounter.DurationMinutes.Value < 10)
                {
                    warnings.Add(new PlausibilityWarning
                    {
                        Severity = PlausibilitySeverity.Error,
                        CheckType = "Zeitdokumentation",
                        GopCode = gop,
                        Message = $"GOP {gop} erfordert eine Gesprächsdauer von mindestens 10 Minuten. Bitte Dauer dokumentieren.",
                        PatientName = patientName,
                        EncounterId = encounterId
                    });
                }
            }
        }

        return warnings;
    }

    public async Task<QuarterValidationResult> ValidateQuarterAsync(string quarter, CancellationToken ct = default)
    {
        var allItems = await BillingItems
            .Include(b => b.Patient)
            .Include(b => b.Encounter)
            .Where(b => b.Quarter == quarter)
            .ToListAsync(ct);

        var warnings = new List<PlausibilityWarning>();

        // Group by patient for per-patient checks
        var byPatient = allItems.GroupBy(b => b.PatientId);
        var patientCount = byPatient.Count();

        foreach (var patientGroup in byPatient)
        {
            var patientItems = patientGroup.ToList();
            var patientName = patientItems.First().Patient?.FullName;

            // Check quantity limits per patient per quarter
            var byGop = patientItems.GroupBy(i => i.GopCode);
            foreach (var gopGroup in byGop)
            {
                var limit = GopQuantityLimitCatalog.GetLimit(gopGroup.Key);
                if (limit is null) continue;

                var totalQty = gopGroup.Sum(i => i.Quantity);
                if (totalQty > limit.MaxPerQuarter)
                {
                    warnings.Add(new PlausibilityWarning
                    {
                        Severity = PlausibilitySeverity.Error,
                        CheckType = "Mengenbegrenzung",
                        GopCode = gopGroup.Key,
                        Message = $"GOP {gopGroup.Key} überschreitet das Quartalslimit: {totalQty}x abgerechnet, max. {limit.MaxPerQuarter}x erlaubt. ({limit.Description})",
                        PatientName = patientName,
                        EncounterId = gopGroup.First().EncounterId
                    });
                }

                // Check per-day limits
                if (limit.MaxPerDay.HasValue)
                {
                    var byDay = gopGroup.GroupBy(i => i.Encounter?.EncounterDate);
                    foreach (var dayGroup in byDay)
                    {
                        var dayQty = dayGroup.Sum(i => i.Quantity);
                        if (dayQty > limit.MaxPerDay.Value)
                        {
                            warnings.Add(new PlausibilityWarning
                            {
                                Severity = PlausibilitySeverity.Error,
                                CheckType = "Tageslimit",
                                GopCode = gopGroup.Key,
                                Message = $"GOP {gopGroup.Key} überschreitet das Tageslimit am {dayGroup.Key:dd.MM.yyyy}: {dayQty}x, max. {limit.MaxPerDay}x erlaubt.",
                                PatientName = patientName,
                                EncounterId = dayGroup.First().EncounterId
                            });
                        }
                    }
                }
            }

            // Check encounter-level exclusions and time documentation per encounter
            var byEncounter = patientItems.GroupBy(i => i.EncounterId);
            foreach (var encounterGroup in byEncounter)
            {
                var encCodes = encounterGroup.Select(i => i.GopCode).ToList();

                // Exclusion rules
                for (var i = 0; i < encCodes.Count; i++)
                {
                    for (var j = i + 1; j < encCodes.Count; j++)
                    {
                        var rule = GopExclusionCatalog.FindRule(encCodes[i], encCodes[j]);
                        if (rule is not null)
                        {
                            warnings.Add(new PlausibilityWarning
                            {
                                Severity = PlausibilitySeverity.Error,
                                CheckType = "Ausschluss",
                                GopCode = $"{encCodes[i]}/{encCodes[j]}",
                                Message = rule.Reason,
                                PatientName = patientName,
                                EncounterId = encounterGroup.Key
                            });
                        }
                    }
                }

                // Duplicate check
                var dups = encounterGroup.GroupBy(i => i.GopCode).Where(g => g.Count() > 1);
                foreach (var dup in dups)
                {
                    warnings.Add(new PlausibilityWarning
                    {
                        Severity = PlausibilitySeverity.Warning,
                        CheckType = "Duplikat",
                        GopCode = dup.Key,
                        Message = $"GOP {dup.Key} ist im selben Kontakt mehrfach abgerechnet.",
                        PatientName = patientName,
                        EncounterId = encounterGroup.Key
                    });
                }

                // Time documentation
                var encDuration = encounterGroup.First().Encounter?.DurationMinutes;
                var timedGops = new[] { "16220", "21220" };
                foreach (var gop in timedGops)
                {
                    if (encCodes.Contains(gop, StringComparer.OrdinalIgnoreCase))
                    {
                        if (!encDuration.HasValue || encDuration.Value < 10)
                        {
                            warnings.Add(new PlausibilityWarning
                            {
                                Severity = PlausibilitySeverity.Error,
                                CheckType = "Zeitdokumentation",
                                GopCode = gop,
                                Message = $"GOP {gop} erfordert Gesprächsdokumentation (mind. 10 Min.). Dauer fehlt oder zu kurz.",
                                PatientName = patientName,
                                EncounterId = encounterGroup.Key
                            });
                        }
                    }
                }
            }
        }

        return new QuarterValidationResult
        {
            Quarter = quarter,
            TotalPatients = patientCount,
            ErrorCount = warnings.Count(w => w.Severity == PlausibilitySeverity.Error),
            WarningCount = warnings.Count(w => w.Severity == PlausibilitySeverity.Warning),
            Warnings = warnings
        };
    }
}
