namespace MediPrax.Application.Catalogs;

public record AppointmentTypeTemplate(string Name, int DefaultDurationMinutes, string Color);

public static class AppointmentTypeCatalog
{
    private static readonly AppointmentTypeTemplate[] Templates =
    [
        new("Erstgespräch", 50, "#2196F3"),         // Blue
        new("Folgetermin", 25, "#4CAF50"),           // Green
        new("Medikamentenkontrolle", 10, "#FFC107"), // Yellow
        new("Psychotherapie-Sitzung", 50, "#9C27B0"),// Purple
        new("EEG", 30, "#FF9800"),                   // Orange
        new("EMG/NLG", 45, "#FF9800"),               // Orange
        new("Akuttermin", 25, "#F44336"),            // Red
        new("Notfall", 15, "#B71C1C"),               // Dark Red
    ];

    public static IReadOnlyList<AppointmentTypeTemplate> AllTemplates => Templates;

    public static AppointmentTypeTemplate? GetByName(string name)
        => Templates.FirstOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
}
