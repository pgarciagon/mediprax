using MediPrax.Application.DTOs;

namespace MediPrax.Application.Interfaces;

public interface IPsychopathFindingService
{
    Task<PsychopathFindingDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<PsychopathFindingDto?> GetByEncounterAsync(Guid encounterId, CancellationToken ct = default);
    Task<IReadOnlyList<PsychopathFindingListItemDto>> GetByPatientAsync(Guid patientId, CancellationToken ct = default);
    Task<PsychopathFindingDto> CreateAsync(CreatePsychopathFindingDto dto, CancellationToken ct = default);
    Task<PsychopathFindingDto> UpdateAsync(UpdatePsychopathFindingDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
