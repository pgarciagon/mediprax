using MediPrax.Application.DTOs;
using MediPrax.Core.Enums;

namespace MediPrax.Application.Interfaces;

public interface IRecallService
{
    Task<IReadOnlyList<RecallDto>> GetByPatientAsync(Guid patientId, CancellationToken ct = default);
    Task<IReadOnlyList<RecallDto>> GetPendingAsync(CancellationToken ct = default);
    Task<RecallDto> CreateAsync(CreateRecallDto dto, CancellationToken ct = default);
    Task UpdateStatusAsync(Guid id, RecallStatus status, CancellationToken ct = default);
}
