using MediPrax.Application.DTOs;

namespace MediPrax.Application.Interfaces;

public interface IPatientDiagnosisService
{
    Task<PatientDiagnosisDto> CreateAsync(CreatePatientDiagnosisDto dto, CancellationToken ct = default);
    Task<PatientDiagnosisDto> UpdateAsync(UpdatePatientDiagnosisDto dto, CancellationToken ct = default);
    Task<IReadOnlyList<PatientDiagnosisDto>> GetByPatientAsync(Guid patientId, bool includeInactive = false, CancellationToken ct = default);
    Task<IReadOnlyList<PatientDiagnosisDto>> GetDauerdiagnosenAsync(Guid patientId, CancellationToken ct = default);
    Task DeactivateAsync(Guid id, CancellationToken ct = default);
    Task InheritDauerdiagnosenForEncounterAsync(Guid encounterId, CancellationToken ct = default);
    Task MigrateLegacyCodesAsync(Guid patientId, Guid doctorId, CancellationToken ct = default);
}
