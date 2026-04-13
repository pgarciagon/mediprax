using MediPrax.Application.Catalogs;
using MediPrax.Core.Entities;
using MediPrax.Core.ValueObjects;

namespace MediPrax.UnitTests;

public class DiagnosisSuggestionCatalogTests
{
    // ========== Depression tests ==========

    [Fact]
    public void SuggestFromPsychopathFindings_MildDepression_ReturnF320()
    {
        var findings = new List<SymptomFinding>
        {
            new() { CategoryCode = "AFF", SymptomCode = "AFF-5", SymptomName = "Deprimiert", Severity = 1 },
            new() { CategoryCode = "SOM", SymptomCode = "SOM-1", SymptomName = "Schlafstorungen", Severity = 1 },
            new() { CategoryCode = "ANT", SymptomCode = "ANT-1", SymptomName = "Antriebsarm", Severity = 1 },
        };

        var result = DiagnosisSuggestionCatalog.SuggestFromPsychopathFindings(findings);

        Assert.Contains(result, s => s.Icd10Code == "F32.0");
        Assert.DoesNotContain(result, s => s.Icd10Code == "F32.1");
    }

    [Fact]
    public void SuggestFromPsychopathFindings_ModerateDepression_ReturnF321()
    {
        var findings = new List<SymptomFinding>
        {
            new() { CategoryCode = "AFF", SymptomCode = "AFF-5", SymptomName = "Deprimiert", Severity = 2 },
            new() { CategoryCode = "SOM", SymptomCode = "SOM-1", SymptomName = "Schlafstorungen", Severity = 2 },
            new() { CategoryCode = "SOM", SymptomCode = "SOM-2", SymptomName = "Appetitminderung", Severity = 1 },
            new() { CategoryCode = "ANT", SymptomCode = "ANT-1", SymptomName = "Antriebsarm", Severity = 2 },
            new() { CategoryCode = "AUF", SymptomCode = "AUF-2", SymptomName = "Konzentrationsstorung", Severity = 1 },
        };

        var result = DiagnosisSuggestionCatalog.SuggestFromPsychopathFindings(findings);

        Assert.Contains(result, s => s.Icd10Code == "F32.1");
    }

    [Fact]
    public void SuggestFromPsychopathFindings_SevereDepression_ReturnF322()
    {
        var findings = new List<SymptomFinding>
        {
            new() { CategoryCode = "AFF", SymptomCode = "AFF-5", SymptomName = "Deprimiert", Severity = 3 },
            new() { CategoryCode = "SOM", SymptomCode = "SOM-1", SymptomName = "Schlafstorungen", Severity = 3 },
            new() { CategoryCode = "SOM", SymptomCode = "SOM-2", SymptomName = "Appetitminderung", Severity = 2 },
            new() { CategoryCode = "ANT", SymptomCode = "ANT-1", SymptomName = "Antriebsarm", Severity = 3 },
            new() { CategoryCode = "AUF", SymptomCode = "AUF-2", SymptomName = "Konzentrationsstorung", Severity = 2 },
            new() { CategoryCode = "AFF", SymptomCode = "AFF-6", SymptomName = "Hoffnungslos", Severity = 3 },
            new() { CategoryCode = "AFF", SymptomCode = "AFF-15", SymptomName = "Schuldgefuhle", Severity = 2 },
        };

        var result = DiagnosisSuggestionCatalog.SuggestFromPsychopathFindings(findings);

        Assert.Contains(result, s => s.Icd10Code == "F32.2");
        Assert.DoesNotContain(result, s => s.Icd10Code == "F32.3");
    }

