using MediPrax.Core.Enums;

namespace MediPrax.Application.DTOs;

public class DashboardDto
{
    public int TotalPatients { get; set; }
    public int AppointmentsToday { get; set; }
    public int EncountersThisWeek { get; set; }
    public int PendingArztbriefe { get; set; }
    public IReadOnlyList<DashboardAppointmentDto> UpcomingAppointments { get; set; } = [];
    public IReadOnlyList<DashboardActivityDto> RecentActivity { get; set; } = [];

    // Arzt-specific
    public int OpenArztbriefe { get; set; }
    public int DueRecalls { get; set; }
    public int OverdueRecalls { get; set; }
    public IReadOnlyList<DashboardWarningDto> Warnings { get; set; } = [];

    // MFA-specific
    public int WaitingRoomCount { get; set; }
    public int InTreatmentCount { get; set; }
    public int WaitlistTotal { get; set; }
    public int WaitlistUrgent { get; set; }
    public double WaitlistAvgDays { get; set; }
    public int PatientsWithMissingData { get; set; }

    // Admin-specific
    public decimal RevenueQuarter { get; set; }
    public int OpenInvoices { get; set; }
    public decimal OpenInvoiceAmount { get; set; }
    public int OverdueInvoices { get; set; }
}

public class DashboardAppointmentDto
{
    public Guid Id { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public Guid PatientId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public int DurationMinutes { get; set; }
    public AppointmentStatus Status { get; set; }
    public string? Notes { get; set; }
}

public class DashboardActivityDto
{
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid? PatientId { get; set; }
    public DateTime Timestamp { get; set; }
}

public class DashboardWarningDto
{
    public string Level { get; set; } = "warning"; // warning, danger, info
    public string Message { get; set; } = string.Empty;
    public Guid? PatientId { get; set; }
    public string? PatientName { get; set; }
}
