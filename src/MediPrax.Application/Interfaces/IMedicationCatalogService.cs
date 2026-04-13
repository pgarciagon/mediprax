using MediPrax.Application.DTOs;
using MediPrax.Core.Enums;

namespace MediPrax.Application.Interfaces;

public interface IMedicationCatalogService
{
    Task<IReadOnlyList<MedicationCatalogEntryDto>> SearchAsync(string term, int maxResults = 15, CancellationToken ct = default);
    Task<MedicationCatalogEntryDto?> GetByPznAsync(string pzn, CancellationToken ct = default);
    Task<IReadOnlyList<MedicationCatalogEntryDto>> GetByWirkstoffAsync(string wirkstoff, CancellationToken ct = default);
    Task<IReadOnlyList<MedicationCatalogEntryDto>> GetByAtcPrefixAsync(string atcPrefix, CancellationToken ct = default);
    Task<IReadOnlyList<MedicationCatalogEntryDto>> GetByCategoryAsync(MedicationCategory category, int maxResults = 50, CancellationToken ct = default);
    Task<CatalogStatisticsDto> GetStatisticsAsync(CancellationToken ct = default);
}
