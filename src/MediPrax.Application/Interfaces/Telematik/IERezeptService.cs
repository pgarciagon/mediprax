namespace MediPrax.Application.Interfaces.Telematik;

/// <summary>
/// E-Rezept — elektronische Verordnung via gematik E-Rezept-Fachdienst.
/// In production: communicates with E-Rezept-Fachdienst via FHIR REST API.
/// </summary>
public interface IERezeptService
{
    Task<ERezeptCreateResultDto> CreateAsync(ERezeptRequestDto request, CancellationToken ct = default);
    Task<ERezeptStatusDto?> GetStatusAsync(string eRezeptId, CancellationToken ct = default);
    Task<IReadOnlyList<ERezeptListItemDto>> GetByPatientAsync(Guid patientId, CancellationToken ct = default);
    Task CancelAsync(string eRezeptId, CancellationToken ct = default);
}

public class ERezeptRequestDto
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public string Kvnr { get; set; } = string.Empty;
    public string MedicationPzn { get; set; } = string.Empty;
    public string MedicationName { get; set; } = string.Empty;
    public string? Dosierung { get; set; }
    public int Quantity { get; set; } = 1;
    public bool IsBtm { get; set; }
    public bool IsSubstitutionAllowed { get; set; } = true;
    public string? Notes { get; set; }
}

public class ERezeptCreateResultDto
{
    public bool Success { get; set; }
    public string? ERezeptId { get; set; }
    public string? TaskId { get; set; }
    public string? AccessCode { get; set; }
    public byte[]? QrCodePng { get; set; }
    public string? ErrorMessage { get; set; }
}

public class ERezeptStatusDto
{
    public string ERezeptId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime Created { get; set; }
    public DateTime? Dispensed { get; set; }
    public string? PharmacyName { get; set; }
}

public class ERezeptListItemDto
{
    public string ERezeptId { get; set; } = string.Empty;
    public string MedicationName { get; set; } = string.Empty;
    public string? Dosierung { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime Created { get; set; }
}
