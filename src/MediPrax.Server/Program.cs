using System.Security.Claims;
using MediPrax.Application.Interfaces;
using MediPrax.Application.Services;
using MediPrax.Core.Enums;
using MediPrax.Core.Interfaces;
using MediPrax.Infrastructure.Persistence;
using MediPrax.Infrastructure.Services;
using MediPrax.Server.Components;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using QuestPDF.Fluent;

QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuditInterceptor>();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var dataSourceBuilder = new Npgsql.NpgsqlDataSourceBuilder(connectionString);
dataSourceBuilder.EnableDynamicJson();
var dataSource = dataSourceBuilder.Build();

builder.Services.AddDbContext<MediPraxDbContext>((sp, options) =>
    options
        .UseNpgsql(dataSource)
        .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning))
        .AddInterceptors(sp.GetRequiredService<AuditInterceptor>()));

// Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Services
builder.Services.AddScoped<DbContext>(sp => sp.GetRequiredService<MediPraxDbContext>());
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEncounterService, EncounterService>();
builder.Services.AddScoped<IEncounterSectionService, EncounterSectionService>();
builder.Services.AddScoped<IPatientDiagnosisService, PatientDiagnosisService>();
builder.Services.AddScoped<IArztbriefService, MediPrax.Server.Services.ArztbriefService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IBillingService, BillingService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IMedicationService, MedicationService>();
builder.Services.AddScoped<IRecallService, RecallService>();
builder.Services.AddScoped<IPsychopathFindingService, PsychopathFindingService>();
builder.Services.AddScoped<ILabResultService, LabResultService>();
builder.Services.AddScoped<IKvdtExportService, KvdtExportService>();
builder.Services.AddScoped<IBillingPlausibilityService, BillingPlausibilityService>();
builder.Services.AddScoped<IGopSuggestionService, GopSuggestionService>();
builder.Services.AddScoped<IImportService, ImportService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IPsychometricTestService, PsychometricTestService>();
builder.Services.AddScoped<ITherapyCaseService, TherapyCaseService>();
builder.Services.AddScoped<INeurologicalExamService, NeurologicalExamService>();
builder.Services.AddScoped<IIcd10CodeService, Icd10CodeService>();
builder.Services.AddScoped<ISuicidalityAssessmentService, SuicidalityAssessmentService>();
builder.Services.AddScoped<IAvailabilityService, AvailabilityService>();
builder.Services.AddScoped<ISlotSuggestionService, SlotSuggestionService>();
builder.Services.AddScoped<IGdtService, GdtService>();
builder.Services.Configure<MediPrax.Application.DTOs.GdtDevicesOptions>(
    builder.Configuration.GetSection(MediPrax.Application.DTOs.GdtDevicesOptions.SectionName));
builder.Services.AddHostedService<MediPrax.Server.Services.GdtFileWatcherService>();
builder.Services.AddScoped<ISeizureDiaryService, SeizureDiaryService>();
builder.Services.AddScoped<IHeadacheDiaryService, HeadacheDiaryService>();
builder.Services.AddScoped<IMsDocumentationService, MsDocumentationService>();
builder.Services.AddScoped<IParkinsonDocumentationService, ParkinsonDocumentationService>();
builder.Services.AddScoped<IAuthService, MediPrax.Server.Services.AuthService>();

// M30: Text Modules
builder.Services.AddScoped<ITextModuleService, TextModuleService>();

// M31: DMP
builder.Services.AddScoped<IDmpService, DmpService>();

// M33: Private Invoices
builder.Services.AddScoped<IPrivateInvoiceService, PrivateInvoiceService>();

// M35: BtM Compliance
builder.Services.AddScoped<IBtmComplianceService, BtmComplianceService>();

// M46: Action Chains
builder.Services.AddScoped<IActionChainService, ActionChainService>();
builder.Services.AddScoped<IActionChainExecutor, ActionChainExecutor>();

