using MediPrax.Application.DTOs;
using MediPrax.Core.Enums;

namespace MediPrax.Application.Interfaces;

public interface IArztbriefService
{
    Task<Guid> SaveDraftAsync(CreateArztbriefDto dto, CancellationToken ct = default);
    Task UpdateAsync(Guid id, CreateArztbriefDto dto, CancellationToken ct = default);
    Task<Guid> GeneratePdfAsync(Guid id, CancellationToken ct = default);
    Task UpdateStatusAsync(Guid id, ArztbriefStatus status, CancellationToken ct = default);
    Task<ArztbriefDetailDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<byte[]?> GetPdfAsync(Guid documentId, CancellationToken ct = default);
    Task<IReadOnlyList<ArztbriefListItemDto>> GetByPatientAsync(Guid patientId, CancellationToken ct = default);

    // Legacy: create + PDF in one step (kept for backward compatibility)
    Task<Guid> CreateAndGeneratePdfAsync(CreateArztbriefDto dto, CancellationToken ct = default);
}
