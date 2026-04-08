using MediPrax.Core.Enums;

namespace MediPrax.Application.DTOs;

public class PtvFormDto
{
    public Guid Id { get; set; }
    public Guid TherapyCaseId { get; set; }
    public PtvFormType FormType { get; set; }
    public string FormTypeName { get; set; } = string.Empty;
    public DateOnly CreatedDate { get; set; }
    public PtvFormStatus Status { get; set; }
    public bool HasPdf { get; set; }
}

public class CreatePtvFormDto
{
    public Guid TherapyCaseId { get; set; }
    public PtvFormType FormType { get; set; }
}
