namespace MediPrax.Application.DTOs;

public class Icd10CodeDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Category { get; set; }
    public bool IsActive { get; set; }
}

public class CreateIcd10CodeDto
{
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Category { get; set; }
}

public class UpdateIcd10CodeDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Category { get; set; }
    public bool IsActive { get; set; }
}
