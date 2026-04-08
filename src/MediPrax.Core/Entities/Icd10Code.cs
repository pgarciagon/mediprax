namespace MediPrax.Core.Entities;

public class Icd10Code
{
    public int Id { get; set; }
    public required string Code { get; set; }
    public required string Description { get; set; }
    public string? Category { get; set; }
    public bool IsActive { get; set; } = true;
}
