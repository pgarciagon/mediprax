namespace MediPrax.Application.Catalogs;

public record MonitoringParameter(string Name, string Unit, decimal? RefLow, decimal? RefHigh, int IntervalDays);

public record MonitoringPanel(string Name, string Description, IReadOnlyList<MonitoringParameter> Parameters);

public static class LabMonitoringCatalog
{
    private static readonly List<MonitoringPanel> _panels =
    [
        new("Lithium",
            "Monitoring bei Lithiumtherapie",
            [
                new("Lithiumspiegel", "mmol/l", 0.6m, 0.8m, 90),
                new("TSH", "mU/l", 0.4m, 4.0m, 180),
                new("Kreatinin", "mg/dl", 0.7m, 1.2m, 90),
                new("GFR", "ml/min", 60m, null, 90)
            ]),

        new("Clozapin",
            "Monitoring bei Clozapintherapie",
            [
                new("Leukozyten", "/\u00b5l", 3500m, 10000m, 7),
                new("Neutrophile", "/\u00b5l", 1500m, null, 7)
            ]),

        new("Valproat",
            "Monitoring bei Valproattherapie",
            [
                new("Valproatspiegel", "\u00b5g/ml", 50m, 100m, 90),
                new("GPT", "U/l", null, 35m, 180),
                new("Blutbild", "", null, null, 180)
            ]),

        new("Carbamazepin",
            "Monitoring bei Carbamazepintherapie",
            [
                new("Carbamazepinspiegel", "\u00b5g/ml", 4m, 12m, 90),
                new("Blutbild", "", null, null, 90),
                new("Natrium", "mmol/l", 135m, 145m, 90)
            ]),

        new("Metabolisch",
            "Metabolisches Monitoring bei atypischen Antipsychotika",
            [
                new("Gewicht", "kg", null, null, 90),
                new("Glucose", "mg/dl", 70m, 110m, 90),
                new("HbA1c", "%", null, 6.5m, 180),
                new("Triglyceride", "mg/dl", null, 150m, 180),
                new("Cholesterin", "mg/dl", null, 200m, 180)
            ])
    ];

    public static IReadOnlyList<MonitoringPanel> AllPanels => _panels;

    public static MonitoringPanel? GetPanel(string monitoringType)
    {
        return _panels.FirstOrDefault(p =>
            p.Name.Equals(monitoringType, StringComparison.OrdinalIgnoreCase));
    }
}
