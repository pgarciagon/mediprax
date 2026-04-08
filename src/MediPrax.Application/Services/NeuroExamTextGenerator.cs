using System.Text;
using MediPrax.Core.Entities;
using MediPrax.Core.ValueObjects;

namespace MediPrax.Application.Services;

public static class NeuroExamTextGenerator
{
    public static string Generate(NeurologicalExamination exam)
    {
        var sb = new StringBuilder();

        AppendCranialNerves(sb, exam.CranialNerves);
        AppendMotor(sb, exam.MotorSystem);
        AppendReflexes(sb, exam.Reflexes);
        AppendSensory(sb, exam.SensorySystem);
        AppendCoordination(sb, exam.Coordination);
        AppendGait(sb, exam.Gait);
        AppendMeningeal(sb, exam.MeningealSigns);

        return sb.ToString().TrimEnd();
    }

    public static string GenerateFromDto(
        CranialNerveFindings cranialNerves,
        MotorFindings motor,
        ReflexFindings reflexes,
        SensoryFindings sensory,
        CoordinationFindings coordination,
        GaitFindings gait,
        MeningealFindings meningeal)
    {
        var exam = new NeurologicalExamination
        {
            CranialNerves = cranialNerves,
            MotorSystem = motor,
            Reflexes = reflexes,
            SensorySystem = sensory,
            Coordination = coordination,
            Gait = gait,
            MeningealSigns = meningeal
        };
        return Generate(exam);
    }

    private static void AppendCranialNerves(StringBuilder sb, CranialNerveFindings cn)
    {
        var defaults = new CranialNerveFindings();
        var isNormal = cn.OlfactoryStatus == defaults.OlfactoryStatus
            && cn.VisualAcuity == defaults.VisualAcuity
            && cn.VisualFields == defaults.VisualFields
            && cn.PupilsStatus == defaults.PupilsStatus
            && cn.EyeMovements == defaults.EyeMovements
            && !cn.DoubleVision
            && cn.NystagmusStatus == defaults.NystagmusStatus
            && cn.TrigeminalSensory == defaults.TrigeminalSensory
            && cn.MasseterReflex == defaults.MasseterReflex
            && cn.CornealReflex == defaults.CornealReflex
            && cn.FacialNerveStatus == defaults.FacialNerveStatus
            && cn.ForeheadWrinkleSymmetric
            && cn.MouthCornerSymmetric
            && cn.HearingStatus == defaults.HearingStatus
            && !cn.Vertigo
            && cn.PalatumStatus == defaults.PalatumStatus
            && cn.SwallowingStatus == defaults.SwallowingStatus
            && cn.GagReflex == defaults.GagReflex
            && cn.HeadRotation == defaults.HeadRotation
            && cn.ShoulderShrug == defaults.ShoulderShrug
            && cn.TongueStatus == defaults.TongueStatus
            && !cn.TongueFasciculations;

        if (isNormal)
        {
            sb.Append("Hirnnerven: Pupillen isokor, prompt reagibel. Augenbewegungen frei, kein Nystagmus. ");
            sb.Append("Mimische Muskulatur seitengleich innerviert. ");
            sb.Append("Zunge gerade herausgestreckt, keine Atrophie, keine Fibrillationen. ");
        }
        else
        {
            sb.Append("Hirnnerven: ");

            // Olfactory
            if (cn.OlfactoryStatus != defaults.OlfactoryStatus)
                sb.Append($"Geruchssinn {cn.OlfactoryStatus}. ");

            // Visual
            sb.Append($"Pupillen {cn.PupilsStatus}. ");
            if (cn.VisualAcuity != defaults.VisualAcuity)
                sb.Append($"Visus {cn.VisualAcuity}. ");
            if (cn.VisualFields != defaults.VisualFields)
                sb.Append($"Gesichtsfeld {cn.VisualFields}. ");

            // Oculomotor
            sb.Append($"Augenbewegungen {cn.EyeMovements}");
            if (cn.DoubleVision)
                sb.Append(", Doppelbilder angegeben");
            sb.Append(". ");
            if (cn.NystagmusStatus != defaults.NystagmusStatus)
                sb.Append($"{cn.NystagmusStatus}. ");

            // Trigeminal
            if (cn.TrigeminalSensory != defaults.TrigeminalSensory)
                sb.Append($"Trigeminussensibilität {cn.TrigeminalSensory}. ");

            // Facial
            sb.Append($"Mimische Muskulatur {cn.FacialNerveStatus}");
            if (!cn.ForeheadWrinkleSymmetric)
                sb.Append(", Stirnrunzeln asymmetrisch");
            if (!cn.MouthCornerSymmetric)
                sb.Append(", Mundwinkel asymmetrisch");
            sb.Append(". ");

            // Vestibulocochlear
            if (cn.HearingStatus != defaults.HearingStatus)
                sb.Append($"Gehör {cn.HearingStatus}. ");
            if (cn.Vertigo)
                sb.Append($"Schwindel angegeben{(cn.VertigoComment is not null ? $" ({cn.VertigoComment})" : "")}. ");

            // Glossopharyngeal/Vagus
            if (cn.SwallowingStatus != defaults.SwallowingStatus)
                sb.Append($"Schlucken {cn.SwallowingStatus}. ");

            // Hypoglossal
            sb.Append($"Zunge {cn.TongueStatus}");
            if (cn.TongueFasciculations)
                sb.Append(", Faszikulationen vorhanden");
            sb.Append(". ");
        }
    }

