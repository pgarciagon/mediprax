using MediPrax.Application.DTOs;
using MediPrax.Application.Interfaces;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace MediPrax.Application.Services;

public class PatientDiagnosisService(DbContext context) : IPatientDiagnosisService
{
    private DbSet<PatientDiagnosis> Diagnoses => context.Set<PatientDiagnosis>();
    private DbSet<EncounterDiagnosis> EncounterDiagnoses => context.Set<EncounterDiagnosis>();
    private DbSet<Encounter> Encounters => context.Set<Encounter>();
    private DbSet<Icd10Code> Icd10Codes => context.Set<Icd10Code>();

    public async Task<PatientDiagnosisDto> CreateAsync(CreatePatientDiagnosisDto dto, CancellationToken ct = default)
    {
        // Look up description from catalog
        var description = await Icd10Codes
            .Where(c => c.Code == dto.Icd10Code)
            .Select(c => c.Description)
            .FirstOrDefaultAsync(ct) ?? dto.Icd10Code;

        var diagnosis = new PatientDiagnosis
        {
            PatientId = dto.PatientId,
            Icd10Code = dto.Icd10Code,
            Icd10Description = description,
            Certainty = dto.Certainty,
            Laterality = dto.Laterality,
            DiagnosisType = dto.DiagnosisType,
            Status = DiagnosisStatus.Active,
            OnsetDate = dto.OnsetDate,
            Notes = dto.Notes,
            CreatedByDoctorId = dto.CreatedByDoctorId
        };

        Diagnoses.Add(diagnosis);
        await context.SaveChangesAsync(ct);

        return MapToDto(diagnosis);
    }

    public async Task<PatientDiagnosisDto> UpdateAsync(UpdatePatientDiagnosisDto dto, CancellationToken ct = default)
    {
        var diagnosis = await Diagnoses
            .Include(d => d.CreatedByDoctor)
            .FirstOrDefaultAsync(d => d.Id == dto.Id, ct)
            ?? throw new KeyNotFoundException($"PatientDiagnosis {dto.Id} not found");

        diagnosis.Certainty = dto.Certainty;
        diagnosis.Laterality = dto.Laterality;
        diagnosis.DiagnosisType = dto.DiagnosisType;
        diagnosis.Status = dto.Status;
        diagnosis.ResolvedDate = dto.ResolvedDate;
        diagnosis.Notes = dto.Notes;

        await context.SaveChangesAsync(ct);
        return MapToDto(diagnosis);
    }

