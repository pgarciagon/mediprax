using System.Security.Claims;
using MediPrax.Application.Interfaces;
using MediPrax.Application.Services;
using MediPrax.Core.Interfaces;
using MediPrax.Infrastructure.Persistence;
using MediPrax.Server.Components;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<MediPraxDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

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
builder.Services.AddScoped<IAuthService, MediPrax.Server.Services.AuthService>();

// Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.ExpireTimeSpan = TimeSpan.FromHours(12);
        options.SlidingExpiration = true;
    });
builder.Services.AddAuthorization();
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
app.MapPost("/api/login", async (HttpContext httpContext, IAuthService authService, LoginRequest request) =>
{
    var user = await authService.ValidateCredentialsAsync(request.Email, request.Password);
    if (user is null)
        return Results.Unauthorized();

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
    return Results.Ok();
});

// Logout endpoint
app.MapPost("/api/logout", async (HttpContext httpContext) =>
{
    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Ok();
});

// PDF endpoint
app.MapGet("/dokumente/{id:guid}/pdf", async (Guid id, IArztbriefService arztbriefService) =>
{
    var pdf = await arztbriefService.GetPdfAsync(id);
    if (pdf is null) return Results.NotFound();
    return Results.File(pdf, "application/pdf", $"Arztbrief-{id:N}.pdf");
});

// Seed: hash placeholder passwords in development
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<MediPraxDbContext>();
    var usersToFix = db.Users.Where(u => u.PasswordHash == "placeholder").ToList();
    foreach (var u in usersToFix)
    {
        u.PasswordHash = BCrypt.Net.BCrypt.HashPassword("mediprax2026");
    }
    if (usersToFix.Count > 0) db.SaveChanges();
}

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

public record LoginRequest(string Email, string Password);
