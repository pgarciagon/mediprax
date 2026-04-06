using MediPrax.Application.DTOs;
using MediPrax.Application.Interfaces;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace MediPrax.Application.Services;

public class EncounterService(DbContext context) : IEncounterService
{
    private DbSet<Encounter> Encounters => context.Set<Encounter>();

    public async Task<EncounterDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var e = await Encounters
            .Include(x => x.Patient)
            .Include(x => x.Doctor)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        return e is null ? null : MapToDto(e);
    }

    public async Task<IReadOnlyList<EncounterListItemDto>> GetByPatientAsync(Guid patientId, CancellationToken ct = default)
    {
        return await Encounters
            .Include(x => x.Doctor)
            .Where(x => x.PatientId == patientId)
            .OrderByDescending(x => x.EncounterDate)
            .Select(x => new EncounterListItemDto
            {
                Id = x.Id,
                EncounterDate = x.EncounterDate,
                DoctorName = x.Doctor.FirstName + " " + x.Doctor.LastName,
                Icd10Codes = x.Icd10Codes,
                DurationMinutes = x.DurationMinutes,
                Status = x.Status
            })
            .ToListAsync(ct);
    }

    public async Task<EncounterDto> CreateAsync(CreateEncounterDto dto, CancellationToken ct = default)
    {
        var encounter = new Encounter
        {
            PatientId = dto.PatientId,
            DoctorId = dto.DoctorId,
            AppointmentId = dto.AppointmentId,
            EncounterDate = dto.EncounterDate,
            Notes = dto.Notes,
            Icd10Codes = dto.Icd10Codes,
            DurationMinutes = dto.DurationMinutes,
            Status = EncounterStatus.Open
        };

        Encounters.Add(encounter);
        await context.SaveChangesAsync(ct);

        var created = await Encounters
            .Include(x => x.Patient)
            .Include(x => x.Doctor)
            .FirstAsync(x => x.Id == encounter.Id, ct);

        return MapToDto(created);
    }

    public async Task<EncounterDto> UpdateAsync(UpdateEncounterDto dto, CancellationToken ct = default)
    {
        var encounter = await Encounters
            .Include(x => x.Patient)
            .Include(x => x.Doctor)
            .FirstOrDefaultAsync(x => x.Id == dto.Id, ct)
            ?? throw new KeyNotFoundException($"Encounter {dto.Id} not found");

        encounter.Notes = dto.Notes;
        encounter.Icd10Codes = dto.Icd10Codes;
        encounter.DurationMinutes = dto.DurationMinutes;
        encounter.Status = dto.Status;

        await context.SaveChangesAsync(ct);
        return MapToDto(encounter);
    }

    private static EncounterDto MapToDto(Encounter e) => new()
    {
        Id = e.Id,
        PatientId = e.PatientId,
        PatientName = e.Patient?.LastName + ", " + e.Patient?.FirstName,
        DoctorId = e.DoctorId,
        DoctorName = e.Doctor?.FirstName + " " + e.Doctor?.LastName,
        AppointmentId = e.AppointmentId,
        EncounterDate = e.EncounterDate,
        Notes = e.Notes,
        Icd10Codes = e.Icd10Codes.Select(c => new Icd10EntryDto { Code = c }).ToList(),
        DurationMinutes = e.DurationMinutes,
        Status = e.Status,
        CreatedAt = e.CreatedAt
    };
}
