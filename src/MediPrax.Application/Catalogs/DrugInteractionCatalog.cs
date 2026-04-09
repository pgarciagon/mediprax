namespace MediPrax.Application.Catalogs;

public enum InteractionSeverity { Critical, Warning, Info }

public enum InteractionType
{
    QTcProlongation,
    SerotoninSyndrome,
    CYP2D6Inhibition,
    CYP3A4Induction,
    SedationPotentiation,
    AnticholinergicLoad,
    LithiumToxicity,
    AgranulocytosisRisk,
    DrugLevelIncrease
}

public record InteractionRule(
    string Substance1,
    string Substance2,
    InteractionSeverity Severity,
    InteractionType Type,
    string Description
);

public record InteractionWarning(
    string Substance1,
    string Substance2,
    InteractionSeverity Severity,
    InteractionType Type,
    string Description
);

public static class DrugInteractionCatalog
{
    private static readonly List<InteractionRule> Rules =
    [
        // Serotonin syndrome
        new("ssri", "mao-hemmer", InteractionSeverity.Critical, InteractionType.SerotoninSyndrome,
            "Kombination von SSRI und MAO-Hemmer kann ein lebensbedrohliches Serotoninsyndrom auslosen. Kontraindiziert!"),
        new("ssri", "tramadol", InteractionSeverity.Warning, InteractionType.SerotoninSyndrome,
            "SSRI und Tramadol: Erhohtes Risiko fur ein Serotoninsyndrom. Engmaschige Uberwachung erforderlich."),

        // QTc prolongation
        new("citalopram", "haloperidol", InteractionSeverity.Critical, InteractionType.QTcProlongation,
            "Citalopram und Haloperidol: Additive QTc-Verlangerung. EKG-Kontrolle obligat!"),
        new("escitalopram", "haloperidol", InteractionSeverity.Critical, InteractionType.QTcProlongation,
            "Escitalopram und Haloperidol: Additive QTc-Verlangerung. EKG-Kontrolle obligat!"),

        // CYP interactions
        new("fluoxetin", "codein", InteractionSeverity.Warning, InteractionType.CYP2D6Inhibition,
            "Fluoxetin hemmt CYP2D6: Codein wird nicht zu Morphin aktiviert. Wirkungsverlust von Codein."),
        new("carbamazepin", "orale kontrazeptiva", InteractionSeverity.Warning, InteractionType.CYP3A4Induction,
            "Carbamazepin induziert CYP3A4: Wirkungsverlust oraler Kontrazeptiva moglich. Zusatzliche Kontrazeption empfohlen."),

        // Sedation
        new("benzodiazepin", "opiat", InteractionSeverity.Warning, InteractionType.SedationPotentiation,
            "Benzodiazepine und Opiate: Additive Atemdepression und Sedierung. Dosis reduzieren und uberwachen."),

        // Anticholinergic
        new("amitriptylin", "biperiden", InteractionSeverity.Warning, InteractionType.AnticholinergicLoad,
            "Amitriptylin und Biperiden: Additive anticholinerge Belastung. Delir-Risiko erhoht, besonders bei alteren Patienten."),

        // Lithium interactions
        new("lithium", "nsaid", InteractionSeverity.Critical, InteractionType.LithiumToxicity,
            "NSAID erhohen den Lithiumspiegel durch verminderte renale Clearance. Lithiumtoxizitat moglich! Spiegel kontrollieren."),
        new("lithium", "ace-hemmer", InteractionSeverity.Warning, InteractionType.LithiumToxicity,
            "ACE-Hemmer konnen den Lithiumspiegel erhohen. Regelmassige Spiegelkontrollen erforderlich."),

        // Clozapine
        new("clozapin", "carbamazepin", InteractionSeverity.Critical, InteractionType.AgranulocytosisRisk,
            "Clozapin und Carbamazepin: Beide konnen Agranulozytose verursachen. Kombination kontraindiziert!"),

        // Drug level interactions
        new("valproat", "lamotrigin", InteractionSeverity.Warning, InteractionType.DrugLevelIncrease,
            "Valproat hemmt den Abbau von Lamotrigin: Lamotrigin-Dosis muss halbiert werden. Erhohtes Risiko fur Stevens-Johnson-Syndrom.")
    ];

    // Substance groups for matching (lowercase)
    private static readonly Dictionary<string, string[]> SubstanceGroups = new()
    {
        ["ssri"] = ["sertralin", "citalopram", "escitalopram", "fluoxetin", "paroxetin", "fluvoxamin"],
        ["mao-hemmer"] = ["tranylcypromin", "moclobemid", "selegilin", "rasagilin"],
        ["benzodiazepin"] = ["diazepam", "lorazepam", "oxazepam", "alprazolam", "clonazepam", "midazolam", "flunitrazepam"],
        ["opiat"] = ["morphin", "fentanyl", "oxycodon", "tramadol", "tilidin", "codein", "methadon", "buprenorphin"],
        ["nsaid"] = ["ibuprofen", "diclofenac", "naproxen", "indometacin", "piroxicam", "meloxicam"],
        ["ace-hemmer"] = ["ramipril", "enalapril", "lisinopril", "captopril", "perindopril"],
        ["orale kontrazeptiva"] = ["ethinylestradiol", "levonorgestrel", "desogestrel", "drospirenon"]
    };

    /// <summary>
    /// Checks all pairwise interactions among a set of active substances (Wirkstoff values).
    /// </summary>
    public static IReadOnlyList<InteractionWarning> CheckInteractions(IEnumerable<string> activeSubstances)
    {
        var substances = activeSubstances
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s.Trim().ToLower())
            .Distinct()
            .ToList();

        var warnings = new List<InteractionWarning>();

        for (var i = 0; i < substances.Count; i++)
        {
            for (var j = i + 1; j < substances.Count; j++)
            {
                var found = FindInteractions(substances[i], substances[j]);
                warnings.AddRange(found);
            }
        }

        return warnings
            .OrderByDescending(w => w.Severity == InteractionSeverity.Critical ? 0 : w.Severity == InteractionSeverity.Warning ? 1 : 2)
            .ToList();
    }

    private static IEnumerable<InteractionWarning> FindInteractions(string sub1, string sub2)
    {
        foreach (var rule in Rules)
        {
            if ((MatchesSubstance(sub1, rule.Substance1) && MatchesSubstance(sub2, rule.Substance2)) ||
                (MatchesSubstance(sub1, rule.Substance2) && MatchesSubstance(sub2, rule.Substance1)))
            {
                yield return new InteractionWarning(sub1, sub2, rule.Severity, rule.Type, rule.Description);
            }
        }
    }

    private static bool MatchesSubstance(string actual, string ruleSubstance)
    {
        // Direct match
        if (actual == ruleSubstance)
            return true;

        // Check if the actual substance is in a group matching the rule substance
        if (SubstanceGroups.TryGetValue(ruleSubstance, out var groupMembers))
        {
            if (groupMembers.Contains(actual))
                return true;
        }

        // Check if the actual substance contains the rule substance (partial match)
        if (actual.Contains(ruleSubstance) || ruleSubstance.Contains(actual))
            return true;

        return false;
    }
}
