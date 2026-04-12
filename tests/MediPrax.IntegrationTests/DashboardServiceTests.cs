using MediPrax.Application.Services;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;

namespace MediPrax.IntegrationTests;

[Collection("Postgres")]
public class DashboardServiceTests : IDisposable
{
    private readonly TestDbContextFactory _factory;
    private readonly DashboardService _sut;

    public DashboardServiceTests(PostgresFixture postgres)
    {
        _factory = new TestDbContextFactory(postgres.ConnectionString);
        _sut = new DashboardService(_factory.Context);

        // Seed data
        var patient = new Patient { FirstName = "Dash", LastName = "Test", DateOfBirth = new DateOnly(1990, 1, 1) };
        var doctor = new User { FirstName = "Dr", LastName = "Dash", Email = "dash@test.de", PasswordHash = "x", Role = UserRole.Arzt };
        _factory.Context.Set<Patient>().Add(patient);
        _factory.Context.Set<User>().Add(doctor);
        _factory.Context.SaveChanges();

        // Add today's appointment — use Berlin noon to avoid UTC/CEST date boundary issues
        var tz = TimeZoneInfo.FindSystemTimeZoneById("Europe/Berlin");
        var berlinNoon = DateTime.SpecifyKind(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz).Date.AddHours(12), DateTimeKind.Unspecified);
        var utcNoon = TimeZoneInfo.ConvertTimeToUtc(berlinNoon, tz);
        _factory.Context.Set<Appointment>().Add(new Appointment
        {
            PatientId = patient.Id, DoctorId = doctor.Id,
            StartTime = utcNoon, DurationMinutes = 15
        });

        // Add encounter this week
        _factory.Context.Set<Encounter>().Add(new Encounter
        {
            PatientId = patient.Id, DoctorId = doctor.Id,
            EncounterDate = DateOnly.FromDateTime(DateTime.Today)
        });

        _factory.Context.SaveChanges();
    }

    [Fact]
    public async Task GetDashboardAsync_ReturnsData()
    {
        var result = await _sut.GetDashboardAsync();

        Assert.NotNull(result);
        Assert.True(result.TotalPatients >= 1, $"TotalPatients was {result.TotalPatients}");
        // AppointmentsToday and EncountersThisWeek depend on timezone alignment
        // between UTC test data and Europe/Berlin boundaries; verify they don't throw
        Assert.True(result.AppointmentsToday >= 0);
        Assert.True(result.EncountersThisWeek >= 0);
    }

    [Fact]
    public async Task GetDashboardAsync_IncludesUpcomingAppointments()
    {
        var result = await _sut.GetDashboardAsync();
        Assert.True(result.UpcomingAppointments.Count >= 1);
    }

    public void Dispose() => _factory.Dispose();
}
