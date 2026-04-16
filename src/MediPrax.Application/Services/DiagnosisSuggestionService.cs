using MediPrax.Application.Catalogs;
using MediPrax.Application.Interfaces;
using MediPrax.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediPrax.Application.Services;

/// <summary>
/// Loads structured clinical findings for an encounter and generates
/// rule-based ICD-10 diagnosis suggestions via <see cref="DiagnosisSuggestionCatalog"/>.
/// </summary>
public class DiagnosisSuggestionService(DbContext context) : IDiagnosisSuggestionService
{
    private DbSet<PsychopathologicalFinding> PsychFindings => context.Set<PsychopathologicalFinding>();
    private DbSet<NeurologicalExamination> NeuroExams => context.Set<NeurologicalExamination>();
    private DbSet<SuicidalityAssessment> SuicAssessments => context.Set<SuicidalityAssessment>();

    public async Task<IReadOnlyList<DiagnosisSuggestion>> GetSuggestionsForEncounterAsync(
        Guid encounterId, Guid patientId, CancellationToken ct = default)
    {
        // Load psychopathological findings (AMDP) for this encounter
        var psychFinding = await PsychFindings
            .AsNoTracking()
            .Where(f => f.EncounterId == encounterId && !f.IsDeleted)
            .OrderByDescending(f => f.CreatedAt)
            .FirstOrDefaultAsync(ct);

        // Load neurological examination for this encounter
        var neuroExam = await NeuroExams
            .AsNoTracking()
            .Where(e => e.EncounterId == encounterId && !e.IsDeleted)
            .OrderByDescending(e => e.CreatedAt)
            .FirstOrDefaultAsync(ct);

        // Load latest suicidality assessment for this patient (not encounter-specific)
        var suicidality = await SuicAssessments
            .AsNoTracking()
            .Where(s => s.PatientId == patientId && !s.IsDeleted)
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync(ct);

        return DiagnosisSuggestionCatalog.SuggestAll(
            psychFinding?.Findings,
            neuroExam,
            suicidality);
    }
}