    private static void AppendMotor(StringBuilder sb, MotorFindings m)
    {
        var allStrength5 = m.StrengthRightArm == 5 && m.StrengthLeftArm == 5
            && m.StrengthRightLeg == 5 && m.StrengthLeftLeg == 5;
        var allToneNormal = m.ToneRightArm == "normal" && m.ToneLeftArm == "normal"
            && m.ToneRightLeg == "normal" && m.ToneLeftLeg == "normal";
        var allBulkNormal = m.BulkRightArm == "normal" && m.BulkLeftArm == "normal"
            && m.BulkRightLeg == "normal" && m.BulkLeftLeg == "normal";
        var isNormal = allStrength5 && allToneNormal && allBulkNormal && !m.FasciculationsPresent;

        if (isNormal)
        {
            sb.Append("Motorik: Kraftgrade allseits 5/5, Muskeltonus unauffällig, keine Atrophien. ");
        }
        else
        {
            sb.Append("Motorik: ");

            // Strength
            if (allStrength5)
            {
                sb.Append("Kraftgrade allseits 5/5. ");
            }
            else
            {
                var parts = new List<string>();
                if (m.StrengthRightArm < 5) parts.Add($"rechter Arm {m.StrengthRightArm}/5");
                if (m.StrengthLeftArm < 5) parts.Add($"linker Arm {m.StrengthLeftArm}/5");
                if (m.StrengthRightLeg < 5) parts.Add($"rechtes Bein {m.StrengthRightLeg}/5");
                if (m.StrengthLeftLeg < 5) parts.Add($"linkes Bein {m.StrengthLeftLeg}/5");
                sb.Append($"Kraftgrad {string.Join(", ", parts)}. ");
            }

            // Tone
            if (allToneNormal)
            {
                sb.Append("Muskeltonus unauffällig. ");
            }
            else
            {
                var parts = new List<string>();
                if (m.ToneRightArm != "normal") parts.Add($"rechter Arm {m.ToneRightArm}");
                if (m.ToneLeftArm != "normal") parts.Add($"linker Arm {m.ToneLeftArm}");
                if (m.ToneRightLeg != "normal") parts.Add($"rechtes Bein {m.ToneRightLeg}");
                if (m.ToneLeftLeg != "normal") parts.Add($"linkes Bein {m.ToneLeftLeg}");
                sb.Append($"Tonus {string.Join(", ", parts)}. ");
            }

            // Bulk
            if (!allBulkNormal)
            {
                var parts = new List<string>();
                if (m.BulkRightArm != "normal") parts.Add($"rechter Arm {m.BulkRightArm}");
                if (m.BulkLeftArm != "normal") parts.Add($"linker Arm {m.BulkLeftArm}");
                if (m.BulkRightLeg != "normal") parts.Add($"rechtes Bein {m.BulkRightLeg}");
                if (m.BulkLeftLeg != "normal") parts.Add($"linkes Bein {m.BulkLeftLeg}");
                sb.Append($"Trophik: {string.Join(", ", parts)}. ");
            }
            else
            {
                sb.Append("Keine Atrophien. ");
            }

            if (m.FasciculationsPresent)
                sb.Append($"Faszikulationen vorhanden{(m.FasciculationsLocation is not null ? $" ({m.FasciculationsLocation})" : "")}. ");
        }
    }

