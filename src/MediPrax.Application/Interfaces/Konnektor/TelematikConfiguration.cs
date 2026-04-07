namespace MediPrax.Application.Interfaces.Konnektor;

/// <summary>
/// Configuration for Telematikinfrastruktur connection.
/// Loaded from appsettings.json section "Telematik".
/// </summary>
public class TelematikConfiguration
{
    public const string SectionName = "Telematik";

    /// <summary>Konnektor base URL (e.g., https://192.168.0.100:443/)</summary>
    public string KonnektorUrl { get; set; } = string.Empty;

    /// <summary>Path to client certificate (PFX/P12) for mutual TLS auth.</summary>
    public string ClientCertificatePath { get; set; } = string.Empty;

    /// <summary>Password for client certificate.</summary>
    public string ClientCertificatePassword { get; set; } = string.Empty;

    /// <summary>Mandant-ID for the Konnektor context.</summary>
    public string MandantId { get; set; } = "m0001";

    /// <summary>Client-System-ID registered with gematik.</summary>
    public string ClientSystemId { get; set; } = "MediPrax";

    /// <summary>Workplace-ID for the Konnektor context.</summary>
    public string WorkplaceId { get; set; } = "w0001";

    /// <summary>E-Rezept Fachdienst URL.</summary>
    public string ERezeptFachdienstUrl { get; set; } = "https://erp-ref.zentral.erp.splitdns.ti-dienste.de/";

    /// <summary>IDP (Identity Provider) URL for token exchange.</summary>
    public string IdpUrl { get; set; } = "https://idp-ref.zentral.idp.splitdns.ti-dienste.de/";

    /// <summary>Use mock services instead of real TI (for development).</summary>
    public bool UseMock { get; set; } = true;

    /// <summary>KIM email address for this practice.</summary>
    public string KimAddress { get; set; } = string.Empty;

    /// <summary>BSNR (Betriebsstättennummer) of the practice.</summary>
    public string Bsnr { get; set; } = string.Empty;
}
