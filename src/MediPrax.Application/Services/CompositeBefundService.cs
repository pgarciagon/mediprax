using MediPrax.Application.DTOs;
using MediPrax.Application.Interfaces;
using MediPrax.Core.Enums;

namespace MediPrax.Application.Services;

/// <summary>
/// Composes the full Befund from three independent data sources:
/// 1. PsychopathologicalFinding (AMDP) → NarrativeText
/// 2. NeurologicalExamination → NarrativeText
/// 3. EncounterSection (Befund type) → free-text Content
/// No data duplication — pure read-time composition.
/// </summary>
public class CompositeBefundService(
    IPsychopathFindingService psychopathService,
    INeurologicalExamService neuroService,
    IEncounterSectionService sectionService) : ICompositeBefundService
{
    public async Task<CompositeBefundDto> GetAsync(Guid encounterId, CancellationToken ct = default)
    {
        var dto = new CompositeBefundDto();

        // 1. AMDP narrative
        var psychFinding = await psychopathService.GetByEncounterAsync(encounterId, ct);
        if (psychFinding is not null)
        {
            dto.PsychopathNarrative = psychFinding.NarrativeText;
            dto.PsychopathFindingId = psychFinding.Id;
            dto.PsychopathAssessmentDate = psychFinding.AssessmentDate;
        }

        // 2. Neurological examination narrative
        var neuroExam = await neuroService.GetByEncounterAsync(encounterId, ct);
        if (neuroExam is not null)
        {
            dto.NeuroNarrative = neuroExam.NarrativeText;
            dto.NeuroExamId = neuroExam.Id;
            dto.NeuroExamDate = neuroExam.ExamDate;
        }

        // 3. Free-text Befund section (M43 ABDTP)
        var sections = await sectionService.GetByEncounterAsync(encounterId, ct);
        var befundSection = sections.FirstOrDefault(
            s => s.SectionType == EncounterSectionType.Befund);
        dto.FreitextBefund = befundSection?.Content;

        return dto;
    }
}
