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
}

public class DashboardActivityDto
{
    public string Type { get; set; } = string.Empty; // "encounter", "arztbrief", "patient"
    public string Description { get; set; } = string.Empty;
    public Guid? PatientId { get; set; }
    public DateTime Timestamp { get; set; }
}
