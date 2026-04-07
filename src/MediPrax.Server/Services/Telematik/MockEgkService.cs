using MediPrax.Application.Interfaces.Telematik;

namespace MediPrax.Server.Services.Telematik;

public class MockEgkService : IEgkService
{
    private static readonly EgkReadResultDto[] TestCards =
    [
        new()
        {
            Success = true,
            PatientData = new EgkPatientDataDto
            {
                Kvnr = "A123456789", FirstName = "Erika", LastName = "Musterfrau",
                DateOfBirth = new DateOnly(1964, 8, 12), Gender = "W",
                Street = "Heidestraße 17", PostalCode = "51147", City = "Köln"
            },
            InsuranceData = new EgkInsuranceDataDto
            {
                InsuranceNumber = "A123456789", InsuranceProvider = "AOK Bremen/Bremerhaven",
                InsuranceProviderIknr = "101575519", InsuranceType = "GKV",
                ValidFrom = new DateOnly(2024, 1, 1), ValidUntil = new DateOnly(2029, 12, 31),
                WopKennung = "03"
            }
        },
        new()
        {
            Success = true,
            PatientData = new EgkPatientDataDto
            {
                Kvnr = "B987654321", FirstName = "Max", LastName = "Mustermann",
                DateOfBirth = new DateOnly(1975, 3, 22), Gender = "M",
                Street = "Am Wall 201", PostalCode = "28195", City = "Bremen"
            },
            InsuranceData = new EgkInsuranceDataDto
            {
                InsuranceNumber = "B987654321", InsuranceProvider = "Techniker Krankenkasse",
                InsuranceProviderIknr = "101575003", InsuranceType = "GKV",
                ValidFrom = new DateOnly(2023, 7, 1), ValidUntil = new DateOnly(2028, 6, 30),
                WopKennung = "03"
            }
        }
    ];

    private int _cardIndex;

    public async Task<EgkReadResultDto> ReadCardAsync(CancellationToken ct = default)
    {
        // Simulate card reader delay
        await Task.Delay(800, ct);
        var card = TestCards[_cardIndex % TestCards.Length];
        _cardIndex++;
        return card;
    }

    public async Task<VsdmValidationResultDto> ValidateOnlineAsync(string kvnr, CancellationToken ct = default)
    {
        await Task.Delay(500, ct);
        return new VsdmValidationResultDto
        {
            Valid = true,
            Pruefungsnachweis = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
            Timestamp = DateTime.UtcNow
        };
    }
}
