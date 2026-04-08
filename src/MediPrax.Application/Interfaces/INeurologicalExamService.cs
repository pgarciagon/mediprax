using MediPrax.Application.DTOs;

namespace MediPrax.Application.Interfaces;

public interface INeurologicalExamService
{
    Task<NeurologicalExaminationDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<NeurologicalExaminationDto?> GetByEncounterAsync(Guid encounterId, CancellationToken ct = default);
    Task<IReadOnlyList<NeurologicalExamListItemDto>> GetByPatientAsync(Guid patientId, CancellationToken ct = default);
    Task<NeurologicalExaminationDto> CreateAsync(CreateNeurologicalExamDto dto, CancellationToken ct = default);
    Task<NeurologicalExaminationDto> UpdateAsync(UpdateNeurologicalExamDto dto, CancellationToken ct = default);
    Task<string> GenerateNarrativeAsync(CreateNeurologicalExamDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
