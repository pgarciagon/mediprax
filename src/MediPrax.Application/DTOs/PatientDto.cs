using MediPrax.Core.Enums;

namespace MediPrax.Application.DTOs;

public class PatientDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public InsuranceType? InsuranceType { get; set; }
    public string? InsuranceNumber { get; set; }
    public string? InsuranceProvider { get; set; }
    public string? Kvnr { get; set; }
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }

    public string FullName => $"{FirstName} {LastName}";
    public string InsuranceDisplay => InsuranceType?.ToString() ?? "—";
}

public class PatientListItemDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public InsuranceType? InsuranceType { get; set; }
    public string? Kvnr { get; set; }
    public string? Phone { get; set; }

    public string FullName => $"{LastName}, {FirstName}";
}

public class CreatePatientDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public InsuranceType? InsuranceType { get; set; }
    public string? InsuranceNumber { get; set; }
    public string? InsuranceProvider { get; set; }
    public string? Kvnr { get; set; }
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
}

public class UpdatePatientDto : CreatePatientDto
{
    public Guid Id { get; set; }
}

public class PatientSearchDto
{
    public string? SearchTerm { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPages;
}
