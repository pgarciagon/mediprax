using MediPrax.Application.DTOs;
using MediPrax.Application.Interfaces.Telematik;
using MediPrax.Core.Enums;

namespace MediPrax.Server.Services.Telematik;

public class MockEauService : IEauService
{
    private static readonly Dictionary<Guid, EauDto> _store = [];

    public async Task<EauDto> TransmitAsync(CreateEauDto dto, CancellationToken ct = default)
    {
        await Task.Delay(500, ct);

        var eau = new EauDto
        {
            Id = Guid.NewGuid(),
            PatientId = dto.PatientId,
            Von = dto.Von,
            Bis = dto.Bis,
            Diagnose = dto.Diagnose,
            IcdCode = dto.IcdCode,
            IsErstbescheinigung = dto.IsErstbescheinigung,
            Status = EauStatus.Transmitted,
            TransmissionId = $"eAU-{Random.Shared.Next(100000, 999999)}",
            TransmittedAt = DateTime.UtcNow
        };

        _store[eau.Id] = eau;
        return eau;
    }

    public async Task<EauDto?> GetStatusAsync(Guid eauId, CancellationToken ct = default)
    {
        await Task.Delay(200, ct);

        if (!_store.TryGetValue(eauId, out var eau))
            return null;

        // Simulate confirmation after transmission
        if (eau.Status == EauStatus.Transmitted)
        {
            eau.Status = EauStatus.Confirmed;
            _store[eauId] = eau;
        }

        return eau;
    }

    public async Task CancelAsync(Guid eauId, CancellationToken ct = default)
    {
        await Task.Delay(300, ct);

        if (_store.TryGetValue(eauId, out var eau))
        {
            eau.Status = EauStatus.Cancelled;
            _store[eauId] = eau;
        }
    }
}
