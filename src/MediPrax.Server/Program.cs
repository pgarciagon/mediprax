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

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuditInterceptor>();
builder.Services.AddDbContext<MediPraxDbContext>((sp, options) =>
    options
        .UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
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
builder.Services.AddScoped<IArztbriefService, MediPrax.Server.Services.ArztbriefService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IBillingService, BillingService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IKvdtExportService, KvdtExportService>();
builder.Services.AddScoped<IImportService, ImportService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IAuthService, MediPrax.Server.Services.AuthService>();

// Telematik — Mock services (replace with real implementations when TI access is available)
builder.Services.AddSingleton<MediPrax.Application.Interfaces.Telematik.IEgkService, MediPrax.Server.Services.Telematik.MockEgkService>();
builder.Services.AddSingleton<MediPrax.Application.Interfaces.Telematik.IERezeptService, MediPrax.Server.Services.Telematik.MockERezeptService>();
builder.Services.AddSingleton<MediPrax.Application.Interfaces.Telematik.IKimService, MediPrax.Server.Services.Telematik.MockKimService>();
builder.Services.AddSingleton<MediPrax.Application.Interfaces.Telematik.IEpaService, MediPrax.Server.Services.Telematik.MockEpaService>();
builder.Services.AddScoped<IAuditService, AuditService>();

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

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

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
});

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

// KVDT export endpoint
app.MapGet("/api/kvdt-export/{quarter}", async (string quarter, IKvdtExportService kvdtService, IAuditService auditService) =>
{
    var result = await kvdtService.ExportQuarterAsync(quarter);
    if (!result.Success) return Results.BadRequest(result.ErrorMessage);
    await auditService.LogAsync(AuditAction.Export, "BillingItem", details: $"KVDT-Export {quarter}: {result.ItemCount} Positionen, {result.PatientCount} Patienten");
    return Results.File(result.Content, "text/plain", result.FileName);
}).RequireAuthorization("Admin");

// Seed: ensure admin user and hash placeholder passwords
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<MediPraxDbContext>();

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
}

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

public record LoginRequest(string Email, string Password);
