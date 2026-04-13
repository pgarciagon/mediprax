using MediPrax.Core.Entities;
using MediPrax.Core.ValueObjects;

namespace MediPrax.Application.Catalogs;

public enum SuggestionConfidence { Low, Medium, High }

public record DiagnosisSuggestion(
    string Icd10Code,
    string Description,
    SuggestionConfidence Confidence,
    string Rationale);

/// <summary>
/// Static catalog that suggests ICD-10 diagnoses based on structured AMDP findings,
/// neurological examination results, and suicidality assessments.
/// Pure rule-based approach — no DB, no ML.
/// </summary>
public static class DiagnosisSuggestionCatalog
{
    /// <summary>
    /// Suggests ICD-10 diagnoses from AMDP psychopathological findings.
    /// </summary>
    public static IReadOnlyList<DiagnosisSuggestion> SuggestFromPsychopathFindings(
        IEnumerable<SymptomFinding> findings)
    {
        if (findings is null) return [];

        var list = findings.Where(f => f.Severity > 0).ToList();
        if (list.Count == 0) return [];

        var suggestions = new List<DiagnosisSuggestion>();

        // Helper: get max severity for a symptom code
        int Sev(string code) => list
            .Where(f => f.SymptomCode.Equals(code, StringComparison.OrdinalIgnoreCase))
            .Select(f => f.Severity)
            .DefaultIfEmpty(0)
            .Max();

        // Helper: get max severity in a category
        int CatMax(string cat) => list
            .Where(f => f.CategoryCode.Equals(cat, StringComparison.OrdinalIgnoreCase))
            .Select(f => f.Severity)
            .DefaultIfEmpty(0)
            .Max();

        // Helper: count symptoms present (severity > 0) in a category
        int CatCount(string cat) => list
            .Count(f => f.CategoryCode.Equals(cat, StringComparison.OrdinalIgnoreCase) && f.Severity > 0);

        // Helper: is a specific symptom present?
        bool Has(string code) => Sev(code) > 0;

        // ---- Depression rules ----
        var depressedSev = Sev("AFF-5"); // Deprimiert/Traurig
        if (depressedSev > 0)
        {
            // Count supporting symptoms for depression
            var supportingSymptoms = new List<string>();
            if (Has("SOM-1")) supportingSymptoms.Add("Schlafstorungen (SOM-1)");
            if (Has("SOM-2")) supportingSymptoms.Add("Appetitminderung (SOM-2)");
            if (Has("ANT-1") || Has("ANT-2")) supportingSymptoms.Add("Antriebsminderung (ANT-1/2)");
            if (Has("AUF-2")) supportingSymptoms.Add("Konzentrationsstorung (AUF-2)");
            if (Has("AFF-6")) supportingSymptoms.Add("Hoffnungslosigkeit (AFF-6)");
            if (Has("AFF-15")) supportingSymptoms.Add("Schuldgefuhle (AFF-15)");
            if (Has("AFF-13")) supportingSymptoms.Add("Insuffizienzgefuhle (AFF-13)");
            if (Has("AFF-2")) supportingSymptoms.Add("Gefuhllosigkeit (AFF-2)");
            if (Has("AFF-4")) supportingSymptoms.Add("Vitalstorung (AFF-4)");
            if (Has("CIR-1")) supportingSymptoms.Add("Morgentief (CIR-1)");
            if (Has("SOM-6")) supportingSymptoms.Add("Libidoverlust (SOM-6)");
            if (Has("ANT-10")) supportingSymptoms.Add("Sozialer Ruckzug (ANT-10)");
            if (Has("FDS-1") || Has("FDS-2")) supportingSymptoms.Add("Denkhemmung/-verlangsamung (FDS-1/2)");
            if (Has("FDS-6")) supportingSymptoms.Add("Grubeln (FDS-6)");

            var supportCount = supportingSymptoms.Count;
            var supportText = string.Join(", ", supportingSymptoms);

            // Check for psychotic features
            var hasHallucinations = CatMax("SIN") >= 1;
            var hasDelusions = CatMax("INH") >= 2;
            var hasPsychotic = hasHallucinations || hasDelusions;

            if (depressedSev >= 3 && supportCount >= 5 && hasPsychotic)
            {
                suggestions.Add(new DiagnosisSuggestion(
                    "F32.3",
                    "Schwere depressive Episode mit psychotischen Symptomen",
                    SuggestionConfidence.High,
                    $"Depressive Stimmung (Schweregrad 3) + {supportCount} Begleitsymptome ({supportText}) + psychotische Merkmale"));
            }
            else if (depressedSev >= 3 && supportCount >= 5)
            {
                suggestions.Add(new DiagnosisSuggestion(
                    "F32.2",
                    "Schwere depressive Episode ohne psychotische Symptome",
                    SuggestionConfidence.High,
                    $"Depressive Stimmung (Schweregrad 3) + {supportCount} Begleitsymptome ({supportText})"));
            }
            else if (depressedSev >= 2 && supportCount >= 4)
            {
                suggestions.Add(new DiagnosisSuggestion(
                    "F32.1",
                    "Mittelgradige depressive Episode",
                    SuggestionConfidence.Medium,
                    $"Depressive Stimmung (Schweregrad {depressedSev}) + {supportCount} Begleitsymptome ({supportText})"));
            }
            else if (depressedSev >= 1 && supportCount >= 2)
            {
                suggestions.Add(new DiagnosisSuggestion(
                    "F32.0",
                    "Leichte depressive Episode",
                    SuggestionConfidence.Medium,
                    $"Depressive Stimmung (Schweregrad {depressedSev}) + {supportCount} Begleitsymptome ({supportText})"));
            }
        }

        // ---- Mania rules ----
        var euphoricSev = Sev("AFF-8"); // Euphorisch
        if (euphoricSev >= 2)
        {
            var maniaEvidence = new List<string> { $"Euphorie (Schweregrad {euphoricSev})" };
            if (Has("ANT-3")) maniaEvidence.Add("Antriebssteigerung (ANT-3)");
            if (Has("ANT-9")) maniaEvidence.Add("Logorrhoe (ANT-9)");
            if (Has("FDS-8")) maniaEvidence.Add("Ideenflucht (FDS-8)");
            if (Has("AFF-14")) maniaEvidence.Add("Gesteigertes Selbstwertgefuhl (AFF-14)");
            if (Has("AFF-10")) maniaEvidence.Add("Reizbarkeit (AFF-10)");

            if (maniaEvidence.Count >= 3) // euphoria + at least 2 supporting
            {
                suggestions.Add(new DiagnosisSuggestion(
                    "F30.1",
                    "Manie ohne psychotische Symptome",
                    SuggestionConfidence.Medium,
                    string.Join(", ", maniaEvidence)));
            }
        }

        // ---- Paranoid schizophrenia rules ----
        var delusionSev = CatMax("INH");
        var hallucinationSev = CatMax("SIN");
        var egoDisturbanceSev = CatMax("ICH");

        if (delusionSev >= 2 || (hallucinationSev >= 2 && egoDisturbanceSev >= 1))
        {
            var schizEvidence = new List<string>();
            if (delusionSev >= 2) schizEvidence.Add($"Wahn (Schweregrad {delusionSev})");
            if (hallucinationSev >= 2) schizEvidence.Add($"Halluzinationen (Schweregrad {hallucinationSev})");
            if (egoDisturbanceSev >= 1) schizEvidence.Add($"Ich-Storungen (Schweregrad {egoDisturbanceSev})");

            // Only suggest if hallucinations are present (otherwise delusional disorder)
            if (hallucinationSev >= 1 || egoDisturbanceSev >= 1)
            {
                suggestions.Add(new DiagnosisSuggestion(
                    "F20.0",
                    "Paranoide Schizophrenie",
                    delusionSev >= 3 || hallucinationSev >= 3 ? SuggestionConfidence.High : SuggestionConfidence.Medium,
                    string.Join(", ", schizEvidence)));
            }
        }

        // ---- Delusional disorder (F22.0) ----
        if (delusionSev >= 2 && hallucinationSev == 0 && egoDisturbanceSev == 0)
        {
            var delusionSymptoms = list
                .Where(f => f.CategoryCode == "INH" && f.Severity >= 2)
                .Select(f => f.SymptomName)
                .ToList();
            suggestions.Add(new DiagnosisSuggestion(
                "F22.0",
                "Wahnhafte Storung",
                SuggestionConfidence.Medium,
                $"Wahnphanomene ohne Halluzinationen: {string.Join(", ", delusionSymptoms)}"));
        }

        // ---- Generalized anxiety (F41.1) ----
        var anxietySev = Sev("AFF-7"); // Angstlich
        if (anxietySev >= 2)
        {
            var anxietyEvidence = new List<string> { $"Angstlichkeit (Schweregrad {anxietySev})" };
            if (Has("AFF-11")) anxietyEvidence.Add("Innere Unruhe (AFF-11)");
            if (Has("FDS-6")) anxietyEvidence.Add("Grubeln (FDS-6)");
            if (CatCount("SOM") >= 1) anxietyEvidence.Add($"Somatische Symptome ({CatCount("SOM")} Befunde)");

            var hasSomatic = CatCount("SOM") >= 1;
            var hasWorry = Has("FDS-6") || Has("AFF-11");

            if (hasSomatic && hasWorry)
            {
                suggestions.Add(new DiagnosisSuggestion(
                    "F41.1",
                    "Generalisierte Angststorung",
                    SuggestionConfidence.Medium,
                    string.Join(", ", anxietyEvidence)));
            }
        }

        // ---- OCD (F42.0) ----
        var obsessionSev = Sev("INH-1"); // Zwangsgedanken
        if (obsessionSev >= 2)
        {
            suggestions.Add(new DiagnosisSuggestion(
                "F42.0",
                "Vorwiegend Zwangsgedanken",
                SuggestionConfidence.Medium,
                $"Zwangsgedanken (Schweregrad {obsessionSev})"));
        }

        // ---- Delirium (F05.0) ----
        var consciousnessSev = CatMax("BEW");
        var orientationSev = CatMax("ORI");
        if (consciousnessSev >= 1 && orientationSev >= 1)
        {
            suggestions.Add(new DiagnosisSuggestion(
                "F05.0",
                "Delir ohne Demenz",
                consciousnessSev >= 2 && orientationSev >= 2 ? SuggestionConfidence.High : SuggestionConfidence.Medium,
                $"Bewusstseinsstorung (Schweregrad {consciousnessSev}) + Orientierungsstorung (Schweregrad {orientationSev})"));
        }

        // ---- Mild cognitive impairment (F06.7) ----
        var attentionSev = CatMax("AUF");
        if (attentionSev >= 1 && (Has("AUF-3") || Has("AUF-4")))
        {
            var cogEvidence = new List<string>();
            if (Has("AUF-2")) cogEvidence.Add("Konzentrationsstorung");
            if (Has("AUF-3")) cogEvidence.Add("Merkfahigkeitsstorung");
            if (Has("AUF-4")) cogEvidence.Add("Gedachtnisstorung");
            if (Has("AUF-1")) cogEvidence.Add("Auffassungsstorung");

            // Only suggest if no delirium already suggested (avoid overlap)
            if (consciousnessSev == 0)
            {
                suggestions.Add(new DiagnosisSuggestion(
                    "F06.7",
                    "Leichte kognitive Storung",
                    SuggestionConfidence.Low,
                    $"Kognitive Defizite: {string.Join(", ", cogEvidence)}"));
            }
        }

        return suggestions;
    }