// Telematik — Mock services (replace with real implementations when TI access is available)
builder.Services.AddSingleton<MediPrax.Application.Interfaces.Telematik.IEgkService, MediPrax.Server.Services.Telematik.MockEgkService>();
builder.Services.AddSingleton<MediPrax.Application.Interfaces.Telematik.IERezeptService, MediPrax.Server.Services.Telematik.MockERezeptService>();
builder.Services.AddSingleton<MediPrax.Application.Interfaces.Telematik.IKimService, MediPrax.Server.Services.Telematik.MockKimService>();
builder.Services.AddSingleton<MediPrax.Application.Interfaces.Telematik.IEpaService, MediPrax.Server.Services.Telematik.MockEpaService>();
builder.Services.AddSingleton<MediPrax.Application.Interfaces.Konnektor.IKonnektorClient, MediPrax.Server.Services.Telematik.MockKonnektorClient>();
// M34: eAU
builder.Services.AddSingleton<MediPrax.Application.Interfaces.Telematik.IEauService, MediPrax.Server.Services.Telematik.MockEauService>();
builder.Services.AddScoped<IAuditService, AuditService>();

// Health Checks
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection")!, name: "postgresql");

// Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = 429;
    options.AddPolicy("login", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1)
            }));
});

// Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.ExpireTimeSpan = TimeSpan.FromHours(12);
        options.SlidingExpiration = true;
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("Arzt", policy => policy.RequireRole("Arzt", "Admin"));
    options.AddPolicy("Klinisch", policy => policy.RequireRole("Arzt", "MFA", "Admin"));
    options.AddPolicy("Empfang", policy => policy.RequireRole("Arzt", "MFA", "Empfang", "Admin"));
});
builder.Services.AddCascadingAuthenticationState();

// Blazor
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

// Serve /docs/ MkDocs user manual — must be before StatusCodePages to avoid Blazor 404 catch
var docsPath = Path.Combine(app.Environment.WebRootPath, "docs");
if (Directory.Exists(docsPath))
{
    var docsProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(docsPath);
    app.UseStaticFiles(new StaticFileOptions
    {
        RequestPath = "/docs",
        FileProvider = docsProvider
    });
    app.Use(async (context, next) =>
    {
        var path = context.Request.Path.Value;
        if (path != null && path.StartsWith("/docs", StringComparison.OrdinalIgnoreCase))
        {
            var relativePath = path["/docs".Length..].TrimStart('/');
            var indexPath = Path.Combine(docsPath, relativePath, "index.html");
            if (File.Exists(indexPath))
            {
                context.Response.ContentType = "text/html";
                await context.Response.SendFileAsync(indexPath);
                return;
            }
        }
        await next();
    });
}

app.UseStatusCodePagesWithReExecute("/not-found");
app.UseRateLimiter();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

// Health check endpoint
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new { name = e.Key, status = e.Value.Status.ToString(), duration = e.Value.Duration.TotalMilliseconds }),
            totalDuration = report.TotalDuration.TotalMilliseconds
        };
        await context.Response.WriteAsJsonAsync(result);
    }
});

// Login endpoint
app.MapPost("/api/login", async (HttpContext httpContext, IAuthService authService, IAuditService auditService, LoginRequest request) =>
{
    var ip = httpContext.Connection.RemoteIpAddress?.ToString();
    var user = await authService.ValidateCredentialsAsync(request.Email, request.Password);
    if (user is null)
    {
        // Log failed login — write directly since no user context yet
        var db = httpContext.RequestServices.GetRequiredService<MediPraxDbContext>();
        db.AuditLogs.Add(new MediPrax.Core.Entities.AuditLog
        {
            Action = AuditAction.LoginFailed,
            Details = $"Fehlgeschlagener Login: {request.Email}",
            IpAddress = ip
        });
        await db.SaveChangesAsync();
        return Results.Unauthorized();
    }

    var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new(ClaimTypes.Name, user.FullName),
        new(ClaimTypes.Email, user.Email),
        new(ClaimTypes.Role, user.Role.ToString())
    };

    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    var principal = new ClaimsPrincipal(identity);

    await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

    // Log successful login
    var dbCtx = httpContext.RequestServices.GetRequiredService<MediPraxDbContext>();
    dbCtx.AuditLogs.Add(new MediPrax.Core.Entities.AuditLog
    {
        UserId = user.Id,
        UserName = user.FullName,
        UserRole = user.Role.ToString(),
        Action = AuditAction.Login,
        Details = "Erfolgreicher Login",
        IpAddress = ip
    });
    await dbCtx.SaveChangesAsync();

    return Results.Ok();
}).RequireRateLimiting("login");

