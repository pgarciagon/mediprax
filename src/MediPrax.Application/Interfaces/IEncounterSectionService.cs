using MediPrax.Application.DTOs;

namespace MediPrax.Application.Interfaces;

public interface IEncounterSectionService
{
    Task<EncounterSectionDto> CreateAsync(Guid encounterId, CreateEncounterSectionDto dto, CancellationToken ct = default);
    Task<IReadOnlyList<EncounterSectionDto>> GetByEncounterAsync(Guid encounterId, CancellationToken ct = default);
    Task<EncounterSectionDto> UpdateAsync(UpdateEncounterSectionDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid sectionId, CancellationToken ct = default);
    Task<string> GetCombinedTextAsync(Guid encounterId, CancellationToken ct = default);
    Task MigrateNotesToSectionsAsync(Guid encounterId, Guid? authorId = null, CancellationToken ct = default);
    Task SaveAllAsync(Guid encounterId, IReadOnlyList<CreateEncounterSectionDto> sections, CancellationToken ct = default);
}
