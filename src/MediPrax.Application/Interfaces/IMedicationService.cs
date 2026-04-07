using MediPrax.Application.DTOs;

namespace MediPrax.Application.Interfaces;

public interface IMedicationService
{
    Task<IReadOnlyList<MedicationDto>> GetByPatientAsync(Guid patientId, bool activeOnly = true, CancellationToken ct = default);
    Task<MedicationDto> AddAsync(CreateMedicationDto dto, CancellationToken ct = default);
    Task DeactivateAsync(Guid id, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
