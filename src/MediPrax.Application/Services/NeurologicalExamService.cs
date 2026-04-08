using MediPrax.Application.DTOs;
using MediPrax.Application.Interfaces;
using MediPrax.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediPrax.Application.Services;

public class NeurologicalExamService(DbContext context) : INeurologicalExamService
{
    private DbSet<NeurologicalExamination> Exams => context.Set<NeurologicalExamination>();

    public async Task<NeurologicalExaminationDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var exam = await Exams
            .Include(x => x.ExaminedBy)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        return exam is null ? null : MapToDto(exam);
    }

    public async Task<NeurologicalExaminationDto?> GetByEncounterAsync(Guid encounterId, CancellationToken ct = default)
    {
        var exam = await Exams
            .Include(x => x.ExaminedBy)
            .FirstOrDefaultAsync(x => x.EncounterId == encounterId, ct);

        return exam is null ? null : MapToDto(exam);
    }

    public async Task<IReadOnlyList<NeurologicalExamListItemDto>> GetByPatientAsync(Guid patientId, CancellationToken ct = default)
    {
        return await Exams
            .Include(x => x.ExaminedBy)
            .Where(x => x.PatientId == patientId)
            .OrderByDescending(x => x.ExamDate)
            .Select(x => new NeurologicalExamListItemDto
            {
                Id = x.Id,
                ExamDate = x.ExamDate,
                ExaminedByName = x.ExaminedBy.FirstName + " " + x.ExaminedBy.LastName,
                NarrativePreview = x.NarrativeText != null
                    ? x.NarrativeText.Substring(0, Math.Min(x.NarrativeText.Length, 200))
                    : string.Empty
            })
            .ToListAsync(ct);
    }

    public async Task<NeurologicalExaminationDto> CreateAsync(CreateNeurologicalExamDto dto, CancellationToken ct = default)
    {
        var exam = new NeurologicalExamination
        {
            EncounterId = dto.EncounterId,
            PatientId = dto.PatientId,
            ExaminedById = dto.ExaminedById,
            ExamDate = dto.ExamDate,
            CranialNerves = dto.CranialNerves,
            MotorSystem = dto.MotorSystem,
            Reflexes = dto.Reflexes,
            SensorySystem = dto.SensorySystem,
            Coordination = dto.Coordination,
            Gait = dto.Gait,
            MeningealSigns = dto.MeningealSigns,
            NarrativeText = dto.NarrativeText ?? NeuroExamTextGenerator.GenerateFromDto(
                dto.CranialNerves, dto.MotorSystem, dto.Reflexes,
                dto.SensorySystem, dto.Coordination, dto.Gait, dto.MeningealSigns),
            Notes = dto.Notes
        };

        Exams.Add(exam);
        await context.SaveChangesAsync(ct);

        var created = await Exams
            .Include(x => x.ExaminedBy)
            .FirstAsync(x => x.Id == exam.Id, ct);

        return MapToDto(created);
    }

    public async Task<NeurologicalExaminationDto> UpdateAsync(UpdateNeurologicalExamDto dto, CancellationToken ct = default)
    {
        var exam = await Exams
            .Include(x => x.ExaminedBy)
            .FirstOrDefaultAsync(x => x.Id == dto.Id, ct)
            ?? throw new KeyNotFoundException($"Neurologischer Befund {dto.Id} nicht gefunden");

        exam.ExamDate = dto.ExamDate;
        exam.CranialNerves = dto.CranialNerves;
        exam.MotorSystem = dto.MotorSystem;
        exam.Reflexes = dto.Reflexes;
        exam.SensorySystem = dto.SensorySystem;
        exam.Coordination = dto.Coordination;
        exam.Gait = dto.Gait;
        exam.MeningealSigns = dto.MeningealSigns;
        exam.NarrativeText = dto.NarrativeText ?? NeuroExamTextGenerator.GenerateFromDto(
            dto.CranialNerves, dto.MotorSystem, dto.Reflexes,
            dto.SensorySystem, dto.Coordination, dto.Gait, dto.MeningealSigns);
        exam.Notes = dto.Notes;

        await context.SaveChangesAsync(ct);
        return MapToDto(exam);
    }

    public Task<string> GenerateNarrativeAsync(CreateNeurologicalExamDto dto, CancellationToken ct = default)
    {
        var text = NeuroExamTextGenerator.GenerateFromDto(
            dto.CranialNerves, dto.MotorSystem, dto.Reflexes,
            dto.SensorySystem, dto.Coordination, dto.Gait, dto.MeningealSigns);
        return Task.FromResult(text);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var exam = await Exams.FindAsync([id], ct)
            ?? throw new KeyNotFoundException($"Neurologischer Befund {id} nicht gefunden");
        Exams.Remove(exam);
        await context.SaveChangesAsync(ct);
    }

    private static NeurologicalExaminationDto MapToDto(NeurologicalExamination e) => new()
    {
        Id = e.Id,
        EncounterId = e.EncounterId,
        PatientId = e.PatientId,
        ExaminedByName = e.ExaminedBy?.FirstName + " " + e.ExaminedBy?.LastName,
        ExamDate = e.ExamDate,
        CranialNerves = e.CranialNerves,
        MotorSystem = e.MotorSystem,
        Reflexes = e.Reflexes,
        SensorySystem = e.SensorySystem,
        Coordination = e.Coordination,
        Gait = e.Gait,
        MeningealSigns = e.MeningealSigns,
        NarrativeText = e.NarrativeText,
        Notes = e.Notes
    };
}
