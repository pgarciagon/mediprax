namespace MediPrax.Application.Interfaces.Konnektor;

/// <summary>
/// Low-level client for the gematik Konnektor SOAP/REST API.
/// Handles authentication (TLS mutual auth with SMC-B), card operations,
/// and communication with TI services.
///
/// In production: connects to physical Konnektor (e.g., secunet, Rise) at a configured URL.
/// For development: use MockKonnektorClient.
/// </summary>
public interface IKonnektorClient
{
    /// <summary>Check connection to Konnektor and get status.</summary>
    Task<KonnektorStatusDto> GetStatusAsync(CancellationToken ct = default);

    /// <summary>Get list of inserted cards (eGK, HBA, SMC-B).</summary>
    Task<IReadOnlyList<CardInfoDto>> GetCardsAsync(CancellationToken ct = default);

    /// <summary>Read VSD (Versichertenstammdaten) from inserted eGK.</summary>
    Task<VsdResponseDto> ReadVsdAsync(string cardHandle, CancellationToken ct = default);

    /// <summary>Verify the eGK online against the Fachdienst.</summary>
    Task<OnlineCheckResponseDto> PerformOnlineCheckAsync(string cardHandle, CancellationToken ct = default);

    /// <summary>Sign data with the HBA (Heilberufsausweis) — required for E-Rezept.</summary>
    Task<SignResponseDto> SignDocumentAsync(string hbaHandle, byte[] data, CancellationToken ct = default);
}

public class KonnektorStatusDto
{
    public bool Connected { get; set; }
    public string? KonnektorVersion { get; set; }
    public string? ProductType { get; set; }
    public bool TiOnline { get; set; }
    public int InsertedCards { get; set; }
}

public class CardInfoDto
{
    public string CardHandle { get; set; } = string.Empty;
    public string CardType { get; set; } = string.Empty; // EGK, HBA, SMC_B
    public string? Kvnr { get; set; }
    public string? Iccsn { get; set; }
    public string? HolderName { get; set; }
}

public class VsdResponseDto
{
    public bool Success { get; set; }
    public string? Xml { get; set; }
    public string? ErrorCode { get; set; }
}

public class OnlineCheckResponseDto
{
    public bool Valid { get; set; }
    public string? Pruefungsnachweis { get; set; }
    public string? ErrorCode { get; set; }
}

public class SignResponseDto
{
    public bool Success { get; set; }
    public byte[]? Signature { get; set; }
    public string? ErrorCode { get; set; }
}
