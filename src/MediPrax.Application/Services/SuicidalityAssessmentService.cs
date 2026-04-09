using MediPrax.Application.DTOs;
using MediPrax.Application.Interfaces;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace MediPrax.Application.Services;

public class SuicidalityAssessmentService(DbContext context, IAuditService auditService) : ISuicidalityAssessmentService
{
    private DbSet<SuicidalityAssessment> Assessments => context.Set<SuicidalityAssessment>();
    private DbSet<Patient> Patients => context.Set<Patient>();

    public async Task<IReadOnlyList<SuicidalityAssessmentDto>> GetByPatientAsync(Guid patientId, CancellationToken ct = default)
    {
        return await Assessments
            .Include(a => a.Patient)
            .Include(a => a.AssessedBy)
            .Where(a => a.PatientId == patientId)
            .OrderByDescending(a => a.AssessmentDate)
            .Select(a => MapToDto(a))
            .ToListAsync(ct);
    }

    public async Task<SuicidalityAssessmentDto?> GetLatestByPatientAsync(Guid patientId, CancellationToken ct = default)
    {
        return await Assessments
            .Include(a => a.Patient)
            .Include(a => a.AssessedBy)
            .Where(a => a.PatientId == patientId)
            .OrderByDescending(a => a.AssessmentDate)
            .Select(a => MapToDto(a))
            .FirstOrDefaultAsync(ct);
    }

    public async Task<SuicidalityAssessmentDto> CreateAsync(CreateSuicidalityAssessmentDto dto, CancellationToken ct = default)
    {
        var assessment = new SuicidalityAssessment
        {
            PatientId = dto.PatientId,
            EncounterId = dto.EncounterId,
            AssessedById = Guid.Empty, // Set by caller or current user context
            AssessmentDate = DateOnly.FromDateTime(DateTime.Today),
            RiskLevel = dto.RiskLevel,
            SuicidalIdeation = dto.SuicidalIdeation,
            SuicidalPlans = dto.SuicidalPlans,
            SuicidalIntent = dto.SuicidalIntent,
            PriorAttempts = dto.PriorAttempts,
            PriorAttemptsDetails = dto.PriorAttemptsDetails,
            RiskFactors = dto.RiskFactors,
            ProtectiveFactors = dto.ProtectiveFactors,
            SafetyPlan = dto.SafetyPlan,
            ActionsTaken = dto.ActionsTaken,
            Notes = dto.Notes
        };

        Assessments.Add(assessment);

        // Update patient's current risk level
        var patient = await Patients.FindAsync([dto.PatientId], ct);
        if (patient is not null)
        {
            patient.CurrentSuicidalityRisk = dto.RiskLevel;
            patient.SuicidalityRiskUpdatedAt = DateTime.UtcNow;
        }

        await context.SaveChangesAsync(ct);
        await auditService.LogAsync(AuditAction.Create, "SuicidalityAssessment", assessment.Id,
            $"Suizidalitätseinschätzung erstellt: {dto.RiskLevel}");

        var created = await Assessments
            .Include(a => a.Patient)
            .Include(a => a.AssessedBy)
            .FirstAsync(a => a.Id == assessment.Id, ct);

        return MapToDto(created);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entry = await Assessments.FindAsync([id], ct)
            ?? throw new KeyNotFoundException($"SuicidalityAssessment {id} not found");
        Assessments.Remove(entry);
        await context.SaveChangesAsync(ct);
    }

    public async Task<SuicidalityRiskLevel?> GetCurrentRiskLevelAsync(Guid patientId, CancellationToken ct = default)
    {
        var patient = await Patients.FindAsync([patientId], ct);
        return patient?.CurrentSuicidalityRisk;
    }

    private static SuicidalityAssessmentDto MapToDto(SuicidalityAssessment a) => new()
    {
        Id = a.Id,
        PatientId = a.PatientId,
        PatientName = a.Patient?.LastName + ", " + a.Patient?.FirstName ?? string.Empty,
        EncounterId = a.EncounterId,
        AssessedByName = a.AssessedBy != null ? a.AssessedBy.FirstName + " " + a.AssessedBy.LastName : string.Empty,
        AssessmentDate = a.AssessmentDate,
        RiskLevel = a.RiskLevel,
        SuicidalIdeation = a.SuicidalIdeation,
        SuicidalPlans = a.SuicidalPlans,
        SuicidalIntent = a.SuicidalIntent,
        PriorAttempts = a.PriorAttempts,
        PriorAttemptsDetails = a.PriorAttemptsDetails,
        RiskFactors = a.RiskFactors,
        ProtectiveFactors = a.ProtectiveFactors,
        SafetyPlan = a.SafetyPlan,
        ActionsTaken = a.ActionsTaken,
        Notes = a.Notes
    };
}
