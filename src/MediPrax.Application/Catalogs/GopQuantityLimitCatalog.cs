namespace MediPrax.Application.Catalogs;

/// <summary>
/// Defines quantity limits for GOP billing codes per patient per quarter (and optionally per day).
/// Based on EBM 2026 regulations for neurology (16), psychiatry (21), and psychotherapy (35).
/// </summary>
public record QuantityLimit(
    string GopCode,
    int MaxPerQuarter,
    int? MaxPerDay,
    string Description);

public static class GopQuantityLimitCatalog
{
    private static readonly QuantityLimit[] Limits =
    [
        // Grundpauschalen Neurologie: 1x pro Quartal/Patient
        new("16210", 1, null, "Grundpauschale Neurologie (bis 59 J.): max. 1x pro Quartal"),
        new("16211", 1, null, "Grundpauschale Neurologie (ab 60 J.): max. 1x pro Quartal"),
        new("16212", 1, null, "Grundpauschale Neurologie (Kinder): max. 1x pro Quartal"),

        // Grundpauschalen Psychiatrie: 1x pro Quartal/Patient
        new("21210", 1, null, "Grundpauschale Psychiatrie (bis 59 J.): max. 1x pro Quartal"),
        new("21211", 1, null, "Grundpauschale Psychiatrie (ab 60 J.): max. 1x pro Quartal"),
        new("21212", 1, null, "Grundpauschale Psychiatrie (Kinder/Jugendliche): max. 1x pro Quartal"),

        // Psychotherapeutische Intervention Kapitel 35
        new("35100", 6, null, "Psychotherapeutische Sprechstunde 35100: max. 6x pro Quartal"),

        // Gespräche Neurologie: max. 3x pro Quartal
        new("16220", 3, 1, "Neurologisches Gespräch 16220: max. 3x pro Quartal, 1x pro Tag"),

        // Gespräche Psychiatrie: max. 3x pro Quartal
        new("21220", 3, 1, "Psychiatrisches Gespräch 21220: max. 3x pro Quartal, 1x pro Tag"),

        // Krisenintervention: begrenzt
        new("21215", 2, 1, "Krisenintervention 21215: max. 2x pro Quartal, 1x pro Tag"),
    ];

    public static IReadOnlyList<QuantityLimit> AllLimits => Limits;

    /// <summary>
    /// Returns the quantity limit for a GOP code, or null if no limit is defined.
    /// </summary>
    public static QuantityLimit? GetLimit(string gopCode)
        => Limits.FirstOrDefault(l => l.GopCode.Equals(gopCode, StringComparison.OrdinalIgnoreCase));
}
