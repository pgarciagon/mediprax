namespace MediPrax.Application.Catalogs;

public record MonitoringParameter(string Name, int IntervalDays);

public record MonitoringPanel(
    string MonitoringType,
    string Description,
    IReadOnlyList<MonitoringParameter> Parameters
);

public record MonitoringAlert(
    string MonitoringType,
    string Parameter,
    int IntervalDays,
    bool IsOverdue,
    string Message
);

public static class MedicationMonitoringCatalog
{
    private static readonly Dictionary<string, MonitoringPanel> Panels = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Lithium"] = new MonitoringPanel(
            "Lithium",
            "Lithium-Monitoring: Spiegelkontrolle und Organfunktion",
            [
                new MonitoringParameter("Lithiumspiegel", 90),
                new MonitoringParameter("TSH", 90),
                new MonitoringParameter("Kreatinin", 90),
                new MonitoringParameter("GFR", 90)
            ]),

        ["Clozapin"] = new MonitoringPanel(
            "Clozapin",
            "Clozapin-Monitoring: Blutbildkontrolle (Agranulozytose-Risiko)",
            [
                new MonitoringParameter("Leukozyten", 7),   // first 18 weeks weekly, then monthly
                new MonitoringParameter("Neutrophile", 7)
            ]),

        ["Valproat"] = new MonitoringPanel(
            "Valproat",
            "Valproat-Monitoring: Spiegel, Leber und Blutbild",
            [
                new MonitoringParameter("Valproatspiegel", 90),
                new MonitoringParameter("Leberwerte", 180),
                new MonitoringParameter("Blutbild", 180)
            ]),

        ["Carbamazepin"] = new MonitoringPanel(
            "Carbamazepin",
            "Carbamazepin-Monitoring: Spiegel, Blutbild und Elektrolyte",
            [
                new MonitoringParameter("Carbamazepinspiegel", 90),
                new MonitoringParameter("Blutbild", 180),
                new MonitoringParameter("Natrium", 180),
                new MonitoringParameter("Leberwerte", 180)
            ]),

        ["Metabolic"] = new MonitoringPanel(
            "Metabolic",
            "Metabolisches Monitoring bei atypischen Antipsychotika",
            [
                new MonitoringParameter("Gewicht", 90),
                new MonitoringParameter("Glucose", 90),
                new MonitoringParameter("HbA1c", 180),
                new MonitoringParameter("Lipide", 180)
            ])
    };

    /// <summary>
    /// Returns the monitoring panel for a given monitoring type, or null if unknown.
    /// </summary>
    public static MonitoringPanel? GetMonitoringPanel(string monitoringType)
    {
        return Panels.TryGetValue(monitoringType, out var panel) ? panel : null;
    }

    /// <summary>
    /// Returns all known monitoring types.
    /// </summary>
    public static IReadOnlyList<string> GetAllMonitoringTypes()
        => Panels.Keys.ToList();

    /// <summary>
    /// Checks whether monitoring is overdue for each parameter of the given type,
    /// given the last check date (or null if never checked).
    /// </summary>
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
                    ? $"{param.Name}: Letzte Kontrolle vor {daysSinceCheck} Tagen (Intervall: {param.IntervalDays} Tage). Uberfällig!"
                    : $"{param.Name}: Nachste Kontrolle in {param.IntervalDays - daysSinceCheck} Tagen.";
            }

            alerts.Add(new MonitoringAlert(monitoringType, param.Name, param.IntervalDays, isOverdue, message));
        }

        return alerts;
    }
}