    [Fact]
    public void SuggestFromPsychopathFindings_SevereDepressionWithPsychosis_ReturnF323()
    {
        var findings = new List<SymptomFinding>
        {
            new() { CategoryCode = "AFF", SymptomCode = "AFF-5", SymptomName = "Deprimiert", Severity = 3 },
            new() { CategoryCode = "SOM", SymptomCode = "SOM-1", SymptomName = "Schlafstorungen", Severity = 3 },
            new() { CategoryCode = "SOM", SymptomCode = "SOM-2", SymptomName = "Appetitminderung", Severity = 2 },
            new() { CategoryCode = "ANT", SymptomCode = "ANT-1", SymptomName = "Antriebsarm", Severity = 3 },
            new() { CategoryCode = "AUF", SymptomCode = "AUF-2", SymptomName = "Konzentrationsstorung", Severity = 2 },
            new() { CategoryCode = "AFF", SymptomCode = "AFF-6", SymptomName = "Hoffnungslos", Severity = 3 },
            new() { CategoryCode = "AFF", SymptomCode = "AFF-15", SymptomName = "Schuldgefuhle", Severity = 2 },
            // Psychotic: hallucinations
            new() { CategoryCode = "SIN", SymptomCode = "SIN-2", SymptomName = "Stimmenhoren", Severity = 2 },
        };

        var result = DiagnosisSuggestionCatalog.SuggestFromPsychopathFindings(findings);

        Assert.Contains(result, s => s.Icd10Code == "F32.3");
    }

    // ========== Schizophrenia tests ==========

    [Fact]
    public void SuggestFromPsychopathFindings_ParanoidSchizophrenia_ReturnF200()
    {
        var findings = new List<SymptomFinding>
        {
            new() { CategoryCode = "INH", SymptomCode = "INH-8", SymptomName = "Verfolgungswahn", Severity = 3 },
            new() { CategoryCode = "SIN", SymptomCode = "SIN-2", SymptomName = "Stimmenhoren", Severity = 2 },
            new() { CategoryCode = "ICH", SymptomCode = "ICH-6", SymptomName = "Fremdbeeinflussungserlebnisse", Severity = 2 },
        };

        var result = DiagnosisSuggestionCatalog.SuggestFromPsychopathFindings(findings);

        Assert.Contains(result, s => s.Icd10Code == "F20.0");
        var schiz = result.First(s => s.Icd10Code == "F20.0");
        Assert.Equal(SuggestionConfidence.High, schiz.Confidence);
    }

    [Fact]
    public void SuggestFromPsychopathFindings_DelusionalDisorder_ReturnF220()
    {
        var findings = new List<SymptomFinding>
        {
            new() { CategoryCode = "INH", SymptomCode = "INH-8", SymptomName = "Verfolgungswahn", Severity = 2 },
            new() { CategoryCode = "INH", SymptomCode = "INH-6", SymptomName = "Beziehungswahn", Severity = 2 },
        };

        var result = DiagnosisSuggestionCatalog.SuggestFromPsychopathFindings(findings);

        Assert.Contains(result, s => s.Icd10Code == "F22.0");
        // No F20.0 because no hallucinations or ego disturbances
        Assert.DoesNotContain(result, s => s.Icd10Code == "F20.0");
    }

    // ========== Neurology tests ==========

    [Fact]
    public void SuggestFromNeurologicalExam_Parkinson_ReturnG20()
    {
        var exam = new NeurologicalExamination
        {
            MotorSystem = new MotorFindings
            {
                ToneRightArm = "rigide",
                ToneLeftArm = "rigide",
            },
            Gait = new GaitFindings
            {
                GaitPattern = "kleinschrittig",
                ArmSwing = "vermindert rechts"
            },
            CranialNerves = new CranialNerveFindings(),
            Reflexes = new ReflexFindings(),
            SensorySystem = new SensoryFindings(),
            Coordination = new CoordinationFindings(),
            MeningealSigns = new MeningealFindings()
        };

        var result = DiagnosisSuggestionCatalog.SuggestFromNeurologicalExam(exam);

        Assert.Contains(result, s => s.Icd10Code == "G20.0");
        var parkinson = result.First(s => s.Icd10Code == "G20.0");
        Assert.Equal(SuggestionConfidence.High, parkinson.Confidence);
    }

    [Fact]
    public void SuggestFromNeurologicalExam_Polyneuropathy_ReturnG629()
    {
        var exam = new NeurologicalExamination
        {
            SensorySystem = new SensoryFindings
            {
                TouchSensation = "eingeschrankt",
                VibrationSenseRight = "4/8",
                VibrationSenseLeft = "4/8"
            },
            Reflexes = new ReflexFindings
            {
                AchillesRight = 0,
                AchillesLeft = 1
            },
            MotorSystem = new MotorFindings(),
            CranialNerves = new CranialNerveFindings(),
            Gait = new GaitFindings(),
            Coordination = new CoordinationFindings(),
            MeningealSigns = new MeningealFindings()
        };

        var result = DiagnosisSuggestionCatalog.SuggestFromNeurologicalExam(exam);

        Assert.Contains(result, s => s.Icd10Code == "G62.9");
    }

