using MediPrax.Application.Interfaces.Konnektor;

namespace MediPrax.Server.Services.Telematik;

public class MockKonnektorClient : IKonnektorClient
{
    public async Task<KonnektorStatusDto> GetStatusAsync(CancellationToken ct = default)
    {
        await Task.Delay(200, ct);
        return new KonnektorStatusDto
        {
            Connected = true,
            KonnektorVersion = "5.50.0 (Mock)",
            ProductType = "MediPrax Development Simulator",
            TiOnline = true,
            InsertedCards = 2
        };
    }

    public async Task<IReadOnlyList<CardInfoDto>> GetCardsAsync(CancellationToken ct = default)
    {
        await Task.Delay(200, ct);
        return
        [
            new() { CardHandle = "egk-handle-001", CardType = "EGK", Kvnr = "A123456789", HolderName = "Erika Musterfrau" },
            new() { CardHandle = "hba-handle-001", CardType = "HBA", Iccsn = "80276883110000000001", HolderName = "Dr. Thomas Müller" }
        ];
    }

    public async Task<VsdResponseDto> ReadVsdAsync(string cardHandle, CancellationToken ct = default)
    {
        await Task.Delay(500, ct);
        return new VsdResponseDto
        {
            Success = true,
            Xml = "<UC_PersoenlicheVersichertendatenXML><Nachname>Musterfrau</Nachname><Vorname>Erika</Vorname></UC_PersoenlicheVersichertendatenXML>"
        };
    }

    public async Task<OnlineCheckResponseDto> PerformOnlineCheckAsync(string cardHandle, CancellationToken ct = default)
    {
        await Task.Delay(400, ct);
        return new OnlineCheckResponseDto
        {
            Valid = true,
            Pruefungsnachweis = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
        };
    }

    public async Task<SignResponseDto> SignDocumentAsync(string hbaHandle, byte[] data, CancellationToken ct = default)
    {
        await Task.Delay(300, ct);
        return new SignResponseDto
        {
            Success = true,
            Signature = Guid.NewGuid().ToByteArray()
        };
    }
}
