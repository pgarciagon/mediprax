using MediPrax.Core.Enums;

namespace MediPrax.Application.DTOs;

public class BillingItemDto
{
    public Guid Id { get; set; }
    public Guid EncounterId { get; set; }
    public Guid PatientId { get; set; }
    public string GopCode { get; set; } = string.Empty;
    public string? GopDescription { get; set; }
    public int Quantity { get; set; }
    public BillingType BillingType { get; set; }
    public string? Quarter { get; set; }
    public bool KvdtExported { get; set; }
    public decimal Amount { get; set; }

    public string BillingTypeDisplay => BillingType switch
    {
        BillingType.EBM => "EBM",
        BillingType.GOA => "GOÄ",
        _ => BillingType.ToString()
    };
}

public class CreateBillingItemDto
{
    public Guid EncounterId { get; set; }
    public Guid PatientId { get; set; }
    public string GopCode { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;
    public BillingType BillingType { get; set; }
}

public class QuarterOverviewDto
{
    public string Quarter { get; set; } = string.Empty;
    public IReadOnlyList<PatientQuarterDto> Patients { get; set; } = [];
    public int TotalItems { get; set; }
    public decimal TotalAmount { get; set; }
}

public class PatientQuarterDto
{
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public IReadOnlyList<BillingItemDto> Items { get; set; } = [];
    public decimal TotalAmount { get; set; }
    public int ItemCount { get; set; }
}
