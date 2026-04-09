using MediPrax.Application.Catalogs;
using MediPrax.Application.DTOs;
using MediPrax.Application.Interfaces;
using MediPrax.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediPrax.Application.Services;

public class DrugInteractionService(DbContext context) : IDrugInteractionService
{
    private DbSet<Medication> Medications => context.Set<Medication>();

    public async Task<IReadOnlyList<InteractionWarningDto>> CheckPatientInteractionsAsync(Guid patientId, CancellationToken ct = default)
    {
        var substances = await GetActiveSubstancesAsync(patientId, ct);
        var warnings = DrugInteractionCatalog.CheckInteractions(substances);
        return MapToDto(warnings);
    }

    public async Task<IReadOnlyList<InteractionWarningDto>> CheckNewMedicationAsync(Guid patientId, string newWirkstoff, CancellationToken ct = default)
    {
        var substances = await GetActiveSubstancesAsync(patientId, ct);
        substances.Add(newWirkstoff);
        var warnings = DrugInteractionCatalog.CheckInteractions(substances);
        return MapToDto(warnings);
    }

    private async Task<List<string>> GetActiveSubstancesAsync(Guid patientId, CancellationToken ct)
    {
        return await Medications
            .Where(m => m.PatientId == patientId && m.IsActive && m.Wirkstoff != null)
            .Select(m => m.Wirkstoff!)
            .ToListAsync(ct);
    }

    private static List<InteractionWarningDto> MapToDto(IReadOnlyList<InteractionWarning> warnings)
    {
        return warnings.Select(w => new InteractionWarningDto
        {
            Substance1 = w.Substance1,
            Substance2 = w.Substance2,
            Severity = w.Severity.ToString(),
            InteractionType = w.Type.ToString(),
            Description = w.Description
        }).ToList();
    }
}