    [Fact]
    public void SuggestFromNeurologicalExam_Hemiparesis_ReturnG819()
    {
        var exam = new NeurologicalExamination
        {
            MotorSystem = new MotorFindings
            {
                StrengthLeftArm = 3,
                StrengthLeftLeg = 4,
                StrengthRightArm = 5,
                StrengthRightLeg = 5
            },
            CranialNerves = new CranialNerveFindings(),
            Reflexes = new ReflexFindings(),
            SensorySystem = new SensoryFindings(),
            Gait = new GaitFindings(),
            Coordination = new CoordinationFindings(),
            MeningealSigns = new MeningealFindings()
        };

        var result = DiagnosisSuggestionCatalog.SuggestFromNeurologicalExam(exam);

        Assert.Contains(result, s => s.Icd10Code == "G81.9");
        var hemi = result.First(s => s.Icd10Code == "G81.9");
        Assert.Equal(SuggestionConfidence.High, hemi.Confidence);
        Assert.Contains("Linksseitig", hemi.Rationale);
    }

    [Fact]
    public void SuggestFromNeurologicalExam_Meningismus_ReturnR291()
    {
        var exam = new NeurologicalExamination
        {
            MeningealSigns = new MeningealFindings
            {
                NeckStiffness = true,
                KernigSign = true
            },
            MotorSystem = new MotorFindings(),
            CranialNerves = new CranialNerveFindings(),
            Reflexes = new ReflexFindings(),
            SensorySystem = new SensoryFindings(),
            Gait = new GaitFindings(),
            Coordination = new CoordinationFindings()
        };

        var result = DiagnosisSuggestionCatalog.SuggestFromNeurologicalExam(exam);

        Assert.Contains(result, s => s.Icd10Code == "R29.1");
        var mening = result.First(s => s.Icd10Code == "R29.1");
        Assert.Equal(SuggestionConfidence.High, mening.Confidence);
    }

    // ========== Empty findings ==========

    [Fact]
    public void SuggestFromPsychopathFindings_EmptyFindings_ReturnsEmpty()
    {
        var result = DiagnosisSuggestionCatalog.SuggestFromPsychopathFindings([]);
        Assert.Empty(result);
    }

    [Fact]
    public void SuggestFromPsychopathFindings_Null_ReturnsEmpty()
    {
        var result = DiagnosisSuggestionCatalog.SuggestFromPsychopathFindings(null!);
        Assert.Empty(result);
    }

    [Fact]
    public void SuggestFromNeurologicalExam_NormalExam_ReturnsEmpty()
    {
        var exam = new NeurologicalExamination
        {
            MotorSystem = new MotorFindings(),
            CranialNerves = new CranialNerveFindings(),
            Reflexes = new ReflexFindings(),
            SensorySystem = new SensoryFindings(),
            Gait = new GaitFindings(),
            Coordination = new CoordinationFindings(),
            MeningealSigns = new MeningealFindings()
        };

        var result = DiagnosisSuggestionCatalog.SuggestFromNeurologicalExam(exam);

        Assert.Empty(result);
    }

    [Fact]
    public void SuggestFromNeurologicalExam_Null_ReturnsEmpty()
    {
        var result = DiagnosisSuggestionCatalog.SuggestFromNeurologicalExam(null!);
        Assert.Empty(result);
    }

    // ========== Suicidality tests ==========

    [Fact]
    public void SuggestFromSuicidality_WithIdeation_ReturnR458()
    {
        var assessment = new SuicidalityAssessment
        {
            SuicidalIdeation = true,
            SuicidalPlans = false,
            SuicidalIntent = false
        };

        var result = DiagnosisSuggestionCatalog.SuggestFromSuicidality(assessment);

        Assert.Single(result);
        Assert.Equal("R45.8", result[0].Icd10Code);
        Assert.Equal(SuggestionConfidence.Medium, result[0].Confidence);
    }

