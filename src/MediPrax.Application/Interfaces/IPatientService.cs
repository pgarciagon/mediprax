using MediPrax.Application.DTOs;

namespace MediPrax.Application.Interfaces;

public interface IPatientService
{
    Task<PatientDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<PagedResult<PatientListItemDto>> SearchAsync(PatientSearchDto search, CancellationToken ct = default);
    Task<PatientDto> CreateAsync(CreatePatientDto dto, CancellationToken ct = default);
    Task<PatientDto> UpdateAsync(UpdatePatientDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
