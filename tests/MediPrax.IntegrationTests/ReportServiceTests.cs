using MediPrax.Application.Services;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;

namespace MediPrax.IntegrationTests;

public class ReportServiceTests : IDisposable
{
    private readonly TestDbContextFactory _factory;
    private readonly ReportService _sut;

    public ReportServiceTests()
    {
        _factory = new TestDbContextFactory();
        _sut = new ReportService(_factory.Context);

        var patient = new Patient { FirstName = "Report", LastName = "Test", DateOfBirth = new DateOnly(1975, 6, 1) };
        var doctor = new User { FirstName = "Dr", LastName = "Report", Email = "report@test.de", PasswordHash = "x", Role = UserRole.Arzt };
        _factory.Context.Set<Patient>().Add(patient);
        _factory.Context.Set<User>().Add(doctor);
        _factory.Context.SaveChanges();

        _factory.Context.Set<Appointment>().Add(new Appointment
        {
            PatientId = patient.Id, DoctorId = doctor.Id,
            StartTime = DateTime.UtcNow, DurationMinutes = 10,
            Status = AppointmentStatus.Completed
        });

        _factory.Context.Set<Encounter>().Add(new Encounter
        {
            PatientId = patient.Id, DoctorId = doctor.Id,
            EncounterDate = DateOnly.FromDateTime(DateTime.Today),
            Icd10Codes = ["F32.1", "G43.0"]
        });

        _factory.Context.SaveChanges();
    }

    [Fact]
    public async Task GetDailyReportAsync_ReturnsReport()
    {
        var report = await _sut.GetDailyReportAsync(DateOnly.FromDateTime(DateTime.Today));
        Assert.Equal(DateOnly.FromDateTime(DateTime.Today), report.Date);
        Assert.True(report.EncountersCreated >= 1);
    }

    [Fact]
    public async Task GetDailyReportAsync_EmptyForFutureDate()
    {
        var report = await _sut.GetDailyReportAsync(DateOnly.FromDateTime(DateTime.Today.AddYears(10)));
        Assert.Equal(0, report.TotalAppointments);
    }

    [Fact]
    public async Task GetQuarterlyReportAsync_ReturnsReport()
    {
        var now = DateTime.UtcNow;
        var q = (now.Month - 1) / 3 + 1;
        var quarter = $"{now.Year}-Q{q}";

        var report = await _sut.GetQuarterlyReportAsync(quarter);

        Assert.Equal(quarter, report.Quarter);
        Assert.True(report.TotalEncounters >= 1);
        Assert.True(report.TotalPatientsSeen >= 1);
    }

    [Fact]
    public async Task GetQuarterlyReportAsync_IncludesTopDiagnoses()
    {
        var now = DateTime.UtcNow;
        var q = (now.Month - 1) / 3 + 1;
        var quarter = $"{now.Year}-Q{q}";

        var report = await _sut.GetQuarterlyReportAsync(quarter);
        Assert.True(report.TopDiagnoses.Count >= 1);
        Assert.Contains(report.TopDiagnoses, d => d.Code == "F32.1");
    }

    [Fact]
    public async Task GetQuarterlyReportAsync_InvalidQuarter_ReturnsEmpty()
    {
        var report = await _sut.GetQuarterlyReportAsync("invalid");
        Assert.Equal(0, report.TotalEncounters);
    }

    public void Dispose() => _factory.Dispose();
}
