using MediPrax.Application.DTOs;

namespace MediPrax.Application.Interfaces;

public interface ILabResultService
{
    Task<LabResultDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<LabResultListItemDto>> GetByPatientAsync(Guid patientId, CancellationToken ct = default);
    Task<IReadOnlyList<LabResultListItemDto>> GetPendingResultsAsync(CancellationToken ct = default);
    Task<IReadOnlyList<LabResultListItemDto>> GetActionRequiredAsync(CancellationToken ct = default);
    Task<LabResultDto> CreateAsync(CreateLabResultDto dto, CancellationToken ct = default);
    Task<LabResultDto> AddValuesAsync(AddLabValuesDto dto, CancellationToken ct = default);
    Task MarkReviewedAsync(Guid id, Guid reviewedById, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
