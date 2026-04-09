namespace MediPrax.Application.Catalogs;

/// <summary>
/// BtM substance catalog with maximum 30-day prescription quantities.
/// Based on BtMVV (Betäubungsmittelverschreibungsverordnung).
/// </summary>
public record BtmSubstance(
    string Name,
    string Unit,
    decimal Max30DayQuantity,
    string Description);

public static class BtmSubstanceCatalog
{
    private static readonly BtmSubstance[] Substances =
    [
        new("Methylphenidat", "mg", 2640m, "Methylphenidat (z.B. Ritalin, Concerta) — max. 2640 mg / 30 Tage"),
        new("Dexamfetamin", "mg", 1320m, "Dexamfetamin (z.B. Attentin) — max. 1320 mg / 30 Tage"),
        new("Lisdexamfetamin", "mg", 1800m, "Lisdexamfetamin (z.B. Elvanse) — max. 1800 mg / 30 Tage"),
        new("Cannabis (Blüten)", "g", 100m, "Medizinisches Cannabis (Blüten) — max. 100 g / 30 Tage"),
        new("Cannabis (Extrakt)", "mg", 3000m, "Cannabisextrakt — max. 3000 mg THC-Äquivalent / 30 Tage"),
        new("Dronabinol", "mg", 3000m, "Dronabinol (THC) — max. 3000 mg / 30 Tage"),
        new("Morphin", "mg", 20000m, "Morphin — max. 20000 mg / 30 Tage"),
        new("Oxycodon", "mg", 15000m, "Oxycodon — max. 15000 mg / 30 Tage"),
        new("Hydromorphon", "mg", 5000m, "Hydromorphon — max. 5000 mg / 30 Tage"),
        new("Fentanyl", "mg", 2400m, "Fentanyl (transdermal) — max. 2400 mg / 30 Tage"),
        new("Buprenorphin", "mg", 1200m, "Buprenorphin — max. 1200 mg / 30 Tage"),
        new("Methadon", "mg", 3000m, "Methadon (Substitution) — max. 3000 mg / 30 Tage"),
        new("Levomethadon", "mg", 1500m, "Levomethadon (Substitution) — max. 1500 mg / 30 Tage"),
        new("Tramadol", "mg", 30000m, "Tramadol — max. 30000 mg / 30 Tage"),
        new("Codein", "mg", 30000m, "Codein — max. 30000 mg / 30 Tage"),
        new("Clonazepam", "mg", 180m, "Clonazepam (BtM-pflichtig) — max. 180 mg / 30 Tage"),
    ];

    public static IReadOnlyList<BtmSubstance> AllSubstances => Substances;

    public static BtmSubstance? Find(string name) =>
        Substances.FirstOrDefault(s =>
            s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

    public static decimal? GetMaxQuantity(string substance) =>
        Find(substance)?.Max30DayQuantity;
}
