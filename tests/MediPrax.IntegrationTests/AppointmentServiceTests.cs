using MediPrax.Application.DTOs;
using MediPrax.Application.Services;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;

namespace MediPrax.IntegrationTests;

[Collection("Postgres")]
public class AppointmentServiceTests : IDisposable
{
    private readonly TestDbContextFactory _factory;
    private readonly AppointmentService _sut;
    private readonly Guid _patientId;
    private readonly Guid _doctorId;

    public AppointmentServiceTests(PostgresFixture postgres)
    {
        _factory = new TestDbContextFactory(postgres.ConnectionString);
        _sut = new AppointmentService(_factory.Context);

        // Seed patient and doctor
        var patient = new Patient { FirstName = "Test", LastName = "Patient", DateOfBirth = new DateOnly(1990, 1, 1) };
        var doctor = new User { FirstName = "Dr", LastName = "Test", Email = "dr@test.de", PasswordHash = "x", Role = UserRole.Arzt };
        _factory.Context.Set<Patient>().Add(patient);
        _factory.Context.Set<User>().Add(doctor);
        _factory.Context.SaveChanges();
        _patientId = patient.Id;
        _doctorId = doctor.Id;
    }

    [Fact]
    public async Task CreateAsync_CreatesAppointment()
    {
        var result = await _sut.CreateAsync(new CreateAppointmentDto
        {
            PatientId = _patientId,
            DoctorId = _doctorId,
            StartTime = DateTime.UtcNow.AddDays(1),
            DurationMinutes = 15
        });

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(15, result.DurationMinutes);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsAppointment()
    {
        var created = await _sut.CreateAsync(new CreateAppointmentDto
        {
            PatientId = _patientId, DoctorId = _doctorId,
            StartTime = DateTime.UtcNow.AddDays(2), DurationMinutes = 10
        });

        var found = await _sut.GetByIdAsync(created.Id);
        Assert.NotNull(found);
        Assert.Equal(created.Id, found.Id);
    }

    [Fact]
    public async Task UpdateStatusAsync_ChangesStatus()
    {
        var created = await _sut.CreateAsync(new CreateAppointmentDto
        {
            PatientId = _patientId, DoctorId = _doctorId,
            StartTime = DateTime.UtcNow, DurationMinutes = 10
        });

        await _sut.UpdateStatusAsync(created.Id, AppointmentStatus.CheckedIn);
        var updated = await _sut.GetByIdAsync(created.Id);
        Assert.Equal(AppointmentStatus.CheckedIn, updated!.Status);
    }

    [Fact]
    public async Task DeleteAsync_SoftDeletesAppointment()
    {
        var created = await _sut.CreateAsync(new CreateAppointmentDto
        {
            PatientId = _patientId, DoctorId = _doctorId,
            StartTime = DateTime.UtcNow.AddDays(3), DurationMinutes = 10
        });

        await _sut.DeleteAsync(created.Id);

        // GetById uses FindAsync which bypasses filters, so check via week schedule
        var schedule = await _sut.GetWeekScheduleAsync(
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)), null);
        var dayAppts = schedule.Days.SelectMany(d => d.Appointments);
        Assert.DoesNotContain(dayAppts, a => a.Id == created.Id);
    }

    public void Dispose() => _factory.Dispose();
}
