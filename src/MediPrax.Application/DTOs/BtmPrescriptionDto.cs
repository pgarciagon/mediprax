namespace MediPrax.Application.DTOs;

public class BtmPrescriptionDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public Guid PrescribedById { get; set; }
    public string PrescribedByName { get; set; } = string.Empty;
    public DateOnly PrescriptionDate { get; set; }
    public string MedicationName { get; set; } = string.Empty;
    public string? Pzn { get; set; }
    public string Substance { get; set; } = string.Empty;
    public string Amount { get; set; } = string.Empty;
    public string? Dosierung { get; set; }
    public string BtmRecipeNumber { get; set; } = string.Empty;
    public string? PrescriberBtmNumber { get; set; }
    public bool IsEBtm { get; set; }
    public string? Notes { get; set; }
}

public class CreateBtmPrescriptionDto
{
    public Guid PatientId { get; set; }
    public Guid PrescribedById { get; set; }
    public DateOnly PrescriptionDate { get; set; }
    public string MedicationName { get; set; } = string.Empty;
    public string? Pzn { get; set; }
    public string Substance { get; set; } = string.Empty;
    public string Amount { get; set; } = string.Empty;
    public string? Dosierung { get; set; }
    public string BtmRecipeNumber { get; set; } = string.Empty;
    public string? PrescriberBtmNumber { get; set; }
    public bool IsEBtm { get; set; }
    public string? Notes { get; set; }
}

public class BtmComplianceCheckDto
{
    public bool IsCompliant { get; set; }
    public List<string> Warnings { get; set; } = [];
    public string? MaxQuantity { get; set; }
    public string? PrescribedQuantity { get; set; }
}

public class BtmSummaryDto
{
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string Substance { get; set; } = string.Empty;
    public decimal TotalAmount30Days { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal? MaxAllowed { get; set; }
    public bool ExceedsLimit { get; set; }
}
