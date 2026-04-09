using MediPrax.Application.DTOs;
using MediPrax.Application.Services;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;

namespace MediPrax.IntegrationTests;

public class SlotSuggestionServiceTests : IDisposable
{
    private readonly TestDbContextFactory _factory;
    private readonly AvailabilityService _availability;
    private readonly SlotSuggestionService _sut;
    private readonly User _doctor;
    private readonly Patient _patient;
    private static readonly TimeZoneInfo Tz = TimeZoneInfo.FindSystemTimeZoneById("Europe/Berlin");

    public SlotSuggestionServiceTests()
    {
        _factory = new TestDbContextFactory();
        _availability = new AvailabilityService(_factory.Context);
        _sut = new SlotSuggestionService(_availability, _factory.Context);

        _doctor = new User
        {
            FirstName = "Dr. Test", LastName = "Therapeut",
            Email = "therapeut@test.de", PasswordHash = "hash",
            Role = UserRole.Arzt, IsActive = true
        };
        _patient = new Patient
        {
            FirstName = "Anna", LastName = "Test",
            DateOfBirth = new DateOnly(1985, 5, 15)
        };
        _factory.Context.Users.Add(_doctor);
        _factory.Context.Patients.Add(_patient);
        _factory.Context.SaveChanges();

        // Setup: Mo+Mi+Fr schedule
        foreach (var day in new[] { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday })
        {
            _factory.Context.Set<DoctorScheduleTemplate>().Add(new DoctorScheduleTemplate
            {
                DoctorId = _doctor.Id, DayOfWeek = day,
                StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(12, 0),
                SlotDurationMinutes = 50
            });
        }
        _factory.Context.SaveChanges();
    }

    [Fact]
    public async Task SuggestNextSlot_ReturnsSlots()
    {
        var slots = await _sut.SuggestNextSlotAsync(_doctor.Id, 50);
        Assert.True(slots.Count >= 1);
        Assert.True(slots.Count <= 5);
        Assert.All(slots, s => Assert.Equal(_doctor.Id, s.DoctorId));
    }

    [Fact]
    public async Task SuggestNextSlot_RespectsDuration()
    {
        var slots = await _sut.SuggestNextSlotAsync(_doctor.Id, 50);
        Assert.All(slots, s => Assert.Equal(50, s.DurationMinutes));
    }

    [Fact]
    public async Task SuggestNextTherapySession_WithExistingCase()
    {
        var therapyCase = new TherapyCase
        {
            PatientId = _patient.Id, TherapistId = _doctor.Id,
            TherapyType = TherapyType.LangzeittherapieVT,
            Status = TherapyCaseStatus.InBehandlung,
            StartDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(-3)),
            ApprovedSessions = 24, CompletedSessions = 8,
            SessionDurationMinutes = 50
        };
        _factory.Context.Set<TherapyCase>().Add(therapyCase);
        _factory.Context.SaveChanges();

        var slots = await _sut.SuggestNextTherapySessionAsync(therapyCase.Id);
        Assert.True(slots.Count >= 1);
        Assert.All(slots, s => Assert.Equal(50, s.DurationMinutes));
    }

    [Fact]
    public async Task SuggestNextTherapySession_PrefersLastSessionDayTime()
    {
        // Create therapy case
        var therapyCase = new TherapyCase
        {
            PatientId = _patient.Id, TherapistId = _doctor.Id,
            TherapyType = TherapyType.LangzeittherapieVT,
            Status = TherapyCaseStatus.InBehandlung,
            StartDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(-2)),
            ApprovedSessions = 24, CompletedSessions = 5,
            SessionDurationMinutes = 50
        };
        _factory.Context.Set<TherapyCase>().Add(therapyCase);

        // Create a past appointment on Wednesday at 10:00
        var lastWed = DateOnly.FromDateTime(DateTime.Today);
        while (lastWed.DayOfWeek != DayOfWeek.Wednesday) lastWed = lastWed.AddDays(-1);
        var utcStart = TimeZoneInfo.ConvertTimeToUtc(
            new DateTime(lastWed.Year, lastWed.Month, lastWed.Day, 10, 0, 0, DateTimeKind.Unspecified), Tz);

        _factory.Context.Appointments.Add(new Appointment
        {
            PatientId = _patient.Id, DoctorId = _doctor.Id,
            StartTime = utcStart, DurationMinutes = 50,
            Status = AppointmentStatus.Completed
        });
        _factory.Context.SaveChanges();

        var slots = await _sut.SuggestNextTherapySessionAsync(therapyCase.Id);
        Assert.True(slots.Count >= 1);
        // At least one suggestion should be on Wednesday (preferred day)
        var hasWednesday = slots.Any(s => s.Start.ToLocalTime().DayOfWeek == DayOfWeek.Wednesday);
        Assert.True(hasWednesday, "Should suggest at least one slot on Wednesday (last session day)");
    }

    [Fact]
    public async Task SuggestNextTherapySession_InvalidCase_ReturnsEmpty()
    {
        var slots = await _sut.SuggestNextTherapySessionAsync(Guid.NewGuid());
        Assert.Empty(slots);
    }

    [Fact]
    public async Task SuggestNextSlot_SkipsUrlaubDays()
    {
        // Block next 2 weeks of Mondays
        var nextMonday = DateOnly.FromDateTime(DateTime.Today);
        while (nextMonday.DayOfWeek != DayOfWeek.Monday) nextMonday = nextMonday.AddDays(1);

        _factory.Context.Set<DoctorAbsence>().Add(new DoctorAbsence
        {
            DoctorId = _doctor.Id,
            StartDate = nextMonday, EndDate = nextMonday.AddDays(13),
            AbsenceType = AbsenceType.Urlaub
        });
        _factory.Context.SaveChanges();

        var slots = await _sut.SuggestNextSlotAsync(_doctor.Id, 50);
        // No slot should be during the Urlaub period
        Assert.All(slots, s =>
        {
            var date = DateOnly.FromDateTime(s.Start.ToLocalTime());
            var duringUrlaub = date >= nextMonday && date <= nextMonday.AddDays(13);
            Assert.False(duringUrlaub, $"Slot on {date} falls during Urlaub {nextMonday}-{nextMonday.AddDays(13)}");
        });
    }

    public void Dispose() => _factory.Dispose();
}
