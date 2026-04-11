using System.Text.Json;
using MediPrax.Application.Data;
using MediPrax.Application.DTOs;
using MediPrax.Application.Interfaces;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace MediPrax.Application.Services;

/// <summary>
/// Executes an ActionChain by processing each step in order,
/// calling existing services. Continues on failure and collects results.
/// </summary>
public class ActionChainExecutor(
    DbContext context,
    IActionChainService chainService,
    IPatientDiagnosisService diagnosisService,
    IBillingService billingService,
    IEncounterSectionService sectionService,
    IRecallService recallService
) : IActionChainExecutor
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<ActionChainResult> ExecuteAsync(Guid chainId, Guid encounterId, CancellationToken ct = default)
    {
        var chain = await chainService.GetByIdAsync(chainId, ct)
            ?? throw new InvalidOperationException("Aktionskette nicht gefunden.");

        if (!chain.IsActive)
            throw new InvalidOperationException("Aktionskette ist deaktiviert.");

        var encounter = await context.Set<Encounter>()
            .FirstOrDefaultAsync(e => e.Id == encounterId, ct)
            ?? throw new InvalidOperationException("Konsultation nicht gefunden.");

        var results = new List<ActionStepResult>();
        var warnings = new List<string>();

        foreach (var step in chain.Steps.OrderBy(s => s.SortOrder))
        {
            try
            {
                var result = step.StepType switch
                {
                    ActionStepType.AddDiagnosis => await HandleAddDiagnosis(step, encounter, ct),
                    ActionStepType.AddBillingCode => await HandleAddBillingCode(step, encounter, ct),
                    ActionStepType.SetNoteTemplate => await HandleSetNoteTemplate(step, encounter, ct),
                    ActionStepType.CreatePrescription => await HandleCreatePrescription(step, encounter, ct),
                    ActionStepType.SetDuration => await HandleSetDuration(step, encounter, ct),
                    ActionStepType.CreateRecall => await HandleCreateRecall(step, encounter, ct),
                    _ => new ActionStepResult(step.StepType, false, null, $"Schritttyp '{step.StepTypeDisplay}' wird noch nicht unterstützt.")
                };

                results.Add(result);
                if (!result.Success && result.ErrorMessage is not null)
                    warnings.Add($"{step.StepTypeDisplay}: {result.ErrorMessage}");
            }
            catch (Exception ex)
            {
                results.Add(new ActionStepResult(step.StepType, false, null, ex.Message));
                warnings.Add($"{step.StepTypeDisplay}: {ex.Message}");
            }
        }

        var executed = results.Count(r => r.Success);
        var failed = results.Count(r => !r.Success);

        return new ActionChainResult(
            Success: failed == 0,
            StepsExecuted: executed,
            StepsFailed: failed,
            Results: results,
            Warnings: warnings
        );
    }

    private async Task<ActionStepResult> HandleAddDiagnosis(ActionChainStepDto step, Encounter encounter, CancellationToken ct)
    {
        var config = JsonSerializer.Deserialize<DiagnosisConfig>(step.Configuration, JsonOptions);
        if (config is null || string.IsNullOrEmpty(config.Icd10Code))
            return new ActionStepResult(ActionStepType.AddDiagnosis, false, null, "Ungültige Konfiguration: icd10Code fehlt.");

        var certainty = Enum.TryParse<DiagnosisCertainty>(config.Certainty, true, out var c) ? c : DiagnosisCertainty.G;
        var diagType = Enum.TryParse<DiagnosisType>(config.DiagnosisType, true, out var dt) ? dt : DiagnosisType.Encounterdiagnose;

        await diagnosisService.CreateAsync(new CreatePatientDiagnosisDto
        {
            PatientId = encounter.PatientId,
            Icd10Code = config.Icd10Code,
            Certainty = certainty,
            DiagnosisType = diagType,
            CreatedByDoctorId = encounter.DoctorId
        }, ct);

        return new ActionStepResult(ActionStepType.AddDiagnosis, true, $"{config.Icd10Code} ({certainty})", null);
    }

    private async Task<ActionStepResult> HandleAddBillingCode(ActionChainStepDto step, Encounter encounter, CancellationToken ct)
    {
        var config = JsonSerializer.Deserialize<BillingConfig>(step.Configuration, JsonOptions);
        if (config is null || string.IsNullOrEmpty(config.GopCode))
            return new ActionStepResult(ActionStepType.AddBillingCode, false, null, "Ungültige Konfiguration: gopCode fehlt.");

        var gopEntry = GopCatalog.GetByCode(config.GopCode);
        var billingType = gopEntry?.Type ?? BillingType.EBM;

        await billingService.AddAsync(new CreateBillingItemDto
        {
            EncounterId = encounter.Id,
            PatientId = encounter.PatientId,
            GopCode = config.GopCode,
            Quantity = config.Quantity > 0 ? config.Quantity : 1,
            BillingType = billingType
        }, ct);

        var desc = gopEntry?.Description ?? config.GopCode;
        return new ActionStepResult(ActionStepType.AddBillingCode, true, $"{config.GopCode} — {desc}", null);
    }

    private async Task<ActionStepResult> HandleSetNoteTemplate(ActionChainStepDto step, Encounter encounter, CancellationToken ct)
    {
        var config = JsonSerializer.Deserialize<TemplateConfig>(step.Configuration, JsonOptions);
        if (config is null || string.IsNullOrEmpty(config.Template))
            return new ActionStepResult(ActionStepType.SetNoteTemplate, false, null, "Ungültige Konfiguration: template fehlt.");

        var templateSections = SectionTemplates.GetTemplate(config.Template);
        if (templateSections.Count == 0)
            return new ActionStepResult(ActionStepType.SetNoteTemplate, false, null, $"Vorlage '{config.Template}' nicht gefunden.");

        var sectionDtos = templateSections.Select(kvp => new CreateEncounterSectionDto
        {
            SectionType = kvp.Key,
            Content = kvp.Value,
            AuthorId = encounter.DoctorId
        }).ToList();

        await sectionService.SaveAllAsync(encounter.Id, sectionDtos, ct);

        var label = SectionTemplates.Available.FirstOrDefault(t => t.Key == config.Template).Label ?? config.Template;
        return new ActionStepResult(ActionStepType.SetNoteTemplate, true, $"Vorlage '{label}' angewendet", null);
    }

    private async Task<ActionStepResult> HandleCreatePrescription(ActionChainStepDto step, Encounter encounter, CancellationToken ct)
    {
        var config = JsonSerializer.Deserialize<PrescriptionConfig>(step.Configuration, JsonOptions);
        if (config is null || string.IsNullOrEmpty(config.MedicationName))
            return new ActionStepResult(ActionStepType.CreatePrescription, false, null, "Ungültige Konfiguration: medicationName fehlt.");

        var prescription = new Prescription
        {
            PatientId = encounter.PatientId,
            DoctorId = encounter.DoctorId,
            EncounterId = encounter.Id,
            MedicationName = config.MedicationName,
            MedicationPzn = config.MedicationPzn,
            Dosage = config.Dosage,
            IsBtm = config.IsBtm,
            Status = PrescriptionStatus.Draft
        };

        context.Set<Prescription>().Add(prescription);
        await context.SaveChangesAsync(ct);

        return new ActionStepResult(ActionStepType.CreatePrescription, true, $"{config.MedicationName} {config.Dosage}", null);
    }

    private async Task<ActionStepResult> HandleSetDuration(ActionChainStepDto step, Encounter encounter, CancellationToken ct)
    {
        var config = JsonSerializer.Deserialize<DurationConfig>(step.Configuration, JsonOptions);
        if (config is null || config.DurationMinutes <= 0)
            return new ActionStepResult(ActionStepType.SetDuration, false, null, "Ungültige Konfiguration: durationMinutes fehlt.");

        encounter.DurationMinutes = config.DurationMinutes;
        await context.SaveChangesAsync(ct);

        return new ActionStepResult(ActionStepType.SetDuration, true, $"{config.DurationMinutes} min", null);
    }

    private async Task<ActionStepResult> HandleCreateRecall(ActionChainStepDto step, Encounter encounter, CancellationToken ct)
    {
        var config = JsonSerializer.Deserialize<RecallConfig>(step.Configuration, JsonOptions);
        if (config is null || string.IsNullOrEmpty(config.Reason))
            return new ActionStepResult(ActionStepType.CreateRecall, false, null, "Ungültige Konfiguration: reason fehlt.");

        var dueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(config.DaysFromNow > 0 ? config.DaysFromNow : 90));

        await recallService.CreateAsync(new CreateRecallDto
        {
            PatientId = encounter.PatientId,
            CreatedById = encounter.DoctorId,
            DueDate = dueDate,
            Reason = config.Reason
        }, ct);

        return new ActionStepResult(ActionStepType.CreateRecall, true, $"{config.Reason} am {dueDate:dd.MM.yyyy}", null);
    }

    // --- Configuration JSON models ---

    private record DiagnosisConfig(string? Icd10Code, string? Certainty, string? DiagnosisType);
    private record BillingConfig(string? GopCode, int Quantity);
    private record TemplateConfig(string? Template);
    private record PrescriptionConfig(string? MedicationName, string? MedicationPzn, string? Dosage, bool IsBtm);
    private record DurationConfig(int DurationMinutes);
    private record RecallConfig(string? Reason, int DaysFromNow);
}