    /// <summary>
    /// Suggests ICD-10 diagnoses from neurological examination findings.
    /// </summary>
    public static IReadOnlyList<DiagnosisSuggestion> SuggestFromNeurologicalExam(
        NeurologicalExamination exam)
    {
        if (exam is null) return [];

        var suggestions = new List<DiagnosisSuggestion>();

        // ---- Parkinson (G20) ----
        var hasRigidity = exam.MotorSystem.ToneRightArm == "rigide"
                       || exam.MotorSystem.ToneLeftArm == "rigide"
                       || exam.MotorSystem.ToneRightLeg == "rigide"
                       || exam.MotorSystem.ToneLeftLeg == "rigide";
        var hasKleinschrittig = exam.Gait.GaitPattern == "kleinschrittig";
        var hasReducedArmSwing = exam.Gait.ArmSwing != "seitengleich";

        if (hasRigidity && hasKleinschrittig && hasReducedArmSwing)
        {
            suggestions.Add(new DiagnosisSuggestion(
                "G20.0",
                "Primares Parkinson-Syndrom",
                SuggestionConfidence.High,
                "Rigor + kleinschrittiger Gang + vermindertes Mitschwingen der Arme"));
        }
        else if (hasRigidity && (hasKleinschrittig || hasReducedArmSwing))
        {
            suggestions.Add(new DiagnosisSuggestion(
                "G20.0",
                "Primares Parkinson-Syndrom",
                SuggestionConfidence.Medium,
                $"Rigor{(hasKleinschrittig ? " + kleinschrittiger Gang" : "")}{(hasReducedArmSwing ? " + vermindertes Mitschwingen" : "")}"));
        }

        // ---- Polyneuropathy (G62.9) ----
        var hasSensoryLoss = exam.SensorySystem.TouchSensation != "intakt"
                          || exam.SensorySystem.PainSensation != "intakt";
        var hasReducedAchilles = exam.Reflexes.AchillesRight <= 1
                              || exam.Reflexes.AchillesLeft <= 1;
        var hasReducedVibration = exam.SensorySystem.VibrationSenseRight != "normal"
                               || exam.SensorySystem.VibrationSenseLeft != "normal";

        if (hasSensoryLoss && hasReducedAchilles)
        {
            var evidence = new List<string> { "Sensibilitatsstorung" };
            if (hasReducedAchilles) evidence.Add("abgeschwachter/fehlender ASR");
            if (hasReducedVibration) evidence.Add("vermindertes Vibrationsempfinden");

            suggestions.Add(new DiagnosisSuggestion(
                "G62.9",
                "Polyneuropathie, nicht naher bezeichnet",
                hasReducedVibration ? SuggestionConfidence.High : SuggestionConfidence.Medium,
                string.Join(" + ", evidence)));
        }

        // ---- Hemiparesis (G81.9) ----
        var rightArmWeak = exam.MotorSystem.StrengthRightArm < 5;
        var leftArmWeak = exam.MotorSystem.StrengthLeftArm < 5;
        var rightLegWeak = exam.MotorSystem.StrengthRightLeg < 5;
        var leftLegWeak = exam.MotorSystem.StrengthLeftLeg < 5;

        // Right-sided weakness
        if (rightArmWeak && rightLegWeak && !leftArmWeak && !leftLegWeak)
        {
            suggestions.Add(new DiagnosisSuggestion(
                "G81.9",
                "Hemiparese, nicht naher bezeichnet",
                SuggestionConfidence.High,
                $"Rechtsseitige Schwache: Arm KG {exam.MotorSystem.StrengthRightArm}/5, Bein KG {exam.MotorSystem.StrengthRightLeg}/5"));
        }
        // Left-sided weakness
        else if (leftArmWeak && leftLegWeak && !rightArmWeak && !rightLegWeak)
        {
            suggestions.Add(new DiagnosisSuggestion(
                "G81.9",
                "Hemiparese, nicht naher bezeichnet",
                SuggestionConfidence.High,
                $"Linksseitige Schwache: Arm KG {exam.MotorSystem.StrengthLeftArm}/5, Bein KG {exam.MotorSystem.StrengthLeftLeg}/5"));
        }
        // Unilateral arm or leg weakness (lower confidence)
        else if ((rightArmWeak || rightLegWeak) && !leftArmWeak && !leftLegWeak)
        {
            suggestions.Add(new DiagnosisSuggestion(
                "G81.9",
                "Hemiparese, nicht naher bezeichnet",
                SuggestionConfidence.Medium,
                $"Rechtsseitige Schwache: Arm KG {exam.MotorSystem.StrengthRightArm}/5, Bein KG {exam.MotorSystem.StrengthRightLeg}/5"));
        }
        else if ((leftArmWeak || leftLegWeak) && !rightArmWeak && !rightLegWeak)
        {
            suggestions.Add(new DiagnosisSuggestion(
                "G81.9",
                "Hemiparese, nicht naher bezeichnet",
                SuggestionConfidence.Medium,
                $"Linksseitige Schwache: Arm KG {exam.MotorSystem.StrengthLeftArm}/5, Bein KG {exam.MotorSystem.StrengthLeftLeg}/5"));
        }

        // ---- Facial paresis (G51.0) ----
        var hasFacialAsymmetry = exam.CranialNerves.FacialNerveStatus != "seitengleich innerviert"
                              || !exam.CranialNerves.ForeheadWrinkleSymmetric
                              || !exam.CranialNerves.MouthCornerSymmetric;

        if (hasFacialAsymmetry)
        {
            var details = new List<string>();
            if (exam.CranialNerves.FacialNerveStatus != "seitengleich innerviert")
                details.Add($"Fazialis: {exam.CranialNerves.FacialNerveStatus}");
            if (!exam.CranialNerves.ForeheadWrinkleSymmetric)
                details.Add("asymmetrische Stirnrunzeln");
            if (!exam.CranialNerves.MouthCornerSymmetric)
                details.Add("asymmetrischer Mundwinkel");

            suggestions.Add(new DiagnosisSuggestion(
                "G51.0",
                "Fazialisparese",
                SuggestionConfidence.Medium,
                string.Join(", ", details)));
        }

        // ---- Meningismus (R29.1) ----
        if (exam.MeningealSigns.NeckStiffness
            || exam.MeningealSigns.KernigSign
            || exam.MeningealSigns.BrudzinskiSign)
        {
            var signs = new List<string>();
            if (exam.MeningealSigns.NeckStiffness) signs.Add("Meningismus");
            if (exam.MeningealSigns.KernigSign) signs.Add("Kernig-Zeichen positiv");
            if (exam.MeningealSigns.BrudzinskiSign) signs.Add("Brudzinski-Zeichen positiv");

            suggestions.Add(new DiagnosisSuggestion(
                "R29.1",
                "Meningismus",
                signs.Count >= 2 ? SuggestionConfidence.High : SuggestionConfidence.Medium,
                string.Join(", ", signs)));
        }

        return suggestions;
    }

