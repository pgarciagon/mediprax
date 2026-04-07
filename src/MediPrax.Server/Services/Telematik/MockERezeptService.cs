using MediPrax.Application.Interfaces.Telematik;

namespace MediPrax.Server.Services.Telematik;

public class MockERezeptService : IERezeptService
{
    private static readonly List<MockERezept> _store = [];

    public async Task<ERezeptCreateResultDto> CreateAsync(ERezeptRequestDto request, CancellationToken ct = default)
    {
        await Task.Delay(600, ct);

        var id = $"160.{Random.Shared.Next(100, 999)}.{Random.Shared.Next(100000, 999999)}.{Random.Shared.Next(10000, 99999)}.{Random.Shared.Next(10, 99)}";
        var taskId = Guid.NewGuid().ToString("N")[..12];
        var accessCode = Convert.ToBase64String(Guid.NewGuid().ToByteArray())[..16];

        _store.Add(new MockERezept
        {
            ERezeptId = id,
            PatientId = request.PatientId,
            MedicationName = request.MedicationName,
            Dosierung = request.Dosierung,
            Status = "ready",
            Created = DateTime.UtcNow
        });

        return new ERezeptCreateResultDto
        {
            Success = true,
            ERezeptId = id,
            TaskId = taskId,
            AccessCode = accessCode
        };
    }

    public async Task<ERezeptStatusDto?> GetStatusAsync(string eRezeptId, CancellationToken ct = default)
    {
        await Task.Delay(300, ct);
        var item = _store.FirstOrDefault(r => r.ERezeptId == eRezeptId);
        if (item is null) return null;

        return new ERezeptStatusDto
        {
            ERezeptId = item.ERezeptId,
            Status = item.Status,
            Created = item.Created,
            Dispensed = item.Status == "completed" ? DateTime.UtcNow.AddHours(-1) : null,
            PharmacyName = item.Status == "completed" ? "Rats-Apotheke Bremen" : null
        };
    }

    public async Task<IReadOnlyList<ERezeptListItemDto>> GetByPatientAsync(Guid patientId, CancellationToken ct = default)
    {
        await Task.Delay(300, ct);
        return _store
            .Where(r => r.PatientId == patientId)
            .OrderByDescending(r => r.Created)
            .Select(r => new ERezeptListItemDto
            {
                ERezeptId = r.ERezeptId,
                MedicationName = r.MedicationName,
                Dosierung = r.Dosierung,
                Status = r.Status,
                Created = r.Created
            })
            .ToList();
    }

    public async Task CancelAsync(string eRezeptId, CancellationToken ct = default)
    {
        await Task.Delay(300, ct);
        var item = _store.FirstOrDefault(r => r.ERezeptId == eRezeptId);
        if (item is not null) item.Status = "cancelled";
    }

    private class MockERezept
    {
        public string ERezeptId { get; set; } = string.Empty;
        public Guid PatientId { get; set; }
        public string MedicationName { get; set; } = string.Empty;
        public string? Dosierung { get; set; }
        public string Status { get; set; } = "ready";
        public DateTime Created { get; set; }
    }
}
