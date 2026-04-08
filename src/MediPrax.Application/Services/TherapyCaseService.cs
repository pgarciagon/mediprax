using MediPrax.Application.DTOs;
using MediPrax.Application.Interfaces;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;

namespace MediPrax.Application.Services;

public class TherapyCaseService(DbContext context) : ITherapyCaseService
{
    private DbSet<TherapyCase> TherapyCases => context.Set<TherapyCase>();
    private DbSet<TherapySession> TherapySessions => context.Set<TherapySession>();
    private DbSet<PtvForm> PtvForms => context.Set<PtvForm>();

    public async Task<TherapyCaseDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var tc = await TherapyCases
            .Include(x => x.Patient)
            .Include(x => x.Therapist)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        return tc is null ? null : MapToDto(tc);
    }

    public async Task<IReadOnlyList<TherapyCaseListItemDto>> GetByPatientAsync(Guid patientId, CancellationToken ct = default)
    {
        return await TherapyCases
            .Include(x => x.Patient)
            .Where(x => x.PatientId == patientId)
            .OrderByDescending(x => x.StartDate)
            .Select(x => MapToListItem(x))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<TherapyCaseListItemDto>> GetActiveByTherapistAsync(Guid therapistId, CancellationToken ct = default)
    {
        return await TherapyCases
            .Include(x => x.Patient)
            .Where(x => x.TherapistId == therapistId &&
                         x.Status != TherapyCaseStatus.Abgeschlossen &&
                         x.Status != TherapyCaseStatus.Abgebrochen)
            .OrderByDescending(x => x.StartDate)
            .Select(x => MapToListItem(x))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<TherapyCaseListItemDto>> GetAllActiveAsync(CancellationToken ct = default)
    {
        return await TherapyCases
            .Include(x => x.Patient)
            .Where(x => x.Status != TherapyCaseStatus.Abgeschlossen &&
                         x.Status != TherapyCaseStatus.Abgebrochen)
            .OrderByDescending(x => x.StartDate)
            .Select(x => MapToListItem(x))
            .ToListAsync(ct);
    }

    public async Task<TherapyCaseDto> CreateAsync(CreateTherapyCaseDto dto, CancellationToken ct = default)
    {
        var therapyCase = new TherapyCase
        {
            PatientId = dto.PatientId,
            TherapistId = dto.TherapistId,
            TherapyType = dto.TherapyType,
            Status = TherapyCaseStatus.SprechstundePhase,
            StartDate = dto.StartDate,
            ApprovedSessions = dto.ApprovedSessions,
            SessionDurationMinutes = dto.SessionDurationMinutes,
            IsGroupTherapy = dto.IsGroupTherapy,
            Diagnoses = dto.Diagnoses,
            Notes = dto.Notes
        };

        TherapyCases.Add(therapyCase);
        await context.SaveChangesAsync(ct);

        var created = await TherapyCases
            .Include(x => x.Patient)
            .Include(x => x.Therapist)
            .FirstAsync(x => x.Id == therapyCase.Id, ct);

        return MapToDto(created);
    }

    public async Task<TherapyCaseDto> UpdateAsync(UpdateTherapyCaseDto dto, CancellationToken ct = default)
    {
        var therapyCase = await TherapyCases
            .Include(x => x.Patient)
            .Include(x => x.Therapist)
            .FirstOrDefaultAsync(x => x.Id == dto.Id, ct)
            ?? throw new KeyNotFoundException($"TherapyCase {dto.Id} not found");

        therapyCase.TherapyType = dto.TherapyType;
        therapyCase.Status = dto.Status;
        therapyCase.ApprovedSessions = dto.ApprovedSessions;
        therapyCase.SessionDurationMinutes = dto.SessionDurationMinutes;
        therapyCase.IsGroupTherapy = dto.IsGroupTherapy;
        therapyCase.Diagnoses = dto.Diagnoses;
        therapyCase.Notes = dto.Notes;
        therapyCase.GutachterStatus = dto.GutachterStatus;
        therapyCase.EndDate = dto.EndDate;
        therapyCase.InsuranceApprovalRef = dto.InsuranceApprovalRef;

        await context.SaveChangesAsync(ct);
        return MapToDto(therapyCase);
    }

    public async Task<TherapySessionDto> AddSessionAsync(CreateTherapySessionDto dto, CancellationToken ct = default)
    {
        var therapyCase = await TherapyCases
            .FirstOrDefaultAsync(x => x.Id == dto.TherapyCaseId, ct)
            ?? throw new KeyNotFoundException($"TherapyCase {dto.TherapyCaseId} not found");

        var maxNumber = await TherapySessions
            .Where(s => s.TherapyCaseId == dto.TherapyCaseId)
            .Select(s => (int?)s.SessionNumber)
            .MaxAsync(ct) ?? 0;
        var nextNumber = maxNumber + 1;

        var session = new TherapySession
        {
            TherapyCaseId = dto.TherapyCaseId,
            EncounterId = dto.EncounterId,
            AppointmentId = dto.AppointmentId,
            SessionNumber = nextNumber,
            SessionDate = dto.SessionDate,
            DurationMinutes = dto.DurationMinutes,
            SessionType = dto.SessionType,
            IsVideoSession = dto.IsVideoSession,
            Notes = dto.Notes,
            BilledGop = dto.BilledGop
        };

        TherapySessions.Add(session);
        therapyCase.CompletedSessions = nextNumber;
        await context.SaveChangesAsync(ct);

        return MapSessionToDto(session);
    }

    public async Task<IReadOnlyList<TherapySessionDto>> GetSessionsAsync(Guid caseId, CancellationToken ct = default)
    {
        return await TherapySessions
            .Where(s => s.TherapyCaseId == caseId)
            .OrderBy(s => s.SessionNumber)
            .Select(s => new TherapySessionDto
            {
                Id = s.Id,
                TherapyCaseId = s.TherapyCaseId,
                SessionNumber = s.SessionNumber,
                SessionDate = s.SessionDate,
                DurationMinutes = s.DurationMinutes,
                SessionType = s.SessionType,
                IsVideoSession = s.IsVideoSession,
                Notes = s.Notes,
                BilledGop = s.BilledGop
            })
            .ToListAsync(ct);
    }

    public async Task<ContingentStatusDto> GetContingentStatusAsync(Guid caseId, CancellationToken ct = default)
    {
        var therapyCase = await TherapyCases
            .FirstOrDefaultAsync(x => x.Id == caseId, ct)
            ?? throw new KeyNotFoundException($"TherapyCase {caseId} not found");

        var contingent = TherapyContingentRules.GetContingentInfo(therapyCase.TherapyType);
        var remaining = therapyCase.ApprovedSessions - therapyCase.CompletedSessions;
        var warningThreshold = TherapyContingentRules.GetWarningThreshold(therapyCase.ApprovedSessions);

        string warningLevel;
        string? warningMessage;

        if (remaining <= 0)
        {
            warningLevel = "Red";
            warningMessage = "Kontingent erschoepft - keine Sitzungen mehr verfuegbar.";
        }
        else if (therapyCase.CompletedSessions >= warningThreshold)
        {
            warningLevel = "Yellow";
            warningMessage = $"Nur noch {remaining} Sitzungen verbleibend.";
        }
        else
        {
            warningLevel = "None";
            warningMessage = null;
        }

        var canExtend = contingent.MaxExtension.HasValue &&
                        therapyCase.ApprovedSessions < contingent.MaxTotal;
        var extensionPossible = canExtend
            ? contingent.MaxTotal - therapyCase.ApprovedSessions
            : (int?)null;

        return new ContingentStatusDto
        {
            ApprovedSessions = therapyCase.ApprovedSessions,
            CompletedSessions = therapyCase.CompletedSessions,
            RemainingSessions = Math.Max(0, remaining),
            MaxTotal = contingent.MaxTotal,
            RequiresGutachter = contingent.RequiresGutachter,
            WarningLevel = warningLevel,
            WarningMessage = warningMessage,
            CanExtend = canExtend,
            ExtensionPossible = extensionPossible
        };
    }

    public async Task<PtvFormDto> GeneratePtvFormAsync(CreatePtvFormDto dto, CancellationToken ct = default)
    {
        var therapyCase = await TherapyCases
            .Include(x => x.Patient)
            .Include(x => x.Therapist)
            .FirstOrDefaultAsync(x => x.Id == dto.TherapyCaseId, ct)
            ?? throw new KeyNotFoundException($"TherapyCase {dto.TherapyCaseId} not found");

        var formData = new Dictionary<string, string>
        {
            ["PatientName"] = therapyCase.Patient?.FullName ?? "",
            ["PatientGeburtsdatum"] = therapyCase.Patient?.DateOfBirth.ToString("dd.MM.yyyy") ?? "",
            ["Kvnr"] = therapyCase.Patient?.Kvnr ?? "",
            ["Krankenkasse"] = therapyCase.Patient?.InsuranceProvider ?? "",
            ["Versichertennummer"] = therapyCase.Patient?.InsuranceNumber ?? "",
            ["TherapeutName"] = therapyCase.Therapist?.FullName ?? "",
            ["Therapieverfahren"] = GetTherapyMethodName(therapyCase.TherapyType),
            ["BeantragteSitzungen"] = therapyCase.ApprovedSessions.ToString(),
            ["Diagnosen"] = string.Join(", ", therapyCase.Diagnoses),
            ["Datum"] = DateOnly.FromDateTime(DateTime.Today).ToString("dd.MM.yyyy")
        };

        // Generate PDF
        var ptvData = new MediPrax.Reporting.PtvForms.PtvFormData
        {
            PatientName = formData["PatientName"],
            PatientGeburtsdatum = formData["PatientGeburtsdatum"],
            Kvnr = formData["Kvnr"],
            Krankenkasse = formData["Krankenkasse"],
            Versichertennummer = formData["Versichertennummer"],
            TherapeutName = formData["TherapeutName"],
            Therapieverfahren = formData["Therapieverfahren"],
            BeantragteSitzungen = formData["BeantragteSitzungen"],
            Diagnosen = formData["Diagnosen"],
            Datum = formData["Datum"]
        };

        byte[]? pdfBytes = null;
        if (dto.FormType == PtvFormType.PTV1)
        {
            var doc = new MediPrax.Reporting.PtvForms.Ptv1Document(ptvData);
            pdfBytes = doc.GeneratePdf();
        }
        else if (dto.FormType == PtvFormType.PTV2)
        {
            var sprechstunden = await TherapySessions.CountAsync(s => s.TherapyCaseId == dto.TherapyCaseId && s.SessionType == SessionType.Sprechstunde, ct);
            var probatorik = await TherapySessions.CountAsync(s => s.TherapyCaseId == dto.TherapyCaseId && s.SessionType == SessionType.Probatorik, ct);
            var doc = new MediPrax.Reporting.PtvForms.Ptv2Document(ptvData, sprechstunden, probatorik);
            pdfBytes = doc.GeneratePdf();
        }
        else if (dto.FormType == PtvFormType.PTV3)
        {
            var doc = new MediPrax.Reporting.PtvForms.Ptv3Document(ptvData);
            pdfBytes = doc.GeneratePdf();
        }
        else if (dto.FormType == PtvFormType.PTV10)
        {
            var doc = new MediPrax.Reporting.PtvForms.Ptv10Document(ptvData);
            pdfBytes = doc.GeneratePdf();
        }

        var form = new PtvForm
        {
            TherapyCaseId = dto.TherapyCaseId,
            FormType = dto.FormType,
            CreatedDate = DateOnly.FromDateTime(DateTime.Today),
            FormData = formData,
            PdfData = pdfBytes,
            Status = PtvFormStatus.Draft
        };

        PtvForms.Add(form);
        await context.SaveChangesAsync(ct);

        return MapFormToDto(form);
    }

    public async Task<IReadOnlyList<PtvFormDto>> GetPtvFormsAsync(Guid caseId, CancellationToken ct = default)
    {
        return await PtvForms
            .Where(f => f.TherapyCaseId == caseId)
            .OrderByDescending(f => f.CreatedDate)
            .Select(f => new PtvFormDto
            {
                Id = f.Id,
                TherapyCaseId = f.TherapyCaseId,
                FormType = f.FormType,
                FormTypeName = GetFormTypeName(f.FormType),
                CreatedDate = f.CreatedDate,
                Status = f.Status,
                HasPdf = f.PdfData != null
            })
            .ToListAsync(ct);
    }

    public async Task<byte[]?> GetPtvFormPdfAsync(Guid formId, CancellationToken ct = default)
    {
        var form = await PtvForms
            .FirstOrDefaultAsync(f => f.Id == formId, ct);
        return form?.PdfData;
    }

    public async Task UpdatePtvFormStatusAsync(Guid formId, PtvFormStatus newStatus, CancellationToken ct = default)
    {
        var form = await PtvForms.FindAsync([formId], ct)
            ?? throw new KeyNotFoundException($"PtvForm {formId} not found");
        form.Status = newStatus;
        await context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var therapyCase = await TherapyCases.FindAsync([id], ct)
            ?? throw new KeyNotFoundException($"TherapyCase {id} not found");

        TherapyCases.Remove(therapyCase);
        await context.SaveChangesAsync(ct);
    }

    private static TherapyCaseDto MapToDto(TherapyCase tc)
    {
        var remaining = tc.ApprovedSessions - tc.CompletedSessions;
        var warningThreshold = TherapyContingentRules.GetWarningThreshold(tc.ApprovedSessions);
        string warningLevel;
        if (remaining <= 0) warningLevel = "Red";
        else if (tc.CompletedSessions >= warningThreshold) warningLevel = "Yellow";
        else warningLevel = "None";

        return new TherapyCaseDto
        {
            Id = tc.Id,
            PatientId = tc.PatientId,
            PatientName = tc.Patient?.FullName ?? "",
            TherapistId = tc.TherapistId,
            TherapistName = tc.Therapist?.FullName ?? "",
            TherapyType = tc.TherapyType,
            TherapyTypeName = GetTherapyTypeName(tc.TherapyType),
            Status = tc.Status,
            StartDate = tc.StartDate,
            EndDate = tc.EndDate,
            ApprovedSessions = tc.ApprovedSessions,
            CompletedSessions = tc.CompletedSessions,
            RemainingSessions = Math.Max(0, remaining),
            SessionDurationMinutes = tc.SessionDurationMinutes,
            IsGroupTherapy = tc.IsGroupTherapy,
            GutachterStatus = tc.GutachterStatus,
            Diagnoses = tc.Diagnoses,
            Notes = tc.Notes,
            WarningLevel = warningLevel
        };
    }

    private static TherapyCaseListItemDto MapToListItem(TherapyCase tc)
    {
        var remaining = tc.ApprovedSessions - tc.CompletedSessions;
        var warningThreshold = TherapyContingentRules.GetWarningThreshold(tc.ApprovedSessions);
        string warningLevel;
        if (remaining <= 0) warningLevel = "Red";
        else if (tc.CompletedSessions >= warningThreshold) warningLevel = "Yellow";
        else warningLevel = "None";

        return new TherapyCaseListItemDto
        {
            Id = tc.Id,
            PatientName = tc.Patient?.FullName ?? "",
            TherapyTypeName = GetTherapyTypeName(tc.TherapyType),
            Status = tc.Status,
            CompletedSessions = tc.CompletedSessions,
            ApprovedSessions = tc.ApprovedSessions,
            RemainingSessions = Math.Max(0, remaining),
            StartDate = tc.StartDate,
            WarningLevel = warningLevel
        };
    }

    private static TherapySessionDto MapSessionToDto(TherapySession s) => new()
    {
        Id = s.Id,
        TherapyCaseId = s.TherapyCaseId,
        SessionNumber = s.SessionNumber,
        SessionDate = s.SessionDate,
        DurationMinutes = s.DurationMinutes,
        SessionType = s.SessionType,
        IsVideoSession = s.IsVideoSession,
        Notes = s.Notes,
        BilledGop = s.BilledGop
    };

    private static PtvFormDto MapFormToDto(PtvForm f) => new()
    {
        Id = f.Id,
        TherapyCaseId = f.TherapyCaseId,
        FormType = f.FormType,
        FormTypeName = GetFormTypeName(f.FormType),
        CreatedDate = f.CreatedDate,
        Status = f.Status,
        HasPdf = f.PdfData is not null
    };

    private static string GetTherapyTypeName(TherapyType type) => type switch
    {
        TherapyType.PsychotherapeutischeSprechstunde => "Psychotherapeutische Sprechstunde",
        TherapyType.Probatorik => "Probatorik",
        TherapyType.Akutbehandlung => "Akutbehandlung",
        TherapyType.KurzzeittherapieKZT1 => "Kurzzeittherapie (KZT1)",
        TherapyType.KurzzeittherapieKZT2 => "Kurzzeittherapie (KZT2)",
        TherapyType.LangzeittherapieVT => "Langzeittherapie VT",
        TherapyType.LangzeittherapieTP => "Langzeittherapie TP",
        TherapyType.LangzeittherapieAP => "Langzeittherapie AP",
        TherapyType.LangzeittherapieST => "Langzeittherapie ST",
        TherapyType.NeuropsychologischeTherapie => "Neuropsychologische Therapie",
        _ => type.ToString()
    };

    private static string GetTherapyMethodName(TherapyType type) => type switch
    {
        TherapyType.LangzeittherapieVT or TherapyType.KurzzeittherapieKZT1 or TherapyType.KurzzeittherapieKZT2 => "Verhaltenstherapie",
        TherapyType.LangzeittherapieTP => "Tiefenpsychologisch fundierte Psychotherapie",
        TherapyType.LangzeittherapieAP => "Analytische Psychotherapie",
        TherapyType.LangzeittherapieST => "Systemische Therapie",
        TherapyType.NeuropsychologischeTherapie => "Neuropsychologische Therapie",
        _ => type.ToString()
    };

    private static string GetFormTypeName(PtvFormType type) => type switch
    {
        PtvFormType.PTV1 => "PTV 1 - Antrag des Versicherten",
        PtvFormType.PTV2 => "PTV 2 - Angaben des Therapeuten",
        PtvFormType.PTV3 => "PTV 3 - Bericht an den Gutachter",
        PtvFormType.PTV10 => "PTV 10 - Akutbehandlung Anzeige",
        PtvFormType.PTV11 => "PTV 11 - Probatorische Sitzungen",
        PtvFormType.PTV12 => "PTV 12 - Akutbehandlung Information",
        _ => type.ToString()
    };
}
