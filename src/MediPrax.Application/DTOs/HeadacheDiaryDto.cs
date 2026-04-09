namespace MediPrax.Application.DTOs;

public class HeadacheDiaryDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public TimeOnly? StartTime { get; set; }
    public decimal? DurationHours { get; set; }
    public int Intensity { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? Location { get; set; }
    public bool AuraPresent { get; set; }
    public bool Nausea { get; set; }
    public bool Photophobia { get; set; }
    public bool Phonophobia { get; set; }
    public List<string>? Triggers { get; set; }
    public string? MedicationTaken { get; set; }
    public bool? MedicationEffective { get; set; }

    public string IntensityDisplay => Intensity switch
    {
        <= 3 => $"{Intensity}/10 (leicht)",
        <= 6 => $"{Intensity}/10 (mittel)",
        _ => $"{Intensity}/10 (stark)"
    };
}

public class CreateHeadacheDiaryDto
{
    public Guid PatientId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly? StartTime { get; set; }
    public decimal? DurationHours { get; set; }
    public int Intensity { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? Location { get; set; }
    public bool AuraPresent { get; set; }
    public bool Nausea { get; set; }
    public bool Photophobia { get; set; }
    public bool Phonophobia { get; set; }
    public List<string>? Triggers { get; set; }
    public string? MedicationTaken { get; set; }
    public bool? MedicationEffective { get; set; }
}
