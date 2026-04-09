namespace MediPrax.Application.DTOs;

public class ParkinsonDocumentationDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public DateOnly DocumentationDate { get; set; }
    public decimal HoehnYahrStage { get; set; }
    public int Tremor { get; set; }
    public int Rigidity { get; set; }
    public int Bradykinesia { get; set; }
    public int PosturalInstability { get; set; }
    public bool OnOffFluctuations { get; set; }
    public bool Dyskinesia { get; set; }
    public bool FreezeOfGait { get; set; }
    public List<string>? NonMotorSymptoms { get; set; }
    public string? Notes { get; set; }

    public string HoehnYahrDisplay => HoehnYahrStage switch
    {
        0 => "0 — Keine Zeichen",
        1 => "1 — Einseitig",
        1.5m => "1.5 — Einseitig + axial",
        2 => "2 — Beidseitig ohne Gleichgewichtsstörung",
        2.5m => "2.5 — Leicht beidseitig",
        3 => "3 — Leichte bis mäßige Behinderung",
        4 => "4 — Schwere Behinderung, noch gehfähig",
        5 => "5 — Rollstuhl/bettlägerig",
        _ => $"{HoehnYahrStage:F1}"
    };

    public int MotorScore => Tremor + Rigidity + Bradykinesia + PosturalInstability;
}

public class CreateParkinsonDocumentationDto
{
    public Guid PatientId { get; set; }
    public DateOnly DocumentationDate { get; set; }
    public decimal HoehnYahrStage { get; set; }
    public int Tremor { get; set; }
    public int Rigidity { get; set; }
    public int Bradykinesia { get; set; }
    public int PosturalInstability { get; set; }
    public bool OnOffFluctuations { get; set; }
    public bool Dyskinesia { get; set; }
    public bool FreezeOfGait { get; set; }
    public List<string>? NonMotorSymptoms { get; set; }
    public string? Notes { get; set; }
}
