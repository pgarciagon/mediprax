using MediPrax.Application.Interfaces;
using MediPrax.Application.Services;
using MediPrax.Core.Interfaces;
using MediPrax.Infrastructure.Persistence;
using MediPrax.Server.Components;
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
app.UseAntiforgery();

// PDF endpoint
app.MapGet("/dokumente/{id:guid}/pdf", async (Guid id, IArztbriefService arztbriefService) =>
{
    var pdf = await arztbriefService.GetPdfAsync(id);
    if (pdf is null) return Results.NotFound();
    return Results.File(pdf, "application/pdf", $"Arztbrief-{id:N}.pdf");
});

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
