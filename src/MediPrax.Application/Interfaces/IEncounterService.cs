using MediPrax.Application.DTOs;

namespace MediPrax.Application.Interfaces;

public interface IEncounterService
{
    Task<EncounterDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<EncounterListItemDto>> GetByPatientAsync(Guid patientId, CancellationToken ct = default);
    Task<EncounterDto> CreateAsync(CreateEncounterDto dto, CancellationToken ct = default);
    Task<EncounterDto> UpdateAsync(UpdateEncounterDto dto, CancellationToken ct = default);
}
