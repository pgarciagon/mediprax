using MediPrax.Application.DTOs;
using MediPrax.Application.Interfaces;
using MediPrax.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediPrax.Application.Services;

public class PatientService(DbContext context) : IPatientService
{
    private DbSet<Patient> Patients => context.Set<Patient>();

    public async Task<PatientDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var patient = await Patients.FindAsync([id], ct);
        return patient is null ? null : MapToDto(patient);
    }

    public async Task<PagedResult<PatientListItemDto>> SearchAsync(PatientSearchDto search, CancellationToken ct = default)
    {
        var query = Patients.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search.SearchTerm))
        {
            var term = search.SearchTerm.Trim().ToLower();
            query = query.Where(p =>
                p.LastName.ToLower().Contains(term) ||
                p.FirstName.ToLower().Contains(term) ||
                (p.Kvnr != null && p.Kvnr.Contains(term)) ||
                (p.Phone != null && p.Phone.Contains(term)));
        }

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderBy(p => p.LastName).ThenBy(p => p.FirstName)
            .Skip((search.Page - 1) * search.PageSize)
            .Take(search.PageSize)
            .Select(p => new PatientListItemDto
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                DateOfBirth = p.DateOfBirth,
                InsuranceType = p.InsuranceType,
                Kvnr = p.Kvnr,
                Phone = p.Phone
            })
            .ToListAsync(ct);

        return new PagedResult<PatientListItemDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = search.Page,
            PageSize = search.PageSize
        };
    }

    public async Task<PatientDto> CreateAsync(CreatePatientDto dto, CancellationToken ct = default)
    {
        var patient = new Patient
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            DateOfBirth = dto.DateOfBirth,
            Gender = dto.Gender,
            InsuranceType = dto.InsuranceType,
            InsuranceNumber = dto.InsuranceNumber,
            InsuranceProvider = dto.InsuranceProvider,
            Kvnr = dto.Kvnr,
            Street = dto.Street,
            City = dto.City,
            PostalCode = dto.PostalCode,
            Phone = dto.Phone,
            Email = dto.Email
        };

        Patients.Add(patient);
        await context.SaveChangesAsync(ct);
        return MapToDto(patient);
    }

    public async Task<PatientDto> UpdateAsync(UpdatePatientDto dto, CancellationToken ct = default)
    {
        var patient = await Patients.FindAsync([dto.Id], ct)
            ?? throw new KeyNotFoundException($"Patient {dto.Id} not found");
    
        patient.FirstName = dto.FirstName;
        patient.LastName = dto.LastName;
        patient.DateOfBirth = dto.DateOfBirth;
        patient.Gender = dto.Gender;
        patient.InsuranceType = dto.InsuranceType;
        patient.InsuranceNumber = dto.InsuranceNumber;
        patient.InsuranceProvider = dto.InsuranceProvider;
        patient.Kvnr = dto.Kvnr;
        patient.Street = dto.Street;
        patient.City = dto.City;
        patient.PostalCode = dto.PostalCode;
        patient.Phone = dto.Phone;
        patient.Email = dto.Email;

        await context.SaveChangesAsync(ct);
        return MapToDto(patient);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var patient = await Patients.FindAsync([id], ct)
            ?? throw new KeyNotFoundException($"Patient {id} not found");

        Patients.Remove(patient);
        await context.SaveChangesAsync(ct);
    }

    private static PatientDto MapToDto(Patient p) => new()
    {
        Id = p.Id,
        FirstName = p.FirstName,
        LastName = p.LastName,
        DateOfBirth = p.DateOfBirth,
        Gender = p.Gender,
        InsuranceType = p.InsuranceType,
        InsuranceNumber = p.InsuranceNumber,
        InsuranceProvider = p.InsuranceProvider,
        Kvnr = p.Kvnr,
        Street = p.Street,
        City = p.City,
        PostalCode = p.PostalCode,
        Phone = p.Phone,
        Email = p.Email
    };
}
