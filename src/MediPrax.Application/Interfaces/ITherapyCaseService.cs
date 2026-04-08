using MediPrax.Application.DTOs;

namespace MediPrax.Application.Interfaces;

public interface ITherapyCaseService
{
    Task<TherapyCaseDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<TherapyCaseListItemDto>> GetByPatientAsync(Guid patientId, CancellationToken ct = default);
    Task<IReadOnlyList<TherapyCaseListItemDto>> GetActiveByTherapistAsync(Guid therapistId, CancellationToken ct = default);
    Task<IReadOnlyList<TherapyCaseListItemDto>> GetAllActiveAsync(CancellationToken ct = default);
    Task<TherapyCaseDto> CreateAsync(CreateTherapyCaseDto dto, CancellationToken ct = default);
    Task<TherapyCaseDto> UpdateAsync(UpdateTherapyCaseDto dto, CancellationToken ct = default);
    Task<TherapySessionDto> AddSessionAsync(CreateTherapySessionDto dto, CancellationToken ct = default);
    Task<IReadOnlyList<TherapySessionDto>> GetSessionsAsync(Guid caseId, CancellationToken ct = default);
    Task<ContingentStatusDto> GetContingentStatusAsync(Guid caseId, CancellationToken ct = default);
    Task<PtvFormDto> GeneratePtvFormAsync(CreatePtvFormDto dto, CancellationToken ct = default);
    Task<IReadOnlyList<PtvFormDto>> GetPtvFormsAsync(Guid caseId, CancellationToken ct = default);
    Task<byte[]?> GetPtvFormPdfAsync(Guid formId, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
