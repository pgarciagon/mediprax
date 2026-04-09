using MediPrax.Application.Catalogs;
using MediPrax.Application.DTOs;
using MediPrax.Application.Interfaces;
using MediPrax.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediPrax.Application.Services;

public class MedicationMonitoringService(DbContext context) : IMedicationMonitoringService
{
    private DbSet<Medication> Medications => context.Set<Medication>();

    public async Task<IReadOnlyList<MonitoringAlertDto>> GetOverdueMonitoringAsync(Guid? patientId = null, CancellationToken ct = default)
    {
        var query = Medications
            .Include(m => m.Patient)
            .Where(m => m.IsActive && m.RequiresMonitoring && m.MonitoringType != null);

        if (patientId.HasValue)
            query = query.Where(m => m.PatientId == patientId.Value);

        var medications = await query.ToListAsync(ct);
        return BuildAlerts(medications, overdueOnly: true);
    }

    public async Task<IReadOnlyList<MonitoringAlertDto>> GetPatientMonitoringAsync(Guid patientId, CancellationToken ct = default)
    {
        var medications = await Medications
            .Include(m => m.Patient)
            .Where(m => m.PatientId == patientId && m.IsActive && m.RequiresMonitoring && m.MonitoringType != null)
            .ToListAsync(ct);

        return BuildAlerts(medications, overdueOnly: false);
    }

    private static List<MonitoringAlertDto> BuildAlerts(List<Medication> medications, bool overdueOnly)
    {
        var alerts = new List<MonitoringAlertDto>();

        foreach (var med in medications)
        {
            var catalogAlerts = MedicationMonitoringCatalog.CheckOverdueMonitoring(
                med.MonitoringType!,
                lastCheckDate: null // We use SeitDatum as baseline — real implementation would track actual lab dates
            );

            foreach (var alert in catalogAlerts)
            {
                if (overdueOnly && !alert.IsOverdue)
                    continue;

                alerts.Add(new MonitoringAlertDto
                {
                    PatientId = med.PatientId,
                    PatientName = med.Patient.FullName,
                    MedicationName = med.Name,
                    MonitoringType = alert.MonitoringType,
                    Parameter = alert.Parameter,
                    IntervalDays = alert.IntervalDays,
                    LastCheckDate = null,
                    IsOverdue = alert.IsOverdue,
                    Message = alert.Message
                });
            }
        }

        return alerts
            .OrderByDescending(a => a.IsOverdue)
            .ThenBy(a => a.PatientName)
            .ToList();
    }
}
