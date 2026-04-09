using MediPrax.Application.DTOs;
using MediPrax.Application.Services;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;

namespace MediPrax.IntegrationTests;

public class AvailabilityServiceTests : IDisposable
{
    private readonly TestDbContextFactory _factory;
    private readonly AvailabilityService _sut;
    private readonly User _doctor;

    public AvailabilityServiceTests()
    {
        _factory = new TestDbContextFactory();
        _sut = new AvailabilityService(_factory.Context);

        _doctor = new User
        {
            FirstName = "Dr. Test", LastName = "Arzt",
            Email = "test@test.de", PasswordHash = "hash",
            Role = UserRole.Arzt, IsActive = true
        };
        _factory.Context.Users.Add(_doctor);
        _factory.Context.SaveChanges();
    }

    [Fact]
    public async Task CreateScheduleBlock_And_GetSchedule()
    {
        await _sut.CreateScheduleBlockAsync(new CreateScheduleBlockDto
        {
            DoctorId = _doctor.Id,
            DayOfWeek = DayOfWeek.Monday,
            StartTime = new TimeOnly(8, 0),
            EndTime = new TimeOnly(12, 30),
            SlotDurationMinutes = 25
        });

        var schedule = await _sut.GetScheduleAsync(_doctor.Id);
        Assert.Single(schedule);
        Assert.Equal(DayOfWeek.Monday, schedule[0].DayOfWeek);
        Assert.Equal(new TimeOnly(8, 0), schedule[0].StartTime);
    }

    [Fact]
    public async Task CreateAbsence_And_GetAbsences()
    {
        await _sut.CreateAbsenceAsync(new CreateAbsenceDto
        {
            DoctorId = _doctor.Id,
            StartDate = new DateOnly(2026, 7, 28),
            EndDate = new DateOnly(2026, 8, 15),
            AbsenceType = AbsenceType.Urlaub,
            Reason = "Sommerurlaub"
        });

        var absences = await _sut.GetAbsencesAsync(_doctor.Id, new DateOnly(2026, 7, 1), new DateOnly(2026, 8, 31));
        Assert.Single(absences);
        Assert.Equal(AbsenceType.Urlaub, absences[0].AbsenceType);
        Assert.Equal("Sommerurlaub", absences[0].Reason);
    }

    [Fact]
    public async Task GetAbsences_FiltersOutOfRange()
    {
        await _sut.CreateAbsenceAsync(new CreateAbsenceDto
        {
            DoctorId = _doctor.Id,
            StartDate = new DateOnly(2026, 12, 20),
            EndDate = new DateOnly(2027, 1, 3),
            AbsenceType = AbsenceType.Urlaub
        });

        var absences = await _sut.GetAbsencesAsync(_doctor.Id, new DateOnly(2026, 1, 1), new DateOnly(2026, 6, 30));
        Assert.Empty(absences);
    }

    [Fact]
    public async Task CheckAvailability_NoSchedule_ReturnsAvailable()
    {
        // No schedule = no conflict (warning-free)
        var result = await _sut.CheckAvailabilityAsync(_doctor.Id,
            new DateTime(2026, 4, 14, 6, 0, 0, DateTimeKind.Utc), 25);
        Assert.True(result.IsAvailable);
    }

    [Fact]
    public async Task DeleteScheduleBlock_RemovesBlock()
    {
        var block = await _sut.CreateScheduleBlockAsync(new CreateScheduleBlockDto
        {
            DoctorId = _doctor.Id,
            DayOfWeek = DayOfWeek.Friday,
            StartTime = new TimeOnly(8, 0),
            EndTime = new TimeOnly(12, 0)
        });

        await _sut.DeleteScheduleBlockAsync(block.Id);
        var schedule = await _sut.GetScheduleAsync(_doctor.Id);
        Assert.Empty(schedule);
    }

    [Fact]
    public async Task DeleteAbsence_RemovesAbsence()
    {
        var absence = await _sut.CreateAbsenceAsync(new CreateAbsenceDto
        {
            DoctorId = _doctor.Id,
            StartDate = new DateOnly(2026, 5, 1),
            EndDate = new DateOnly(2026, 5, 1),
            AbsenceType = AbsenceType.Fortbildung
        });

        await _sut.DeleteAbsenceAsync(absence.Id);
        var absences = await _sut.GetAbsencesAsync(_doctor.Id, new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 1));
        Assert.Empty(absences);
    }

    public void Dispose() => _factory.Dispose();
}