// Logout endpoint
app.MapGet("/api/logout", async (HttpContext httpContext, IAuditService auditService) =>
{
    await auditService.LogAsync(AuditAction.Logout, details: "Abmeldung");
    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/login");
});

// PDF endpoint
app.MapGet("/dokumente/{id:guid}/pdf", async (Guid id, IArztbriefService arztbriefService, IAuditService auditService) =>
{
    var pdf = await arztbriefService.GetPdfAsync(id);
    if (pdf is null) return Results.NotFound();
    await auditService.LogAsync(AuditAction.Export, "Document", id, "PDF heruntergeladen");
    return Results.File(pdf, "application/pdf", $"Arztbrief-{id:N}.pdf");
});

// PDF inline preview (no download header)
app.MapGet("/api/arztbrief/{id:guid}/preview", async (Guid id, IArztbriefService arztbriefService) =>
{
    var pdf = await arztbriefService.GetPdfAsync(id);
    if (pdf is null) return Results.NotFound();
    return Results.File(pdf, "application/pdf");
}).RequireAuthorization("Klinisch");

// Formulare PDF endpoints
app.MapGet("/api/formulare/rezept", async (Guid patientId, string medikament, string? dosierung, string? pzn, int menge, bool privat, IPatientService patientService) =>
{
    var patient = await patientService.GetByIdAsync(patientId);
    if (patient is null) return Results.NotFound();
    var doc = new MediPrax.Reporting.Formulare.RezeptDocument(new MediPrax.Reporting.Formulare.RezeptData
    {
        PatientName = patient.FullName, PatientGeburtsdatum = patient.DateOfBirth.ToString("dd.MM.yyyy"),
        PatientAdresse = $"{patient.Street}, {patient.PostalCode} {patient.City}",
        Kvnr = patient.Kvnr ?? "", Krankenkasse = patient.InsuranceProvider ?? "",
        Versichertennummer = patient.InsuranceNumber ?? "",
        ArztName = "Dr. med.", Datum = DateTime.Today.ToString("dd.MM.yyyy"), IsPrivat = privat,
        Positionen = [new() { Medikament = medikament, Pzn = pzn, Dosierung = dosierung, Menge = menge }]
    });
    var pdf = doc.GeneratePdf();
    return Results.File(pdf, "application/pdf", $"Rezept_{patient.LastName}.pdf");
}).RequireAuthorization("Klinisch");

app.MapGet("/api/formulare/ueberweisung", async (Guid patientId, string fachrichtung, string? arzt, string diagnose, string? frage, IPatientService patientService) =>
{
    var patient = await patientService.GetByIdAsync(patientId);
    if (patient is null) return Results.NotFound();
    var doc = new MediPrax.Reporting.Formulare.UeberweisungDocument(new MediPrax.Reporting.Formulare.UeberweisungData
    {
        PatientName = patient.FullName, PatientGeburtsdatum = patient.DateOfBirth.ToString("dd.MM.yyyy"),
        Kvnr = patient.Kvnr ?? "", Krankenkasse = patient.InsuranceProvider ?? "",
        Versichertennummer = patient.InsuranceNumber ?? "",
        AnFachrichtung = fachrichtung, AnArzt = arzt, Diagnose = diagnose, Fragestellung = frage,
        ArztName = "Dr. med.", Datum = DateTime.Today.ToString("dd.MM.yyyy")
    });
    var pdf = doc.GeneratePdf();
    return Results.File(pdf, "application/pdf", $"Ueberweisung_{patient.LastName}.pdf");
}).RequireAuthorization("Klinisch");