    private static void AppendReflexes(StringBuilder sb, ReflexFindings r)
    {
        var allNormal = r.BicepsRight == 2 && r.BicepsLeft == 2
            && r.TricepsRight == 2 && r.TricepsLeft == 2
            && r.BrachioradialRight == 2 && r.BrachioradialLeft == 2
            && r.PatellarRight == 2 && r.PatellarLeft == 2
            && r.AchillesRight == 2 && r.AchillesLeft == 2;
        var noPathological = !r.BabinskiRight && !r.BabinskiLeft
            && !r.HoffmannTroemnerRight && !r.HoffmannTroemnerLeft
            && !r.GordonRight && !r.GordonLeft;

        if (allNormal && noPathological)
        {
            sb.Append("Muskeleigenreflexe seitengleich mittellebhaft auslösbar (BSR, TSR, RPR, PSR, ASR). ");
            sb.Append("Keine pathologischen Reflexe, insbesondere kein Babinski. ");
        }
        else
        {
            sb.Append("Reflexe: ");

            // Describe specific reflexes if abnormal
            var reflexParts = new List<string>();
            DescribeReflex(reflexParts, "BSR", r.BicepsRight, r.BicepsLeft);
            DescribeReflex(reflexParts, "TSR", r.TricepsRight, r.TricepsLeft);
            DescribeReflex(reflexParts, "RPR", r.BrachioradialRight, r.BrachioradialLeft);
            DescribeReflex(reflexParts, "PSR", r.PatellarRight, r.PatellarLeft);
            DescribeReflex(reflexParts, "ASR", r.AchillesRight, r.AchillesLeft);

            if (reflexParts.Count > 0)
                sb.Append($"{string.Join(", ", reflexParts)}. ");

            // Pathological
            var pathParts = new List<string>();
            if (r.BabinskiRight || r.BabinskiLeft)
            {
                var side = (r.BabinskiRight, r.BabinskiLeft) switch
                {
                    (true, true) => "beidseits",
                    (true, false) => "rechts",
                    _ => "links"
                };
                pathParts.Add($"Babinski {side} positiv");
            }
            if (r.HoffmannTroemnerRight || r.HoffmannTroemnerLeft)
            {
                var side = (r.HoffmannTroemnerRight, r.HoffmannTroemnerLeft) switch
                {
                    (true, true) => "beidseits",
                    (true, false) => "rechts",
                    _ => "links"
                };
                pathParts.Add($"Hoffmann-Trömner {side} positiv");
            }
            if (r.GordonRight || r.GordonLeft)
            {
                var side = (r.GordonRight, r.GordonLeft) switch
                {
                    (true, true) => "beidseits",
                    (true, false) => "rechts",
                    _ => "links"
                };
                pathParts.Add($"Gordon {side} positiv");
            }

            if (pathParts.Count > 0)
                sb.Append($"{string.Join(", ", pathParts)}. ");
            else
                sb.Append("Keine pathologischen Reflexe. ");
        }
    }

    private static void DescribeReflex(List<string> parts, string name, int right, int left)
    {
        if (right == 2 && left == 2) return;

        var rightDesc = ReflexGradeText(right);
        var leftDesc = ReflexGradeText(left);

        if (right == left)
            parts.Add($"{name} beidseits {rightDesc}");
        else
            parts.Add($"{name} rechts {rightDesc}, links {leftDesc}");
    }

    private static string ReflexGradeText(int grade) => grade switch
    {
        0 => "nicht auslösbar",
        1 => "abgeschwächt",
        2 => "mittellebhaft",
        3 => "lebhaft",
        4 => "gesteigert mit Kloni",
        _ => grade.ToString()
    };

