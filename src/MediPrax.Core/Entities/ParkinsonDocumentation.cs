namespace MediPrax.Core.Entities;

public class ParkinsonDocumentation : BaseEntity
{
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public DateOnly DocumentationDate { get; set; }
    public decimal HoehnYahrStage { get; set; } // 0–5
    public int Tremor { get; set; }             // 0–4
    public int Rigidity { get; set; }           // 0–4
    public int Bradykinesia { get; set; }       // 0–4
    public int PosturalInstability { get; set; } // 0–4
    public bool OnOffFluctuations { get; set; }
    public bool Dyskinesia { get; set; }
    public bool FreezeOfGait { get; set; }
    public List<string>? NonMotorSymptoms { get; set; }
    public string? Notes { get; set; }
}