app.MapGet("/api/formulare/au", async (Guid patientId, DateOnly von, DateOnly bis, bool erst, string diagnose, string? icd, IPatientService patientService) =>
{
    var patient = await patientService.GetByIdAsync(patientId);
    if (patient is null) return Results.NotFound();
    var doc = new MediPrax.Reporting.Formulare.AuDocument(new MediPrax.Reporting.Formulare.AuData
    {
        PatientName = patient.FullName, PatientGeburtsdatum = patient.DateOfBirth.ToString("dd.MM.yyyy"),
        Kvnr = patient.Kvnr ?? "", Krankenkasse = patient.InsuranceProvider ?? "",
        Versichertennummer = patient.InsuranceNumber ?? "",
        VonDatum = von, BisDatum = bis, Erstbescheinigung = erst, Diagnose = diagnose, IcdCode = icd,
        ArztName = "Dr. med.", Datum = DateTime.Today.ToString("dd.MM.yyyy")
    });
    var pdf = doc.GeneratePdf();
    return Results.File(pdf, "application/pdf", $"AU_{patient.LastName}.pdf");
}).RequireAuthorization("Klinisch");

// PTV form PDF endpoint
app.MapGet("/api/ptv/{formId:guid}/pdf", async (Guid formId, ITherapyCaseService therapyCaseService) =>
{
    var pdf = await therapyCaseService.GetPtvFormPdfAsync(formId);
    if (pdf is null) return Results.NotFound();
    return Results.File(pdf, "application/pdf", $"PTV-{formId:N}.pdf");
}).RequireAuthorization("Klinisch");

app.MapGet("/api/formulare/krankenhauseinweisung", async (Guid patientId, string krankenhaus, string fachabteilung, string diagnose, string? icd, string? befunde, bool notfall, IPatientService patientService) =>
{
    var patient = await patientService.GetByIdAsync(patientId);
    if (patient is null) return Results.NotFound();
    var doc = new MediPrax.Reporting.Formulare.KrankenhauseinweisungDocument(new MediPrax.Reporting.Formulare.KrankenhauseinweisungData
    {
        PatientName = patient.FullName, Geburtsdatum = patient.DateOfBirth.ToString("dd.MM.yyyy"),
        Kvnr = patient.Kvnr ?? "", Krankenkasse = patient.InsuranceProvider ?? "",
        Versichertennummer = patient.InsuranceNumber ?? "",
        Krankenhaus = krankenhaus, Fachabteilung = fachabteilung,
        Einweisungsdiagnose = diagnose, DiagnoseIcd = icd, Befunde = befunde, IsNotfall = notfall,
        ArztName = "Dr. med.", Datum = DateTime.Today.ToString("dd.MM.yyyy")
    });
    var pdf = doc.GeneratePdf();
    return Results.File(pdf, "application/pdf", $"Krankenhauseinweisung_{patient.LastName}.pdf");
}).RequireAuthorization("Klinisch");

app.MapGet("/api/formulare/heilmittelverordnung", async (Guid patientId, string heilmittel, string diagnose, string? icd, string? ziel, int sitzungen, string? frequenz, bool erst, bool hausbesuch, IPatientService patientService) =>
{
    var patient = await patientService.GetByIdAsync(patientId);
    if (patient is null) return Results.NotFound();
    var doc = new MediPrax.Reporting.Formulare.HeilmittelverordnungDocument(new MediPrax.Reporting.Formulare.HeilmittelverordnungData
    {
        PatientName = patient.FullName, Geburtsdatum = patient.DateOfBirth.ToString("dd.MM.yyyy"),
        Kvnr = patient.Kvnr ?? "", Krankenkasse = patient.InsuranceProvider ?? "",
        Versichertennummer = patient.InsuranceNumber ?? "",
        HeilmittelTyp = heilmittel, Diagnose = diagnose, DiagnoseIcd = icd,
        Therapieziel = ziel, AnzahlSitzungen = sitzungen, Frequenz = frequenz,
        IsErstverordnung = erst, IsHausbesuch = hausbesuch,
        ArztName = "Dr. med.", Datum = DateTime.Today.ToString("dd.MM.yyyy")
    });
    var pdf = doc.GeneratePdf();
    return Results.File(pdf, "application/pdf", $"Heilmittelverordnung_{patient.LastName}.pdf");
}).RequireAuthorization("Klinisch");

