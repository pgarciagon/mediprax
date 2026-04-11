using MediPrax.Application.DTOs;

namespace MediPrax.Application.Interfaces;

public interface IActionChainExecutor
{
    Task<ActionChainResult> ExecuteAsync(Guid chainId, Guid encounterId, CancellationToken ct = default);
}
