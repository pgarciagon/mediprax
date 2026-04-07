using MediPrax.Application.DTOs;

namespace MediPrax.Application.Interfaces;

public interface IBillingService
{
    Task<BillingItemDto> AddAsync(CreateBillingItemDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<BillingItemDto>> GetByEncounterAsync(Guid encounterId, CancellationToken ct = default);
    Task<QuarterOverviewDto> GetQuarterOverviewAsync(string quarter, CancellationToken ct = default);
    string GetCurrentQuarter();
}
