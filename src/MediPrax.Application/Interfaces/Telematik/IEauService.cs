using MediPrax.Application.DTOs;

namespace MediPrax.Application.Interfaces.Telematik;

/// <summary>
/// eAU — elektronische Arbeitsunfähigkeitsbescheinigung via Telematikinfrastruktur.
/// In production: communicates with KV-Server via TI-Gateway.
/// </summary>
public interface IEauService
{
    Task<EauDto> TransmitAsync(CreateEauDto dto, CancellationToken ct = default);
    Task<EauDto?> GetStatusAsync(Guid eauId, CancellationToken ct = default);
    Task CancelAsync(Guid eauId, CancellationToken ct = default);
}
