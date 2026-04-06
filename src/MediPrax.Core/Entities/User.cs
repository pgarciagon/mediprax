using MediPrax.Core.Enums;

namespace MediPrax.Core.Entities;

public class User : BaseEntity
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;

    public string FullName => $"{FirstName} {LastName}";

    public ICollection<Appointment> Appointments { get; set; } = [];
    public ICollection<Encounter> Encounters { get; set; } = [];
}
