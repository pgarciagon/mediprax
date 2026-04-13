using MediPrax.Application.DTOs;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;
using MediPrax.Server.Services;

namespace MediPrax.IntegrationTests;

[Collection("Postgres")]
public class ArztbriefServiceTests : IDisposable
{
    private readonly TestDbContextFactory _factory;
    private readonly ArztbriefService _sut;
    private readonly Guid _patientId;
    private readonly Guid _doctorId;

    public ArztbriefServiceTests(PostgresFixture postgres)
    {
        _factory = new TestDbContextFactory(postgres.ConnectionString);
        _sut = new ArztbriefService(_factory.Context);

        var patient = new Patient
        {
            FirstName = "Brief", LastName = "Patient",
            DateOfBirth = new DateOnly(1975, 3, 20),
            InsuranceType = InsuranceType.GKV,
            InsuranceProvider = "AOK Bremen"
        };
        var doctor = new User
        {
            FirstName = "Dr. Thomas", LastName = "Meier",
            Email = $"arztbrief-{Guid.NewGuid():N}@test.de",
            PasswordHash = "x", Role = UserRole.Arzt, IsActive = true
        };

        _factory.Context.Set<Patient>().Add(patient);
        _factory.Context.Set<User>().Add(doctor);
        _factory.Context.SaveChanges();

        _patientId = patient.Id;
        _doctorId = doctor.Id;
    }

    public void Dispose() => _factory.Dispose();

    private CreateArztbriefDto MakeDto(string recipient = "Dr. Weber", string body = "Befundbericht") => new()
    {
        PatientId = _patientId,
        DoctorId = _doctorId,
        RecipientName = recipient,
        RecipientAddress = "Musterstr. 1, 28199 Bremen",
        Subject = "Arztbrief — Brief Patient",
        Body = body,
        Diagnoses = ["F32.1", "G43.0"]
    };

    [Fact]
    public async Task SaveDraft_CreatesDocument_ReturnsId()
    {
        var id = await _sut.SaveDraftAsync(MakeDto());

        Assert.NotEqual(Guid.Empty, id);

        var detail = await _sut.GetByIdAsync(id);
        Assert.NotNull(detail);
        Assert.Equal(_patientId, detail.PatientId);
        Assert.Equal(_doctorId, detail.DoctorId);
        Assert.Equal("Dr. Weber", detail.RecipientName);
        Assert.Equal("Befundbericht", detail.Body);
        Assert.Equal(ArztbriefStatus.Entwurf, detail.Status);
        Assert.False(detail.HasPdf);
        Assert.Equal(2, detail.Diagnoses.Count);
        Assert.Contains("F32.1", detail.Diagnoses);
        Assert.Contains("G43.0", detail.Diagnoses);
    }

    [Fact]
    public async Task Update_ModifiesContent()
    {
        var id = await _sut.SaveDraftAsync(MakeDto());

        var updatedDto = MakeDto(recipient: "Dr. Schmidt", body: "Verlaufsbericht aktualisiert");
        updatedDto.Diagnoses = ["F33.0"];

        await _sut.UpdateAsync(id, updatedDto);

        var detail = await _sut.GetByIdAsync(id);
        Assert.NotNull(detail);
        Assert.Equal("Dr. Schmidt", detail.RecipientName);
        Assert.Equal("Verlaufsbericht aktualisiert", detail.Body);
        Assert.Single(detail.Diagnoses);
        Assert.Contains("F33.0", detail.Diagnoses);
    }

    [Fact]
    public async Task GeneratePdf_CreatesPdfData()
    {
        var id = await _sut.SaveDraftAsync(MakeDto(body: "Sehr geehrte Kollegen, ich berichte über den Patienten."));

        await _sut.GeneratePdfAsync(id);

        var pdf = await _sut.GetPdfAsync(id);
        Assert.NotNull(pdf);
        Assert.True(pdf.Length > 100, "PDF should have meaningful content");
        // PDF files start with %PDF
        Assert.Equal(0x25, pdf[0]); // '%'
        Assert.Equal(0x50, pdf[1]); // 'P'
        Assert.Equal(0x44, pdf[2]); // 'D'
        Assert.Equal(0x46, pdf[3]); // 'F'

        var detail = await _sut.GetByIdAsync(id);
        Assert.NotNull(detail);
        Assert.True(detail.HasPdf);
    }

    [Fact]
    public async Task UpdateStatus_Freigeben_ThenVersendet()
    {
        var id = await _sut.SaveDraftAsync(MakeDto());
        await _sut.GeneratePdfAsync(id);

        // Freigeben
        await _sut.UpdateStatusAsync(id, ArztbriefStatus.Freigegeben);
        var detail = await _sut.GetByIdAsync(id);
        Assert.Equal(ArztbriefStatus.Freigegeben, detail!.Status);

        // Versendet
        await _sut.UpdateStatusAsync(id, ArztbriefStatus.Versendet);
        detail = await _sut.GetByIdAsync(id);
        Assert.Equal(ArztbriefStatus.Versendet, detail!.Status);
    }

    [Fact]
    public async Task Update_FreigegebenDocument_ResetsToEntwurf()
    {
        var id = await _sut.SaveDraftAsync(MakeDto());
        await _sut.GeneratePdfAsync(id);
        await _sut.UpdateStatusAsync(id, ArztbriefStatus.Freigegeben);

        // Editing a Freigegeben document should reset to Entwurf and invalidate PDF
        await _sut.UpdateAsync(id, MakeDto(body: "Geänderter Text"));

        var detail = await _sut.GetByIdAsync(id);
        Assert.Equal(ArztbriefStatus.Entwurf, detail!.Status);
        Assert.False(detail.HasPdf); // PDF invalidated
    }

    [Fact]
    public async Task Update_VersendeterDocument_ThrowsException()
    {
        var id = await _sut.SaveDraftAsync(MakeDto());
        await _sut.UpdateStatusAsync(id, ArztbriefStatus.Versendet);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.UpdateAsync(id, MakeDto(body: "Should fail")));
    }

    [Fact]
    public async Task GetByPatient_ReturnsAllArztbriefe()
    {
        await _sut.SaveDraftAsync(MakeDto(recipient: "Dr. A", body: "Brief 1"));
        await _sut.SaveDraftAsync(MakeDto(recipient: "Dr. B", body: "Brief 2"));
        await _sut.SaveDraftAsync(MakeDto(recipient: "Dr. C", body: "Brief 3"));

        var list = await _sut.GetByPatientAsync(_patientId);

        Assert.Equal(3, list.Count);
        // Ordered by CreatedAt descending
        Assert.Equal("Dr. C", list[0].RecipientName);
    }

    [Fact]
    public async Task CreateAndGeneratePdf_CombinesSteps()
    {
        var id = await _sut.CreateAndGeneratePdfAsync(MakeDto(body: "Kombinierter Test"));

        var detail = await _sut.GetByIdAsync(id);
        Assert.NotNull(detail);
        Assert.Equal("Kombinierter Test", detail.Body);
        Assert.True(detail.HasPdf);

        var pdf = await _sut.GetPdfAsync(id);
        Assert.NotNull(pdf);
        Assert.True(pdf.Length > 100);
    }

    [Fact]
    public async Task GetById_NonExistent_ReturnsNull()
    {
        var result = await _sut.GetByIdAsync(Guid.NewGuid());
        Assert.Null(result);
    }

    [Fact]
    public async Task GetPdf_NonExistent_ReturnsNull()
    {
        var result = await _sut.GetPdfAsync(Guid.NewGuid());
        Assert.Null(result);
    }
}
