namespace MediPrax.Application.Interfaces.Telematik;

/// <summary>
/// VSDM — Versichertenstammdatenmanagement.
/// Reads patient insurance data from the eGK (elektronische Gesundheitskarte).
/// In production: communicates with Konnektor via SOAP/REST.
/// </summary>
public interface IEgkService
{
    Task<EgkReadResultDto> ReadCardAsync(CancellationToken ct = default);
    Task<VsdmValidationResultDto> ValidateOnlineAsync(string kvnr, CancellationToken ct = default);
}

public class EgkReadResultDto
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public EgkPatientDataDto? PatientData { get; set; }
    public EgkInsuranceDataDto? InsuranceData { get; set; }
}

public class EgkPatientDataDto
{
    public string Kvnr { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? Street { get; set; }
    public string? PostalCode { get; set; }
    public string? City { get; set; }
}

public class EgkInsuranceDataDto
{
    public string InsuranceNumber { get; set; } = string.Empty;
    public string InsuranceProvider { get; set; } = string.Empty;
    public string InsuranceProviderIknr { get; set; } = string.Empty;
    public string InsuranceType { get; set; } = "GKV";
    public DateOnly ValidFrom { get; set; }
    public DateOnly ValidUntil { get; set; }
    public string? WopKennung { get; set; }
}

public class VsdmValidationResultDto
{
    public bool Valid { get; set; }
    public string? Pruefungsnachweis { get; set; }
    public DateTime Timestamp { get; set; }
    public string? ErrorCode { get; set; }
}