app.MapGet("/api/formulare/haeusliche-krankenpflege", async (Guid patientId, string diagnose, string? icd, string leistungen, string von, string bis, string? frequenz, bool psychiatrisch, IPatientService patientService) =>
{
    var patient = await patientService.GetByIdAsync(patientId);
    if (patient is null) return Results.NotFound();
    var doc = new MediPrax.Reporting.Formulare.HaeuslicheKrankenpflegeDocument(new MediPrax.Reporting.Formulare.HaeuslicheKrankenpflegeData
    {
        PatientName = patient.FullName, Geburtsdatum = patient.DateOfBirth.ToString("dd.MM.yyyy"),
        Kvnr = patient.Kvnr ?? "", Krankenkasse = patient.InsuranceProvider ?? "",
        Versichertennummer = patient.InsuranceNumber ?? "",
        Adresse = patient.Street != null ? $"{patient.Street}, {patient.PostalCode} {patient.City}" : null,
        Diagnose = diagnose, DiagnoseIcd = icd, Leistungen = leistungen,
        VonDatum = von, BisDatum = bis, Frequenz = frequenz, IsPsychiatrisch = psychiatrisch,
        ArztName = "Dr. med.", Datum = DateTime.Today.ToString("dd.MM.yyyy")
    });
    var pdf = doc.GeneratePdf();
    return Results.File(pdf, "application/pdf", $"HaeuslicheKrankenpflege_{patient.LastName}.pdf");
}).RequireAuthorization("Klinisch");

app.MapGet("/api/formulare/soziotherapie", async (Guid patientId, string diagnose, string? icd, string stoerungen, string ziele, int stunden, string? therapeut, bool erst, IPatientService patientService) =>
{
    var patient = await patientService.GetByIdAsync(patientId);
    if (patient is null) return Results.NotFound();
    var doc = new MediPrax.Reporting.Formulare.SoziotherapieDocument(new MediPrax.Reporting.Formulare.SoziotherapieData
    {
        PatientName = patient.FullName, Geburtsdatum = patient.DateOfBirth.ToString("dd.MM.yyyy"),
        Kvnr = patient.Kvnr ?? "", Krankenkasse = patient.InsuranceProvider ?? "",
        Versichertennummer = patient.InsuranceNumber ?? "",
        Diagnose = diagnose, DiagnoseIcd = icd,
        Faehigkeitsstoerungen = stoerungen, Therapieziele = ziele,
        VerordneteStunden = stunden, Soziotherapeut = therapeut, IsErstverordnung = erst,
        ArztName = "Dr. med.", Datum = DateTime.Today.ToString("dd.MM.yyyy")
    });
    var pdf = doc.GeneratePdf();
    return Results.File(pdf, "application/pdf", $"Soziotherapie_{patient.LastName}.pdf");
}).RequireAuthorization("Klinisch");

