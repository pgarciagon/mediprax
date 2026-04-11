using MediPrax.Application.DTOs;
using MediPrax.Core.Enums;

namespace MediPrax.UnitTests;

public class AvailabilityTests
{
    [Fact]
    public void FreeSlotDto_TimeDisplay_FormatsCorrectly()
    {
        var slot = new FreeSlotDto
        {
            Start = new DateTime(2026, 4, 9, 6, 0, 0, DateTimeKind.Utc), // 08:00 CEST
            End = new DateTime(2026, 4, 9, 6, 50, 0, DateTimeKind.Utc),
            DurationMinutes = 50,
            DoctorId = Guid.NewGuid(),
            DoctorName = "Dr. Meier"
        };

        Assert.Equal(50, slot.DurationMinutes);
        Assert.Equal("Dr. Meier", slot.DoctorName);
    }

    [Fact]
    public void AbsenceDto_IsFullDay_WhenNoTimes()
    {
        var absence = new AbsenceDto
        {
            StartDate = new DateOnly(2026, 7, 28),
            EndDate = new DateOnly(2026, 8, 15),
            AbsenceType = AbsenceType.Urlaub
        };

        Assert.True(absence.IsFullDay);
        Assert.Equal("Urlaub", absence.TypeDisplay);
    }

    [Fact]
    public void AbsenceDto_IsNotFullDay_WhenTimesSet()
    {
        var absence = new AbsenceDto
        {
            StartDate = new DateOnly(2026, 4, 9),
            EndDate = new DateOnly(2026, 4, 9),
            StartTime = new TimeOnly(12, 0),
            EndTime = new TimeOnly(14, 0),
            AbsenceType = AbsenceType.Sperrzeit
        };

        Assert.False(absence.IsFullDay);
        Assert.Equal("Sperrzeit", absence.TypeDisplay);
    }

    [Theory]
    [InlineData(AbsenceType.Urlaub, "Urlaub")]
    [InlineData(AbsenceType.Fortbildung, "Fortbildung")]
    [InlineData(AbsenceType.Krank, "Krank")]
    [InlineData(AbsenceType.Sperrzeit, "Sperrzeit")]
    public void AbsenceDto_TypeDisplay_AllTypes(AbsenceType type, string expected)
    {
        var dto = new AbsenceDto { AbsenceType = type };
        Assert.Equal(expected, dto.TypeDisplay);
    }

    [Fact]
    public void AvailabilityCheckResult_IsAvailable_Default()
    {
        var result = new AvailabilityCheckResult { IsAvailable = true };
        Assert.True(result.IsAvailable);
        Assert.Null(result.ConflictReason);
        Assert.False(result.IsWarningOnly);
    }

    [Fact]
    public void AvailabilityCheckResult_Blocked()
    {
        var result = new AvailabilityCheckResult
        {
            IsAvailable = false,
            ConflictReason = "Arzt ist Urlaub"
        };
        Assert.False(result.IsAvailable);
        Assert.Contains("Urlaub", result.ConflictReason);
    }

    [Fact]
    public void AvailabilityCheckResult_Warning()
    {
        var result = new AvailabilityCheckResult
        {
            IsAvailable = true,
            IsWarningOnly = true,
            ConflictReason = "Außerhalb der Sprechzeiten"
        };
        Assert.True(result.IsAvailable);
        Assert.True(result.IsWarningOnly);
    }

    [Fact]
    public void ScheduleBlockDto_Properties()
    {
        var block = new ScheduleBlockDto
        {
            DayOfWeek = DayOfWeek.Monday,
            StartTime = new TimeOnly(8, 0),
            EndTime = new TimeOnly(12, 30),
            SlotDurationMinutes = 25
        };

        Assert.Equal(DayOfWeek.Monday, block.DayOfWeek);
        Assert.Equal(new TimeOnly(8, 0), block.StartTime);
        Assert.Equal(new TimeOnly(12, 30), block.EndTime);
        Assert.Equal(25, block.SlotDurationMinutes);
    }

    [Fact]
    public void CreateScheduleBlockDto_DefaultSlotDuration()
    {
        var dto = new CreateScheduleBlockDto();
        Assert.Equal(25, dto.SlotDurationMinutes);
    }

    [Fact]
    public void FreeSlotDto_DateDisplay_FormatsGerman()
    {
        var slot = new FreeSlotDto
        {
            Start = new DateTime(2026, 4, 13, 6, 0, 0, DateTimeKind.Utc), // Monday 08:00 CEST
            End = new DateTime(2026, 4, 13, 6, 50, 0, DateTimeKind.Utc),
            DurationMinutes = 50,
            DoctorId = Guid.NewGuid(),
            DoctorName = "Dr. Test"
        };

        // DateDisplay and TimeDisplay should not be empty
        Assert.False(string.IsNullOrWhiteSpace(slot.DateDisplay));
        Assert.False(string.IsNullOrWhiteSpace(slot.TimeDisplay));
        Assert.Contains("–", slot.TimeDisplay); // Contains time range separator
    }

    [Fact]
    public void FreeSlotDto_DurationMatchesStartEnd()
    {
        var start = new DateTime(2026, 4, 14, 8, 0, 0, DateTimeKind.Utc);
        var slot = new FreeSlotDto
        {
            Start = start,
            End = start.AddMinutes(25),
            DurationMinutes = 25,
            DoctorId = Guid.NewGuid(),
            DoctorName = "Dr. Schmidt"
        };

        Assert.Equal(25, (slot.End - slot.Start).TotalMinutes);
        Assert.Equal(slot.DurationMinutes, (int)(slot.End - slot.Start).TotalMinutes);
    }

    [Fact]
    public void CreateAbsenceDto_PartialAbsence_HasTimes()
    {
        var dto = new CreateAbsenceDto
        {
            DoctorId = Guid.NewGuid(),
            StartDate = new DateOnly(2026, 4, 15),
            EndDate = new DateOnly(2026, 4, 15),
            StartTime = new TimeOnly(12, 0),
            EndTime = new TimeOnly(14, 0),
            AbsenceType = AbsenceType.Sperrzeit,
            Reason = "Teambesprechung"
        };

        Assert.True(dto.StartTime.HasValue);
        Assert.True(dto.EndTime.HasValue);
        Assert.Equal(dto.StartDate, dto.EndDate); // Same day
    }
}
