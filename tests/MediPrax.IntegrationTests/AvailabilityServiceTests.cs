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
    private readonly Patient _patient;
    private static readonly TimeZoneInfo Tz = TimeZoneInfo.FindSystemTimeZoneById("Europe/Berlin");

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
        _patient = new Patient
        {
            FirstName = "Max", LastName = "Muster",
            DateOfBirth = new DateOnly(1990, 1, 1)
        };
        _factory.Context.Users.Add(_doctor);
        _factory.Context.Patients.Add(_patient);
        _factory.Context.SaveChanges();
    }

    // Helper: convert local Berlin time to UTC
    private static DateTime ToUtc(int year, int month, int day, int hour, int minute)
    {
        var local = new DateTime(year, month, day, hour, minute, 0, DateTimeKind.Unspecified);
        return TimeZoneInfo.ConvertTimeToUtc(local, Tz);
    }

    // Helper: find the next occurrence of a given DayOfWeek from today
    private static DateOnly NextDay(DayOfWeek day)
    {
        var date = DateOnly.FromDateTime(DateTime.Today);
        while (date.DayOfWeek != day) date = date.AddDays(1);
        return date;
    }

    // --- CRUD Tests ---

    [Fact]
    public async Task CreateScheduleBlock_And_GetSchedule()
    {
        await _sut.CreateScheduleBlockAsync(new CreateScheduleBlockDto
        {
            DoctorId = _doctor.Id, DayOfWeek = DayOfWeek.Monday,
            StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(12, 30), SlotDurationMinutes = 25
        });

        var schedule = await _sut.GetScheduleAsync(_doctor.Id);
        Assert.Single(schedule);
        Assert.Equal(DayOfWeek.Monday, schedule[0].DayOfWeek);
    }

    [Fact]
    public async Task CreateAbsence_And_GetAbsences()
    {
        await _sut.CreateAbsenceAsync(new CreateAbsenceDto
        {
            DoctorId = _doctor.Id, StartDate = new DateOnly(2026, 7, 28),
            EndDate = new DateOnly(2026, 8, 15), AbsenceType = AbsenceType.Urlaub, Reason = "Sommerurlaub"
        });

        var absences = await _sut.GetAbsencesAsync(_doctor.Id, new DateOnly(2026, 7, 1), new DateOnly(2026, 8, 31));
        Assert.Single(absences);
        Assert.Equal("Sommerurlaub", absences[0].Reason);
    }

    [Fact]
    public async Task GetAbsences_FiltersOutOfRange()
    {
        await _sut.CreateAbsenceAsync(new CreateAbsenceDto
        {
            DoctorId = _doctor.Id, StartDate = new DateOnly(2026, 12, 20),
            EndDate = new DateOnly(2027, 1, 3), AbsenceType = AbsenceType.Urlaub
        });

        var absences = await _sut.GetAbsencesAsync(_doctor.Id, new DateOnly(2026, 1, 1), new DateOnly(2026, 6, 30));
        Assert.Empty(absences);
    }

    [Fact]
    public async Task DeleteScheduleBlock_RemovesBlock()
    {
        var block = await _sut.CreateScheduleBlockAsync(new CreateScheduleBlockDto
        {
            DoctorId = _doctor.Id, DayOfWeek = DayOfWeek.Friday,
            StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(12, 0)
        });

        await _sut.DeleteScheduleBlockAsync(block.Id);
        Assert.Empty(await _sut.GetScheduleAsync(_doctor.Id));
    }

    [Fact]
    public async Task DeleteAbsence_RemovesAbsence()
    {
        var absence = await _sut.CreateAbsenceAsync(new CreateAbsenceDto
        {
            DoctorId = _doctor.Id, StartDate = new DateOnly(2026, 5, 1),
            EndDate = new DateOnly(2026, 5, 1), AbsenceType = AbsenceType.Fortbildung
        });

        await _sut.DeleteAbsenceAsync(absence.Id);
        Assert.Empty(await _sut.GetAbsencesAsync(_doctor.Id, new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 1)));
    }

    // --- Free Slot Calculation Tests ---

    [Fact]
    public async Task GetFreeSlots_ReturnsSlots_WhenScheduleExists()
    {
        var monday = NextDay(DayOfWeek.Monday);
        await _sut.CreateScheduleBlockAsync(new CreateScheduleBlockDto
        {
            DoctorId = _doctor.Id, DayOfWeek = DayOfWeek.Monday,
            StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(10, 0), SlotDurationMinutes = 25
        });

        var slots = await _sut.GetFreeSlotsAsync(_doctor.Id, monday, 25);
        // 08:00-10:00 = 120 min / 25 min slots ~ 4 slots (08:00, 08:25, 08:50, 09:15, 09:35 won't fit as 09:35+25=10:00)
        Assert.True(slots.Count >= 4);
    }

    [Fact]
    public async Task GetFreeSlots_ReturnsEmpty_WhenNoSchedule()
    {
        var tuesday = NextDay(DayOfWeek.Tuesday);
        // No schedule for Tuesday
        var slots = await _sut.GetFreeSlotsAsync(_doctor.Id, tuesday, 25);
        Assert.Empty(slots);
    }

    [Fact]
    public async Task GetFreeSlots_ReturnsEmpty_WhenFullDayAbsence()
    {
        var monday = NextDay(DayOfWeek.Monday);
        await _sut.CreateScheduleBlockAsync(new CreateScheduleBlockDto
        {
            DoctorId = _doctor.Id, DayOfWeek = DayOfWeek.Monday,
            StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(12, 0), SlotDurationMinutes = 25
        });
        await _sut.CreateAbsenceAsync(new CreateAbsenceDto
        {
            DoctorId = _doctor.Id, StartDate = monday, EndDate = monday,
            AbsenceType = AbsenceType.Urlaub
        });

        var slots = await _sut.GetFreeSlotsAsync(_doctor.Id, monday, 25);
        Assert.Empty(slots);
    }

    [Fact]
    public async Task GetFreeSlots_ExcludesOccupiedSlots()
    {
        var monday = NextDay(DayOfWeek.Monday);
        await _sut.CreateScheduleBlockAsync(new CreateScheduleBlockDto
        {
            DoctorId = _doctor.Id, DayOfWeek = DayOfWeek.Monday,
            StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(9, 0), SlotDurationMinutes = 25
        });

        // Add appointment at 08:00-08:25
        var utcStart = TimeZoneInfo.ConvertTimeToUtc(
            new DateTime(monday.Year, monday.Month, monday.Day, 8, 0, 0, DateTimeKind.Unspecified), Tz);
        _factory.Context.Appointments.Add(new Appointment
        {
            PatientId = _patient.Id, DoctorId = _doctor.Id,
            StartTime = utcStart, DurationMinutes = 25
        });
        _factory.Context.SaveChanges();

        var slots = await _sut.GetFreeSlotsAsync(_doctor.Id, monday, 25);
        // 08:00 is occupied, so the 08:00 slot should not appear
        var slotTimes = slots.Select(s => TimeOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(s.Start, Tz))).ToList();
        Assert.DoesNotContain(new TimeOnly(8, 0), slotTimes);
        // But 08:25 should be free
        Assert.Contains(new TimeOnly(8, 25), slotTimes);
    }

    [Fact]
    public async Task GetFreeSlots_ExcludesSperrzeit()
    {
        var wednesday = NextDay(DayOfWeek.Wednesday);
        await _sut.CreateScheduleBlockAsync(new CreateScheduleBlockDto
        {
            DoctorId = _doctor.Id, DayOfWeek = DayOfWeek.Wednesday,
            StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(12, 0), SlotDurationMinutes = 30
        });
        // Sperrzeit 10:00-11:00
        await _sut.CreateAbsenceAsync(new CreateAbsenceDto
        {
            DoctorId = _doctor.Id, StartDate = wednesday, EndDate = wednesday,
            StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(11, 0),
            AbsenceType = AbsenceType.Sperrzeit, Reason = "Teambesprechung"
        });

        var slots = await _sut.GetFreeSlotsAsync(_doctor.Id, wednesday, 30);
        // No slot should overlap 10:00-11:00
        Assert.All(slots, s =>
        {
            var localStart = TimeOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(s.Start, Tz));
            var localEnd = localStart.AddMinutes(30);
            var blocked = localStart < new TimeOnly(11, 0) && localEnd > new TimeOnly(10, 0);
            Assert.False(blocked, $"Slot {localStart}-{localEnd} overlaps Sperrzeit 10:00-11:00");
        });
    }

    // --- Availability Check Tests ---

    [Fact]
    public async Task CheckAvailability_DuringUrlaub_Blocked()
    {
        var monday = NextDay(DayOfWeek.Monday);
        await _sut.CreateAbsenceAsync(new CreateAbsenceDto
        {
            DoctorId = _doctor.Id, StartDate = monday, EndDate = monday.AddDays(4),
            AbsenceType = AbsenceType.Urlaub
        });

        var utcStart = TimeZoneInfo.ConvertTimeToUtc(
            new DateTime(monday.Year, monday.Month, monday.Day, 9, 0, 0, DateTimeKind.Unspecified), Tz);
        var result = await _sut.CheckAvailabilityAsync(_doctor.Id, utcStart, 25);
        Assert.False(result.IsAvailable);
        Assert.Contains("Urlaub", result.ConflictReason);
    }

    [Fact]
    public async Task CheckAvailability_DuringSperrzeit_Blocked()
    {
        var tuesday = NextDay(DayOfWeek.Tuesday);
        await _sut.CreateAbsenceAsync(new CreateAbsenceDto
        {
            DoctorId = _doctor.Id, StartDate = tuesday, EndDate = tuesday,
            StartTime = new TimeOnly(12, 0), EndTime = new TimeOnly(14, 0),
            AbsenceType = AbsenceType.Sperrzeit, Reason = "Supervision"
        });

        var utcStart = TimeZoneInfo.ConvertTimeToUtc(
            new DateTime(tuesday.Year, tuesday.Month, tuesday.Day, 13, 0, 0, DateTimeKind.Unspecified), Tz);
        var result = await _sut.CheckAvailabilityAsync(_doctor.Id, utcStart, 25);
        Assert.False(result.IsAvailable);
        Assert.Contains("Sperrzeit", result.ConflictReason);
    }

    [Fact]
    public async Task CheckAvailability_OutsideSprechzeiten_Warning()
    {
        var monday = NextDay(DayOfWeek.Monday);
        await _sut.CreateScheduleBlockAsync(new CreateScheduleBlockDto
        {
            DoctorId = _doctor.Id, DayOfWeek = DayOfWeek.Monday,
            StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(12, 0)
        });

        // 18:00 is outside Sprechzeiten
        var utcStart = TimeZoneInfo.ConvertTimeToUtc(
            new DateTime(monday.Year, monday.Month, monday.Day, 18, 0, 0, DateTimeKind.Unspecified), Tz);
        var result = await _sut.CheckAvailabilityAsync(_doctor.Id, utcStart, 25);
        Assert.True(result.IsAvailable);
        Assert.True(result.IsWarningOnly);
        Assert.Contains("Sprechzeiten", result.ConflictReason);
    }

    [Fact]
    public async Task CheckAvailability_WithOverlap_Warning()
    {
        var monday = NextDay(DayOfWeek.Monday);
        var utcStart = TimeZoneInfo.ConvertTimeToUtc(
            new DateTime(monday.Year, monday.Month, monday.Day, 9, 0, 0, DateTimeKind.Unspecified), Tz);

        _factory.Context.Appointments.Add(new Appointment
        {
            PatientId = _patient.Id, DoctorId = _doctor.Id,
            StartTime = utcStart, DurationMinutes = 50
        });
        _factory.Context.SaveChanges();

        // Try to book 09:15 which overlaps with 09:00-09:50
        var utcOverlap = utcStart.AddMinutes(15);
        var result = await _sut.CheckAvailabilityAsync(_doctor.Id, utcOverlap, 25);
        Assert.True(result.IsAvailable); // Warning only, not blocked
        Assert.True(result.IsWarningOnly);
        Assert.Contains("Termin", result.ConflictReason);
    }

    [Fact]
    public async Task CheckAvailability_NoConflict_Available()
    {
        var monday = NextDay(DayOfWeek.Monday);
        await _sut.CreateScheduleBlockAsync(new CreateScheduleBlockDto
        {
            DoctorId = _doctor.Id, DayOfWeek = DayOfWeek.Monday,
            StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(12, 0)
        });

        var utcStart = TimeZoneInfo.ConvertTimeToUtc(
            new DateTime(monday.Year, monday.Month, monday.Day, 10, 0, 0, DateTimeKind.Unspecified), Tz);
        var result = await _sut.CheckAvailabilityAsync(_doctor.Id, utcStart, 25);
        Assert.True(result.IsAvailable);
        Assert.False(result.IsWarningOnly);
        Assert.Null(result.ConflictReason);
    }

    // --- FindNextFreeSlot Tests ---

    [Fact]
    public async Task FindNextFreeSlot_ReturnsResults()
    {
        await _sut.CreateScheduleBlockAsync(new CreateScheduleBlockDto
        {
            DoctorId = _doctor.Id, DayOfWeek = DayOfWeek.Monday,
            StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(12, 0), SlotDurationMinutes = 50
        });
        await _sut.CreateScheduleBlockAsync(new CreateScheduleBlockDto
        {
            DoctorId = _doctor.Id, DayOfWeek = DayOfWeek.Wednesday,
            StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(12, 0), SlotDurationMinutes = 50
        });

        var searchFrom = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var slots = await _sut.FindNextFreeSlotAsync(_doctor.Id, 50, searchFrom, 3);
        Assert.True(slots.Count >= 1);
        Assert.All(slots, s => Assert.Equal(50, s.DurationMinutes));
    }

    [Fact]
    public async Task FindNextFreeSlot_SkipsUrlaubDays()
    {
        var monday = NextDay(DayOfWeek.Monday);
        await _sut.CreateScheduleBlockAsync(new CreateScheduleBlockDto
        {
            DoctorId = _doctor.Id, DayOfWeek = DayOfWeek.Monday,
            StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(12, 0), SlotDurationMinutes = 25
        });
        // Block the next 2 Mondays
        await _sut.CreateAbsenceAsync(new CreateAbsenceDto
        {
            DoctorId = _doctor.Id, StartDate = monday, EndDate = monday.AddDays(7),
            AbsenceType = AbsenceType.Urlaub
        });

        var slots = await _sut.FindNextFreeSlotAsync(_doctor.Id, 25, monday, 3);
        // Should only find slots on Mondays AFTER the Urlaub period
        Assert.All(slots, s =>
        {
            var slotDate = DateOnly.FromDateTime(s.Start.ToLocalTime());
            Assert.True(slotDate > monday.AddDays(7), $"Slot on {slotDate} should be after Urlaub ending {monday.AddDays(7)}");
        });
    }

    public void Dispose() => _factory.Dispose();
}
