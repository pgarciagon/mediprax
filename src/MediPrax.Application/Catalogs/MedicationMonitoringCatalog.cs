namespace MediPrax.Application.Catalogs;

public record MonitoringAlert(
    string MonitoringType,
    string Parameter,
    int IntervalDays,
    bool IsOverdue,
    string Message
);

/// <summary>
/// Wraps LabMonitoringCatalog with overdue-checking logic for medication monitoring.
/// </summary>
public static class MedicationMonitoringCatalog
{
    public static MonitoringPanel? GetMonitoringPanel(string monitoringType)
        => LabMonitoringCatalog.GetPanel(monitoringType);

    public static IReadOnlyList<string> GetAllMonitoringTypes()
        => LabMonitoringCatalog.AllPanels.Select(p => p.Name).ToList();

    public static IReadOnlyList<MonitoringAlert> CheckOverdueMonitoring(string monitoringType, DateOnly? lastCheckDate)
    {
        var panel = GetMonitoringPanel(monitoringType);
        if (panel is null)
            return [];

        var today = DateOnly.FromDateTime(DateTime.Today);
        var alerts = new List<MonitoringAlert>();

        foreach (var param in panel.Parameters)
        {
            bool isOverdue;
            string message;

            if (lastCheckDate is null)
            {
                isOverdue = true;
                message = $"{param.Name}: Noch nie kontrolliert. Kontrolle erforderlich.";
            }
            else
            {
                var daysSinceCheck = today.DayNumber - lastCheckDate.Value.DayNumber;
                isOverdue = daysSinceCheck >= param.IntervalDays;
                message = isOverdue
                    ? $"{param.Name}: Letzte Kontrolle vor {daysSinceCheck} Tagen (Intervall: {param.IntervalDays} Tage). Überfällig!"
                    : $"{param.Name}: Nächste Kontrolle in {param.IntervalDays - daysSinceCheck} Tagen.";
            }

            alerts.Add(new MonitoringAlert(monitoringType, param.Name, param.IntervalDays, isOverdue, message));
        }

        return alerts;
    }
}
