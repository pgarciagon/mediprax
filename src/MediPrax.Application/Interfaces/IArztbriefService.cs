using MediPrax.Application.DTOs;

namespace MediPrax.Application.Interfaces;

public interface IArztbriefService
{
    Task<Guid> CreateAndGeneratePdfAsync(CreateArztbriefDto dto, CancellationToken ct = default);
    Task<byte[]?> GetPdfAsync(Guid documentId, CancellationToken ct = default);
    Task<IReadOnlyList<ArztbriefListItemDto>> GetByPatientAsync(Guid patientId, CancellationToken ct = default);
}