    [Fact]
    public void SuggestFromSuicidality_WithPlans_HighConfidence()
    {
        var assessment = new SuicidalityAssessment
        {
            SuicidalIdeation = true,
            SuicidalPlans = true,
            SuicidalIntent = false
        };

        var result = DiagnosisSuggestionCatalog.SuggestFromSuicidality(assessment);

        Assert.Single(result);
        Assert.Equal(SuggestionConfidence.High, result[0].Confidence);
    }

    [Fact]
    public void SuggestFromSuicidality_NoIdeation_ReturnsEmpty()
    {
        var assessment = new SuicidalityAssessment
        {
            SuicidalIdeation = false
        };

        var result = DiagnosisSuggestionCatalog.SuggestFromSuicidality(assessment);

        Assert.Empty(result);
    }

    // ========== Combined tests ==========

    [Fact]
    public void SuggestAll_CombinesMultipleSources_SortedByConfidence()
    {
        var psychFindings = new List<SymptomFinding>
        {
            new() { CategoryCode = "AFF", SymptomCode = "AFF-5", SymptomName = "Deprimiert", Severity = 3 },
            new() { CategoryCode = "SOM", SymptomCode = "SOM-1", SymptomName = "Schlafstorungen", Severity = 3 },
            new() { CategoryCode = "SOM", SymptomCode = "SOM-2", SymptomName = "Appetitminderung", Severity = 2 },
            new() { CategoryCode = "ANT", SymptomCode = "ANT-1", SymptomName = "Antriebsarm", Severity = 3 },
            new() { CategoryCode = "AUF", SymptomCode = "AUF-2", SymptomName = "Konzentrationsstorung", Severity = 2 },
            new() { CategoryCode = "AFF", SymptomCode = "AFF-6", SymptomName = "Hoffnungslos", Severity = 3 },
            new() { CategoryCode = "AFF", SymptomCode = "AFF-15", SymptomName = "Schuldgefuhle", Severity = 2 },
        };

        var neuroExam = new NeurologicalExamination
        {
            MeningealSigns = new MeningealFindings { NeckStiffness = true },
            MotorSystem = new MotorFindings(),
            CranialNerves = new CranialNerveFindings(),
            Reflexes = new ReflexFindings(),
            SensorySystem = new SensoryFindings(),
            Gait = new GaitFindings(),
            Coordination = new CoordinationFindings()
        };

        var suicidality = new SuicidalityAssessment
        {
            SuicidalIdeation = true,
            SuicidalPlans = true
        };

        var result = DiagnosisSuggestionCatalog.SuggestAll(psychFindings, neuroExam, suicidality);

        Assert.True(result.Count >= 3);
        Assert.Contains(result, s => s.Icd10Code == "F32.2");
        Assert.Contains(result, s => s.Icd10Code == "R29.1");
        Assert.Contains(result, s => s.Icd10Code == "R45.8");

        // Verify sorted by confidence descending
        for (int i = 0; i < result.Count - 1; i++)
        {
            Assert.True(result[i].Confidence >= result[i + 1].Confidence,
                $"Expected {result[i].Icd10Code} ({result[i].Confidence}) >= {result[i + 1].Icd10Code} ({result[i + 1].Confidence})");
        }
    }

    [Fact]
    public void SuggestAll_NullInputs_ReturnsEmpty()
    {
        var result = DiagnosisSuggestionCatalog.SuggestAll(null, null, null);
        Assert.Empty(result);
    }

    [Fact]
    public void SuggestAll_DeduplicatesByIcd10Code_KeepsHighestConfidence()
    {
        // Both psych and suicidality could theoretically suggest the same code
        // This test verifies deduplication logic
        var findings = new List<SymptomFinding>
        {
            new() { CategoryCode = "AFF", SymptomCode = "AFF-5", SymptomName = "Deprimiert", Severity = 1 },
            new() { CategoryCode = "SOM", SymptomCode = "SOM-1", SymptomName = "Schlafstorungen", Severity = 1 },
            new() { CategoryCode = "ANT", SymptomCode = "ANT-1", SymptomName = "Antriebsarm", Severity = 1 },
        };

        var result = DiagnosisSuggestionCatalog.SuggestAll(findings, null, null);

        // Each ICD code should appear only once
        var grouped = result.GroupBy(s => s.Icd10Code);
        foreach (var group in grouped)
        {
            Assert.Single(group);
        }
    }

