using MediPrax.Application.DTOs;
using MediPrax.Application.Interfaces;
using MediPrax.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediPrax.Application.Services;

public class MedicationService(DbContext context) : IMedicationService
{
    private DbSet<Medication> Medications => context.Set<Medication>();

    public async Task<IReadOnlyList<MedicationDto>> GetByPatientAsync(Guid patientId, bool activeOnly = true, CancellationToken ct = default)
    {
        var query = Medications
            .Include(m => m.PrescribedBy)
            .Where(m => m.PatientId == patientId);

        if (activeOnly)
            query = query.Where(m => m.IsActive);

        return await query
            .OrderBy(m => m.Name)
            .Select(m => new MedicationDto
            {
                Id = m.Id, PatientId = m.PatientId,
                Name = m.Name, Wirkstoff = m.Wirkstoff, Pzn = m.Pzn,
                Staerke = m.Staerke, Dosierung = m.Dosierung,
                Einheit = m.Einheit, Hinweis = m.Hinweis,
                SeitDatum = m.SeitDatum, BisDatum = m.BisDatum,
                IsBtm = m.IsBtm, IsActive = m.IsActive,
                PrescribedByName = m.PrescribedBy.FirstName + " " + m.PrescribedBy.LastName,
                Category = m.Category, TargetDose = m.TargetDose,
                IsDepot = m.IsDepot, DepotIntervalDays = m.DepotIntervalDays,
                LastDepotDate = m.LastDepotDate, NextDepotDate = m.NextDepotDate,
                RequiresMonitoring = m.RequiresMonitoring, MonitoringType = m.MonitoringType
            })
            .ToListAsync(ct);
    }

    public async Task<MedicationDto> AddAsync(CreateMedicationDto dto, CancellationToken ct = default)
    {
        var med = new Medication
        {
            PatientId = dto.PatientId, PrescribedById = dto.PrescribedById,
            Name = dto.Name, Wirkstoff = dto.Wirkstoff, Pzn = dto.Pzn,
            Staerke = dto.Staerke, Dosierung = dto.Dosierung,
            Einheit = dto.Einheit, Hinweis = dto.Hinweis,
            SeitDatum = dto.SeitDatum, BisDatum = dto.BisDatum,
            IsBtm = dto.IsBtm,
            Category = dto.Category, TargetDose = dto.TargetDose,
            IsDepot = dto.IsDepot, DepotIntervalDays = dto.DepotIntervalDays,
            LastDepotDate = dto.LastDepotDate,
            RequiresMonitoring = dto.RequiresMonitoring, MonitoringType = dto.MonitoringType
        };
        Medications.Add(med);
        await context.SaveChangesAsync(ct);

        return (await GetByPatientAsync(dto.PatientId, false, ct)).First(m => m.Id == med.Id);
    }

    public async Task DeactivateAsync(Guid id, CancellationToken ct = default)
    {
        var med = await Medications.FindAsync([id], ct)
            ?? throw new KeyNotFoundException("Medikament nicht gefunden.");
        med.IsActive = false;
        med.BisDatum = DateOnly.FromDateTime(DateTime.Today);
        await context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var med = await Medications.FindAsync([id], ct)
            ?? throw new KeyNotFoundException("Medikament nicht gefunden.");
        Medications.Remove(med);
        await context.SaveChangesAsync(ct);
    }
}
