namespace MediPrax.Core.ValueObjects;

public class CranialNerveFindings
{
    // I - Olfactory
    public string OlfactoryStatus { get; set; } = "normal"; // normal, eingeschränkt, aufgehoben
    public string? OlfactoryComment { get; set; }

    // II - Visual
    public string VisualAcuity { get; set; } = "normal";
    public string VisualFields { get; set; } = "normal"; // normal, eingeschränkt + description
    public string PupilsStatus { get; set; } = "isokor, prompt reagibel";
    public string? FundoscopyComment { get; set; }

    // III, IV, VI - Oculomotor
    public string EyeMovements { get; set; } = "frei in alle Richtungen";
    public bool DoubleVision { get; set; }
    public string NystagmusStatus { get; set; } = "kein Nystagmus";
    public string? NystagmusComment { get; set; }

    // V - Trigeminal
    public string TrigeminalSensory { get; set; } = "seitengleich intakt";
    public string MasseterReflex { get; set; } = "normal";
    public string CornealReflex { get; set; } = "beidseits auslösbar";

    // VII - Facial
    public string FacialNerveStatus { get; set; } = "seitengleich innerviert";
    public bool ForeheadWrinkleSymmetric { get; set; } = true;
    public bool MouthCornerSymmetric { get; set; } = true;
    public string? FacialComment { get; set; }

    // VIII - Vestibulocochlear
    public string HearingStatus { get; set; } = "beidseits intakt";
    public bool Vertigo { get; set; }
    public string? VertigoComment { get; set; }

    // IX, X - Glossopharyngeal/Vagus
    public string PalatumStatus { get; set; } = "Gaumensegel hebt seitengleich";
    public string SwallowingStatus { get; set; } = "ungestört";
    public string GagReflex { get; set; } = "beidseits auslösbar";

    // XI - Accessory
    public string HeadRotation { get; set; } = "seitengleich kräftig";
    public string ShoulderShrug { get; set; } = "seitengleich kräftig";

    // XII - Hypoglossal
    public string TongueStatus { get; set; } = "gerade herausgestreckt, keine Atrophie";
    public bool TongueFasciculations { get; set; }
}