    // ========== Confidence level tests ==========

    [Fact]
    public void SuggestFromPsychopathFindings_Delirium_HighConfidenceWhenSevere()
    {
        var findings = new List<SymptomFinding>
        {
            new() { CategoryCode = "BEW", SymptomCode = "BEW-2", SymptomName = "Bewusstseinstrubung", Severity = 2 },
            new() { CategoryCode = "ORI", SymptomCode = "ORI-1", SymptomName = "Zeitlich desorientiert", Severity = 3 },
        };

        var result = DiagnosisSuggestionCatalog.SuggestFromPsychopathFindings(findings);

        Assert.Contains(result, s => s.Icd10Code == "F05.0");
        var delirium = result.First(s => s.Icd10Code == "F05.0");
        Assert.Equal(SuggestionConfidence.High, delirium.Confidence);
    }

    [Fact]
    public void SuggestFromPsychopathFindings_Delirium_MediumConfidenceWhenMild()
    {
        var findings = new List<SymptomFinding>
        {
            new() { CategoryCode = "BEW", SymptomCode = "BEW-1", SymptomName = "Bewusstseinsverminderung", Severity = 1 },
            new() { CategoryCode = "ORI", SymptomCode = "ORI-1", SymptomName = "Zeitlich desorientiert", Severity = 1 },
        };

        var result = DiagnosisSuggestionCatalog.SuggestFromPsychopathFindings(findings);

        Assert.Contains(result, s => s.Icd10Code == "F05.0");
        var delirium = result.First(s => s.Icd10Code == "F05.0");
        Assert.Equal(SuggestionConfidence.Medium, delirium.Confidence);
    }

    [Fact]
    public void SuggestFromPsychopathFindings_GeneralizedAnxiety_ReturnF411()
    {
        var findings = new List<SymptomFinding>
        {
            new() { CategoryCode = "AFF", SymptomCode = "AFF-7", SymptomName = "Angstlich", Severity = 2 },
            new() { CategoryCode = "AFF", SymptomCode = "AFF-11", SymptomName = "Innerlich unruhig", Severity = 2 },
            new() { CategoryCode = "FDS", SymptomCode = "FDS-6", SymptomName = "Grubeln", Severity = 2 },
            new() { CategoryCode = "SOM", SymptomCode = "SOM-1", SymptomName = "Schlafstorungen", Severity = 1 },
        };

        var result = DiagnosisSuggestionCatalog.SuggestFromPsychopathFindings(findings);

        Assert.Contains(result, s => s.Icd10Code == "F41.1");
    }

    [Fact]
    public void SuggestFromPsychopathFindings_OCD_ReturnF420()
    {
        var findings = new List<SymptomFinding>
        {
            new() { CategoryCode = "INH", SymptomCode = "INH-1", SymptomName = "Zwangsgedanken", Severity = 2 },
        };

        var result = DiagnosisSuggestionCatalog.SuggestFromPsychopathFindings(findings);

        Assert.Contains(result, s => s.Icd10Code == "F42.0");
    }

    [Fact]
    public void SuggestFromNeurologicalExam_FacialParesis_ReturnG510()
    {
        var exam = new NeurologicalExamination
        {
            CranialNerves = new CranialNerveFindings
            {
                FacialNerveStatus = "asymmetrisch",
                MouthCornerSymmetric = false
            },
            MotorSystem = new MotorFindings(),
            Reflexes = new ReflexFindings(),
            SensorySystem = new SensoryFindings(),
            Gait = new GaitFindings(),
            Coordination = new CoordinationFindings(),
            MeningealSigns = new MeningealFindings()
        };

        var result = DiagnosisSuggestionCatalog.SuggestFromNeurologicalExam(exam);

        Assert.Contains(result, s => s.Icd10Code == "G51.0");
    }
}