    public async Task<IReadOnlyList<PatientDiagnosisDto>> GetByPatientAsync(Guid patientId, bool includeInactive = false, CancellationToken ct = default)
    {
        var query = Diagnoses
            .Include(d => d.CreatedByDoctor)
            .Where(d => d.PatientId == patientId);

        if (!includeInactive)
            query = query.Where(d => d.Status != DiagnosisStatus.Inactive);

        return await query
            .OrderBy(d => d.DiagnosisType)
            .ThenBy(d => d.Icd10Code)
            .Select(d => new PatientDiagnosisDto
            {
                Id = d.Id,
                PatientId = d.PatientId,
                Icd10Code = d.Icd10Code,
                Icd10Description = d.Icd10Description,
                Certainty = d.Certainty,
                Laterality = d.Laterality,
                DiagnosisType = d.DiagnosisType,
                Status = d.Status,
                OnsetDate = d.OnsetDate,
                ResolvedDate = d.ResolvedDate,
                Notes = d.Notes,
                CreatedByDoctorId = d.CreatedByDoctorId,
                CreatedByDoctorName = d.CreatedByDoctor.FirstName + " " + d.CreatedByDoctor.LastName,
                CreatedAt = d.CreatedAt
            })
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<PatientDiagnosisDto>> GetDauerdiagnosenAsync(Guid patientId, CancellationToken ct = default)
    {
        return await Diagnoses
            .Include(d => d.CreatedByDoctor)
            .Where(d => d.PatientId == patientId
                     && d.DiagnosisType == DiagnosisType.Dauerdiagnose
                     && d.Status == DiagnosisStatus.Active)
            .OrderBy(d => d.Icd10Code)
            .Select(d => new PatientDiagnosisDto
            {
                Id = d.Id,
                PatientId = d.PatientId,
                Icd10Code = d.Icd10Code,
                Icd10Description = d.Icd10Description,
                Certainty = d.Certainty,
                Laterality = d.Laterality,
                DiagnosisType = d.DiagnosisType,
                Status = d.Status,
                OnsetDate = d.OnsetDate,
                Notes = d.Notes,
                CreatedByDoctorId = d.CreatedByDoctorId,
                CreatedByDoctorName = d.CreatedByDoctor.FirstName + " " + d.CreatedByDoctor.LastName,
                CreatedAt = d.CreatedAt
            })
            .ToListAsync(ct);
    }

    public async Task DeactivateAsync(Guid id, CancellationToken ct = default)
    {
        var diagnosis = await Diagnoses
            .FirstOrDefaultAsync(d => d.Id == id, ct)
            ?? throw new KeyNotFoundException($"PatientDiagnosis {id} not found");

        diagnosis.Status = DiagnosisStatus.Inactive;
        diagnosis.ResolvedDate = DateOnly.FromDateTime(DateTime.Today);
        await context.SaveChangesAsync(ct);
    }

    public async Task InheritDauerdiagnosenForEncounterAsync(Guid encounterId, CancellationToken ct = default)
    {
        var encounter = await Encounters.FirstOrDefaultAsync(e => e.Id == encounterId, ct)
            ?? throw new KeyNotFoundException($"Encounter {encounterId} not found");

        var dauerdiagnosen = await Diagnoses
            .Where(d => d.PatientId == encounter.PatientId
                     && d.DiagnosisType == DiagnosisType.Dauerdiagnose
                     && d.Status == DiagnosisStatus.Active)
            .ToListAsync(ct);

        var existingLinks = await EncounterDiagnoses
            .Where(ed => ed.EncounterId == encounterId)
            .Select(ed => ed.PatientDiagnosisId)
            .ToHashSetAsync(ct);

        foreach (var dd in dauerdiagnosen)
        {
            if (existingLinks.Contains(dd.Id))
                continue;

            EncounterDiagnoses.Add(new EncounterDiagnosis
            {
                EncounterId = encounterId,
                PatientDiagnosisId = dd.Id,
                IsNewInThisEncounter = false
            });
        }

        await context.SaveChangesAsync(ct);
    }

    public async Task MigrateLegacyCodesAsync(Guid patientId, Guid doctorId, CancellationToken ct = default)
    {
        // Get all encounters for this patient with legacy ICD codes
        var encounters = await Encounters
            .Where(e => e.PatientId == patientId)
            .OrderBy(e => e.EncounterDate)
            .ToListAsync(ct);

        // Count code frequency across encounters
        var codeFrequency = new Dictionary<string, int>();
        var codeFirstSeen = new Dictionary<string, DateOnly>();

        foreach (var enc in encounters)
        {
            foreach (var code in enc.Icd10Codes)
            {
                codeFrequency[code] = codeFrequency.GetValueOrDefault(code, 0) + 1;
                if (!codeFirstSeen.ContainsKey(code))
                    codeFirstSeen[code] = enc.EncounterDate;
            }
        }

        // Check which codes already have a PatientDiagnosis
        var existingCodes = await Diagnoses
            .Where(d => d.PatientId == patientId)
            .Select(d => d.Icd10Code)
            .ToHashSetAsync(ct);

        // Create PatientDiagnosis for each distinct code
        var newDiagnoses = new Dictionary<string, PatientDiagnosis>();
        foreach (var (code, count) in codeFrequency)
        {
            if (existingCodes.Contains(code))
                continue;

            var description = await Icd10Codes
                .Where(c => c.Code == code)
                .Select(c => c.Description)
                .FirstOrDefaultAsync(ct) ?? code;

            var diagnosis = new PatientDiagnosis
            {
                PatientId = patientId,
                Icd10Code = code,
                Icd10Description = description,
                Certainty = DiagnosisCertainty.G,
                DiagnosisType = count >= 3 ? DiagnosisType.Dauerdiagnose : DiagnosisType.Encounterdiagnose,
                Status = DiagnosisStatus.Active,
                OnsetDate = codeFirstSeen.GetValueOrDefault(code),
                CreatedByDoctorId = doctorId
            };

            Diagnoses.Add(diagnosis);
            newDiagnoses[code] = diagnosis;
        }

        await context.SaveChangesAsync(ct);

        // Also load existing diagnoses for linking
        var allDiagnoses = await Diagnoses
            .Where(d => d.PatientId == patientId)
            .ToDictionaryAsync(d => d.Icd10Code, ct);

        // Create EncounterDiagnosis links
        foreach (var enc in encounters)
        {
            var existingLinks = await EncounterDiagnoses
                .Where(ed => ed.EncounterId == enc.Id)
                .Select(ed => ed.PatientDiagnosisId)
                .ToHashSetAsync(ct);

            foreach (var code in enc.Icd10Codes)
            {
                if (allDiagnoses.TryGetValue(code, out var pd) && !existingLinks.Contains(pd.Id))
                {
                    EncounterDiagnoses.Add(new EncounterDiagnosis
                    {
                        EncounterId = enc.Id,
                        PatientDiagnosisId = pd.Id,
                        IsNewInThisEncounter = true
                    });
                }
            }
        }

        await context.SaveChangesAsync(ct);
    }

    private static PatientDiagnosisDto MapToDto(PatientDiagnosis d) => new()
    {
        Id = d.Id,
        PatientId = d.PatientId,
        Icd10Code = d.Icd10Code,
        Icd10Description = d.Icd10Description,
        Certainty = d.Certainty,
        Laterality = d.Laterality,
        DiagnosisType = d.DiagnosisType,
        Status = d.Status,
        OnsetDate = d.OnsetDate,
        ResolvedDate = d.ResolvedDate,
        Notes = d.Notes,
        CreatedByDoctorId = d.CreatedByDoctorId,
        CreatedByDoctorName = d.CreatedByDoctor != null
            ? d.CreatedByDoctor.FirstName + " " + d.CreatedByDoctor.LastName
            : null,
        CreatedAt = d.CreatedAt
    };
}
