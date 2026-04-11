using System.Text;
using MediPrax.Application.Services;

namespace MediPrax.IntegrationTests;

[Collection("Postgres")]
public class ImportServiceTests : IDisposable
{
    private readonly TestDbContextFactory _factory;
    private readonly ImportService _sut;

    public ImportServiceTests(PostgresFixture postgres)
    {
        _factory = new TestDbContextFactory(postgres.ConnectionString);
        _sut = new ImportService(_factory.Context);
    }

    [Fact]
    public async Task ImportPatientsCsvAsync_ImportsPatients()
    {
        var csv = "Nachname;Vorname;Geburtsdatum;Geschlecht;KVNR\nMeier;Hans;01.03.1960;M;X111222333\nSchmidt;Anna;15.07.1985;W;X444555666";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));

        var result = await _sut.ImportPatientsCsvAsync(stream);

        Assert.Equal(2, result.Imported);
        Assert.Equal(0, result.Skipped);
        Assert.Equal(0, result.Errors);
    }

    [Fact]
    public async Task ImportPatientsCsvAsync_SkipsDuplicates()
    {
        var csv = "Nachname;Vorname;Geburtsdatum\nDupTest;Klaus;22.09.1970";
        using var stream1 = new MemoryStream(Encoding.UTF8.GetBytes(csv));
        await _sut.ImportPatientsCsvAsync(stream1);

        using var stream2 = new MemoryStream(Encoding.UTF8.GetBytes(csv));
        var result = await _sut.ImportPatientsCsvAsync(stream2);

        Assert.Equal(0, result.Imported);
        Assert.Equal(1, result.Skipped);
    }

    [Fact]
    public async Task ImportPatientsCsvAsync_ReportsErrors()
    {
        var csv = "Nachname;Vorname;Geburtsdatum\nBadDate;Test;not-a-date";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));

        var result = await _sut.ImportPatientsCsvAsync(stream);

        Assert.Equal(0, result.Imported);
        Assert.Equal(1, result.Errors);
        Assert.Contains(result.ErrorMessages, m => m.Contains("Geburtsdatum"));
    }

    [Fact]
    public async Task ImportPatientsCsvAsync_EmptyFile_ReturnsError()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(""));

        var result = await _sut.ImportPatientsCsvAsync(stream);

        Assert.Contains(result.ErrorMessages, m => m.Contains("Leere"));
    }

    [Fact]
    public async Task ImportPatientsCsvAsync_WithFullData()
    {
        var csv = "Nachname;Vorname;Geburtsdatum;Geschlecht;KVNR;Versichertennr;Krankenkasse;Versicherungsart;Straße;PLZ;Ort;Telefon;Email\n" +
                  "Vollständig;Maria;05.12.1978;W;V123456789;12345;AOK Bremen;GKV;Musterstr. 1;28195;Bremen;0421-12345;maria@test.de";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));

        var result = await _sut.ImportPatientsCsvAsync(stream);

        Assert.Equal(1, result.Imported);
    }

    public void Dispose() => _factory.Dispose();
}
