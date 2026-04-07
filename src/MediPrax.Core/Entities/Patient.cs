using MediPrax.Core.Enums;

namespace MediPrax.Core.Entities;

public class Patient : BaseEntity
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required DateOnly DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public InsuranceType? InsuranceType { get; set; }
    public string? InsuranceNumber { get; set; }
    public string? InsuranceProvider { get; set; }
    public string? Kvnr { get; set; } // Krankenversichertennummer
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }

    public string FullName => $"{FirstName} {LastName}";

    public ICollection<Appointment> Appointments { get; set; } = [];
    public ICollection<Encounter> Encounters { get; set; } = [];
    public ICollection<Prescription> Prescriptions { get; set; } = [];
    public ICollection<Document> Documents { get; set; } = [];
    public ICollection<Medication> Medications { get; set; } = [];
    public ICollection<Recall> Recalls { get; set; } = [];
}