// M32: PsychKG PDF endpoints
app.MapGet("/api/formulare/psychkg-zeugnis", async (Guid patientId, string untersuchungsDatum, string? untersuchungsZeit, string psychopathBefunde, string selbstgefaehrdung, string fremdgefaehrdung, string diagnose, string empfehlung, string? dauer, IPatientService patientService) =>
{
    var patient = await patientService.GetByIdAsync(patientId);
    if (patient is null) return Results.NotFound();
    var doc = new MediPrax.Reporting.Formulare.PsychKGZeugnisDocument(new MediPrax.Reporting.Formulare.PsychKGZeugnisData
    {
        PatientName = patient.FullName,
        PatientGeburtsdatum = patient.DateOfBirth.ToString("dd.MM.yyyy"),
        PatientAdresse = patient.Street != null ? $"{patient.Street}, {patient.PostalCode} {patient.City}" : string.Empty,
        UntersuchungsDatum = untersuchungsDatum,
        UntersuchungsZeit = untersuchungsZeit ?? string.Empty,
        PsychopathologischeBefunde = psychopathBefunde,
        Selbstgefaehrdung = selbstgefaehrdung,
        Fremdgefaehrdung = fremdgefaehrdung,
        Diagnose = diagnose,
        Empfehlung = empfehlung,
        VorgeschlageneDauer = dauer ?? string.Empty,
        ArztName = "Dr. med.",
        Datum = DateTime.Today.ToString("dd.MM.yyyy")
    });
    var pdf = doc.GeneratePdf();
    return Results.File(pdf, "application/pdf", $"PsychKGZeugnis_{patient.LastName}.pdf");
}).RequireAuthorization("Arzt");

app.MapGet("/api/formulare/betreuungsanregung", async (Guid patientId, string diagnose, string begruendung, string? betreuer, string? aufgaben, string? hinweise, IPatientService patientService) =>
{
    var patient = await patientService.GetByIdAsync(patientId);
    if (patient is null) return Results.NotFound();
    var doc = new MediPrax.Reporting.Formulare.BetreuungsanregungDocument(new MediPrax.Reporting.Formulare.BetreuungsanregungData
    {
        PatientName = patient.FullName,
        PatientGeburtsdatum = patient.DateOfBirth.ToString("dd.MM.yyyy"),
        PatientAdresse = patient.Street != null ? $"{patient.Street}, {patient.PostalCode} {patient.City}" : string.Empty,
        Diagnose = diagnose,
        BegruendungBetreuungsbedarf = begruendung,
        VorgeschlagenerBetreuer = betreuer ?? string.Empty,
        BetreuungsAufgaben = aufgaben ?? string.Empty,
        ZusaetzlicheHinweise = hinweise ?? string.Empty,
        ArztName = "Dr. med.",
        Datum = DateTime.Today.ToString("dd.MM.yyyy")
    });
    var pdf = doc.GeneratePdf();
    return Results.File(pdf, "application/pdf", $"Betreuungsanregung_{patient.LastName}.pdf");
}).RequireAuthorization("Arzt");

// KVDT export endpoint
app.MapGet("/api/kvdt-export/{quarter}", async (string quarter, IKvdtExportService kvdtService, IAuditService auditService) =>
{
    var result = await kvdtService.ExportQuarterAsync(quarter);
    if (!result.Success) return Results.BadRequest(result.ErrorMessage);
    await auditService.LogAsync(AuditAction.Export, "BillingItem", details: $"KVDT-Export {quarter}: {result.ItemCount} Positionen, {result.PatientCount} Patienten");
    return Results.File(result.Content, "text/plain", result.FileName);
}).RequireAuthorization("Admin");

// Auto-migrate and seed
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<MediPraxDbContext>();
    db.Database.EnsureCreated();

    // Hash placeholder passwords
    var usersToFix = db.Users.Where(u => u.PasswordHash == "placeholder").ToList();
    foreach (var u in usersToFix)
        u.PasswordHash = BCrypt.Net.BCrypt.HashPassword("mediprax2026");

    // Ensure an Admin user exists
    if (!db.Users.Any(u => u.Role == MediPrax.Core.Enums.UserRole.Admin))
    {
        db.Users.Add(new MediPrax.Core.Entities.User
        {
            FirstName = "System",
            LastName = "Admin",
            Email = "admin@neuropsych-bremen.de",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("mediprax2026"),
            Role = MediPrax.Core.Enums.UserRole.Admin
        });
    }

    db.SaveChanges();

    // Seed demo data
    MediPrax.Server.Services.DemoSeedService.Seed(db);
}

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

public record LoginRequest(string Email, string Password);

// Expose Program class for WebApplicationFactory in E2E tests
public partial class Program;
