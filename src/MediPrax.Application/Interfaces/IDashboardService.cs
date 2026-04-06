using MediPrax.Application.DTOs;

namespace MediPrax.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardDto> GetDashboardAsync(CancellationToken ct = default);
}