    /// <summary>
    /// Suggests ICD-10 diagnoses based on suicidality assessment.
    /// </summary>
    public static IReadOnlyList<DiagnosisSuggestion> SuggestFromSuicidality(
        SuicidalityAssessment assessment)
    {
        if (assessment is null) return [];

        if (assessment.SuicidalIdeation)
        {
            var evidence = new List<string> { "Suizidgedanken vorhanden" };
            if (assessment.SuicidalPlans) evidence.Add("konkrete Plane");
            if (assessment.SuicidalIntent) evidence.Add("Suizidabsicht");
            if (assessment.PriorAttempts) evidence.Add("fruhere Suizidversuche");

            var confidence = assessment.SuicidalPlans || assessment.SuicidalIntent
                ? SuggestionConfidence.High
                : SuggestionConfidence.Medium;

            return
            [
                new DiagnosisSuggestion(
                    "R45.8",
                    "Sonstige Symptome, die die Stimmung betreffen (Suizidalitat)",
                    confidence,
                    string.Join(", ", evidence))
            ];
        }

        return [];
    }

    /// <summary>
    /// Combines diagnosis suggestions from all available sources, sorted by confidence (High first)
    /// then alphabetically by ICD-10 code. Duplicates are removed (highest confidence wins).
    /// </summary>
    public static IReadOnlyList<DiagnosisSuggestion> SuggestAll(
        IEnumerable<SymptomFinding>? psychFindings,
        NeurologicalExamination? neuroExam,
        SuicidalityAssessment? suicidality)
    {
        var all = new List<DiagnosisSuggestion>();

        if (psychFindings is not null)
            all.AddRange(SuggestFromPsychopathFindings(psychFindings));

        if (neuroExam is not null)
            all.AddRange(SuggestFromNeurologicalExam(neuroExam));

        if (suicidality is not null)
            all.AddRange(SuggestFromSuicidality(suicidality));

        // Deduplicate by ICD-10 code, keeping highest confidence
        var deduped = all
            .GroupBy(s => s.Icd10Code)
            .Select(g => g.OrderByDescending(s => s.Confidence).First())
            .ToList();

        return deduped
            .OrderByDescending(s => s.Confidence)
            .ThenBy(s => s.Icd10Code)
            .ToList();
    }
}
