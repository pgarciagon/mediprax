using MediPrax.Application.DTOs;

namespace MediPrax.Application.Interfaces;

public interface IMedicationMonitoringService
{
    Task<IReadOnlyList<MonitoringAlertDto>> GetOverdueMonitoringAsync(Guid? patientId = null, CancellationToken ct = default);
    Task<IReadOnlyList<MonitoringAlertDto>> GetPatientMonitoringAsync(Guid patientId, CancellationToken ct = default);
}