    private static void AppendSensory(StringBuilder sb, SensoryFindings s)
    {
        var defaults = new SensoryFindings();
        var isNormal = s.TouchSensation == defaults.TouchSensation
            && s.PainSensation == defaults.PainSensation
            && s.TemperatureSensation == defaults.TemperatureSensation
            && s.VibrationSenseRight == defaults.VibrationSenseRight
            && s.VibrationSenseLeft == defaults.VibrationSenseLeft
            && s.Proprioception == defaults.Proprioception;

        if (isNormal)
        {
            sb.Append("Sensibilität: Berührungs-, Schmerz- und Temperaturempfinden intakt. ");
            sb.Append("Pallästhesie beidseits normal. Lagesinn erhalten. ");
        }
        else
        {
            sb.Append("Sensibilität: ");

            if (s.TouchSensation != defaults.TouchSensation)
                sb.Append($"Berührungsempfinden {s.TouchSensation}{(s.TouchDistribution is not null ? $" ({s.TouchDistribution})" : "")}. ");
            else
                sb.Append("Berührungsempfinden intakt. ");

            if (s.PainSensation != defaults.PainSensation)
                sb.Append($"Schmerzempfinden {s.PainSensation}{(s.PainDistribution is not null ? $" ({s.PainDistribution})" : "")}. ");

            if (s.TemperatureSensation != defaults.TemperatureSensation)
                sb.Append($"Temperaturempfinden {s.TemperatureSensation}{(s.TemperatureDistribution is not null ? $" ({s.TemperatureDistribution})" : "")}. ");

            // Vibration
            if (s.VibrationSenseRight == s.VibrationSenseLeft)
                sb.Append($"Pallästhesie beidseits {s.VibrationSenseRight}. ");
            else
                sb.Append($"Pallästhesie rechts {s.VibrationSenseRight}, links {s.VibrationSenseLeft}. ");

            sb.Append($"Lagesinn {s.Proprioception}. ");
        }
    }

    private static void AppendCoordination(StringBuilder sb, CoordinationFindings c)
    {
        var defaults = new CoordinationFindings();
        var isNormal = c.FingerNoseTest == defaults.FingerNoseTest
            && c.HeelShinTest == defaults.HeelShinTest
            && c.RombergTest == defaults.RombergTest
            && c.TandemGait == defaults.TandemGait
            && c.Diadochokinesis == defaults.Diadochokinesis;

        if (isNormal)
        {
            sb.Append("Koordination: Finger-Nase-Versuch zielsicher, Knie-Hacke-Versuch zielsicher. ");
            sb.Append("Romberg-Stehversuch sicher. Diadochokinese unauffällig. ");
        }
        else
        {
            sb.Append("Koordination: ");
            sb.Append($"Finger-Nase-Versuch {c.FingerNoseTest}, ");
            sb.Append($"Knie-Hacke-Versuch {c.HeelShinTest}. ");
            sb.Append($"Romberg-Stehversuch {c.RombergTest}. ");
            if (c.TandemGait != defaults.TandemGait)
                sb.Append($"Tandemgang {c.TandemGait}. ");
            sb.Append($"Diadochokinese {c.Diadochokinesis}. ");
        }
    }

    private static void AppendGait(StringBuilder sb, GaitFindings g)
    {
        var defaults = new GaitFindings();
        var isNormal = g.GaitPattern == defaults.GaitPattern
            && g.ArmSwing == defaults.ArmSwing
            && g.Stability == defaults.Stability;

        if (isNormal)
        {
            sb.Append("Gangbild: Unauffällig, Armpendel seitengleich. ");
        }
        else
        {
            sb.Append($"Gangbild: {g.GaitPattern}, Armpendel {g.ArmSwing}, Stabilität {g.Stability}. ");
        }
    }

    private static void AppendMeningeal(StringBuilder sb, MeningealFindings m)
    {
        var isNormal = !m.NeckStiffness && !m.KernigSign && !m.BrudzinskiSign
            && m.LasegueRight is null && m.LasegueLeft is null;

        if (isNormal)
        {
            sb.Append("Meningismus: Kein Meningismus, Lasègue-Zeichen beidseits negativ.");
        }
        else
        {
            sb.Append("Meningismus: ");
            var parts = new List<string>();

            if (m.NeckStiffness) parts.Add("Nackensteifigkeit vorhanden");
            if (m.KernigSign) parts.Add("Kernig-Zeichen positiv");
            if (m.BrudzinskiSign) parts.Add("Brudzinski-Zeichen positiv");

            if (m.LasegueRight is not null || m.LasegueLeft is not null)
            {
                var rightText = m.LasegueRight is not null ? $"rechts positiv bei {m.LasegueRight}°" : "rechts negativ";
                var leftText = m.LasegueLeft is not null ? $"links positiv bei {m.LasegueLeft}°" : "links negativ";
                parts.Add($"Lasègue-Zeichen {rightText}, {leftText}");
            }
            else
            {
                parts.Add("Lasègue-Zeichen beidseits negativ");
            }

            sb.Append($"{string.Join(", ", parts)}.");
        }
    }
}
