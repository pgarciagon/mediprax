using MediPrax.Application.Services;
using MediPrax.Core.Entities;
using MediPrax.Core.ValueObjects;

namespace MediPrax.UnitTests;

public class NeuroExamTextGeneratorTests
{
    [Fact]
    public void Generate_NormalExam_ProducesExpectedText()
    {
        var exam = new NeurologicalExamination
        {
            CranialNerves = new CranialNerveFindings(),
            MotorSystem = new MotorFindings(),
            Reflexes = new ReflexFindings(),
            SensorySystem = new SensoryFindings(),
            Coordination = new CoordinationFindings(),
            Gait = new GaitFindings(),
            MeningealSigns = new MeningealFindings()
        };

        var text = NeuroExamTextGenerator.Generate(exam);

        Assert.Contains("Pupillen isokor, prompt reagibel", text);
        Assert.Contains("Kraftgrade allseits 5/5", text);
        Assert.Contains("Muskeleigenreflexe seitengleich mittellebhaft auslösbar", text);
        Assert.Contains("Kein Meningismus", text);
        Assert.Contains("Finger-Nase-Versuch zielsicher", text);
        Assert.Contains("Unauffällig, Armpendel seitengleich", text);
    }

    [Fact]
    public void Generate_MotorWeakness_MentionsWeaknessInText()
    {
        var exam = new NeurologicalExamination
        {
            CranialNerves = new CranialNerveFindings(),
            MotorSystem = new MotorFindings { StrengthRightArm = 3, ToneRightArm = "spastisch" },
            Reflexes = new ReflexFindings(),
            SensorySystem = new SensoryFindings(),
            Coordination = new CoordinationFindings(),
            Gait = new GaitFindings(),
            MeningealSigns = new MeningealFindings()
        };

        var text = NeuroExamTextGenerator.Generate(exam);

        Assert.Contains("rechter Arm 3/5", text);
        Assert.Contains("spastisch", text);
    }

    [Fact]
    public void Generate_BabinskiPresent_MentionsPathologicalReflex()
    {
        var exam = new NeurologicalExamination
        {
            CranialNerves = new CranialNerveFindings(),
            MotorSystem = new MotorFindings(),
            Reflexes = new ReflexFindings { BabinskiRight = true },
            SensorySystem = new SensoryFindings(),
            Coordination = new CoordinationFindings(),
            Gait = new GaitFindings(),
            MeningealSigns = new MeningealFindings()
        };

        var text = NeuroExamTextGenerator.Generate(exam);

        Assert.Contains("Babinski rechts positiv", text);
    }

    [Fact]
    public void Generate_CoordinationAbnormal_MentionsProblems()
    {
        var exam = new NeurologicalExamination
        {
            CranialNerves = new CranialNerveFindings(),
            MotorSystem = new MotorFindings(),
            Reflexes = new ReflexFindings(),
            SensorySystem = new SensoryFindings(),
            Coordination = new CoordinationFindings
            {
                FingerNoseTest = "Dysmetrie",
                RombergTest = "pathologisch",
                Diadochokinesis = "Dysdiadochokinese"
            },
            Gait = new GaitFindings(),
            MeningealSigns = new MeningealFindings()
        };

        var text = NeuroExamTextGenerator.Generate(exam);

        Assert.Contains("Dysmetrie", text);
        Assert.Contains("pathologisch", text);
        Assert.Contains("Dysdiadochokinese", text);
    }

    [Fact]
    public void Generate_MeningealSigns_HandledCorrectly()
    {
        var exam = new NeurologicalExamination
        {
            CranialNerves = new CranialNerveFindings(),
            MotorSystem = new MotorFindings(),
            Reflexes = new ReflexFindings(),
            SensorySystem = new SensoryFindings(),
            Coordination = new CoordinationFindings(),
            Gait = new GaitFindings(),
            MeningealSigns = new MeningealFindings
            {
                NeckStiffness = true,
                KernigSign = true,
                LasegueRight = 45
            }
        };

        var text = NeuroExamTextGenerator.Generate(exam);

        Assert.Contains("Nackensteifigkeit", text);
        Assert.Contains("Kernig-Zeichen positiv", text);
        Assert.Contains("rechts positiv bei 45", text);
    }
}
