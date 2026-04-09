using MediPrax.Application.DTOs;

namespace MediPrax.Application.Interfaces;

public interface ISeizureDiaryService
{
    Task<IReadOnlyList<SeizureDiaryDto>> GetByPatientAsync(Guid patientId, CancellationToken ct = default);
    Task<SeizureDiaryDto> CreateAsync(CreateSeizureDiaryDto dto, CancellationToken ct = default);
    Task UpdateAsync(Guid id, CreateSeizureDiaryDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public interface IHeadacheDiaryService
{
    Task<IReadOnlyList<HeadacheDiaryDto>> GetByPatientAsync(Guid patientId, CancellationToken ct = default);
    Task<HeadacheDiaryDto> CreateAsync(CreateHeadacheDiaryDto dto, CancellationToken ct = default);
    Task UpdateAsync(Guid id, CreateHeadacheDiaryDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public interface IMsDocumentationService
{
    Task<IReadOnlyList<MsDocumentationDto>> GetByPatientAsync(Guid patientId, CancellationToken ct = default);
    Task<MsDocumentationDto> CreateAsync(CreateMsDocumentationDto dto, CancellationToken ct = default);
    Task UpdateAsync(Guid id, CreateMsDocumentationDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public interface IParkinsonDocumentationService
{
    Task<IReadOnlyList<ParkinsonDocumentationDto>> GetByPatientAsync(Guid patientId, CancellationToken ct = default);
    Task<ParkinsonDocumentationDto> CreateAsync(CreateParkinsonDocumentationDto dto, CancellationToken ct = default);
    Task UpdateAsync(Guid id, CreateParkinsonDocumentationDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
