namespace MediPrax.Application.Catalogs;

/// <summary>
/// Defines mutually exclusive GOP pairs that cannot be billed together in the same encounter.
/// Based on EBM 2026 chapter exclusions for neurology (16), psychiatry (21), and psychotherapy (35).
/// </summary>
public record GopExclusionRule(string GopCodeA, string GopCodeB, string Reason);

public static class GopExclusionCatalog
{
    private static readonly GopExclusionRule[] Rules =
    [
        // Kapitel 16 — Neurologie: Grundpauschalen sind untereinander nicht kombinierbar
        new("16210", "16211", "Grundpauschalen Neurologie 16210/16211 schließen sich gegenseitig aus (Altersgruppen)"),
        new("16210", "21210", "Neurologie-Grundpauschale 16210 schließt Psychiatrie-Grundpauschale 21210 aus"),
        new("16211", "21211", "Neurologie-Grundpauschale 16211 schließt Psychiatrie-Grundpauschale 21211 aus"),
        new("16210", "21211", "Neurologie-Grundpauschale 16210 schließt Psychiatrie-Grundpauschale 21211 aus"),
        new("16211", "21210", "Neurologie-Grundpauschale 16211 schließt Psychiatrie-Grundpauschale 21210 aus"),

        // Kapitel 16 — Gespräche: 16220 und Zuschlag 16221 erfordern Erstabrechnung 16220
        new("16220", "21220", "Neurologisches Gespräch 16220 und psychiatrisches Gespräch 21220 schließen sich aus"),

        // Kapitel 21 — Psychiatrie: Grundpauschalen untereinander
        new("21210", "21211", "Grundpauschalen Psychiatrie 21210/21211 schließen sich gegenseitig aus (Altersgruppen)"),
        new("21213", "21214", "Zusatzpauschale 21213 und Zuschlag schwere Störung 21214 nicht kombinierbar"),

        // Kapitel 35 — Psychotherapie: Konflikt mit Grundpauschalen
        new("35100", "21213", "Psychotherapeutische Intervention 35100 und psychiatrische Zusatzpauschale 21213 schließen sich aus"),
        new("35100", "21215", "Psychotherapeutische Intervention 35100 und Krisenintervention 21215 schließen sich aus"),

        // Grundpauschale + Notfall
        new("16210", "01101", "Grundpauschale und Notfallkomplex 01101 schließen sich aus"),
        new("16211", "01101", "Grundpauschale und Notfallkomplex 01101 schließen sich aus"),
        new("21210", "01101", "Grundpauschale und Notfallkomplex 01101 schließen sich aus"),
        new("21211", "01101", "Grundpauschale und Notfallkomplex 01101 schließen sich aus"),

        // Neurologische Untersuchungen: vollständig vs. symptombezogen
        new("16230", "16231", "Neurologische Untersuchung 16230 und erweiterte Untersuchung 16231 schließen sich aus"),

        // Psychiatrische Untersuchung vs. Testverfahren
        new("21230", "21235", "Psychiatrische Untersuchung 21230 und Testverfahren 21235 können nicht gleichzeitig abgerechnet werden"),
    ];

    public static IReadOnlyList<GopExclusionRule> AllRules => Rules;

    /// <summary>
    /// Returns exclusion rules that apply to a given GOP code.
    /// </summary>
    public static IReadOnlyList<GopExclusionRule> GetRulesFor(string gopCode)
        => Rules
            .Where(r => r.GopCodeA.Equals(gopCode, StringComparison.OrdinalIgnoreCase)
                     || r.GopCodeB.Equals(gopCode, StringComparison.OrdinalIgnoreCase))
            .ToList();

    /// <summary>
    /// Checks whether two GOP codes are mutually exclusive.
    /// </summary>
    public static bool AreExclusive(string gopCodeA, string gopCodeB)
        => Rules.Any(r =>
            (r.GopCodeA.Equals(gopCodeA, StringComparison.OrdinalIgnoreCase) && r.GopCodeB.Equals(gopCodeB, StringComparison.OrdinalIgnoreCase)) ||
            (r.GopCodeA.Equals(gopCodeB, StringComparison.OrdinalIgnoreCase) && r.GopCodeB.Equals(gopCodeA, StringComparison.OrdinalIgnoreCase)));

    /// <summary>
    /// Finds the exclusion rule for a given pair, or null if no rule applies.
    /// </summary>
    public static GopExclusionRule? FindRule(string gopCodeA, string gopCodeB)
        => Rules.FirstOrDefault(r =>
            (r.GopCodeA.Equals(gopCodeA, StringComparison.OrdinalIgnoreCase) && r.GopCodeB.Equals(gopCodeB, StringComparison.OrdinalIgnoreCase)) ||
            (r.GopCodeA.Equals(gopCodeB, StringComparison.OrdinalIgnoreCase) && r.GopCodeB.Equals(gopCodeA, StringComparison.OrdinalIgnoreCase)));
}
