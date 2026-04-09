using System.Text.Json;
using MediPrax.Core.Entities;
using MediPrax.Core.ValueObjects;
using MediPrax.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MediPrax.IntegrationTests;

public class TestDbContextFactory : IDisposable
{
    private readonly SqliteConnection _connection;
    public MediPraxDbContext Context { get; }

    public TestDbContextFactory()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<MediPraxDbContext>()
            .UseSqlite(_connection)
            .Options;

        Context = new TestMediPraxDbContext(options);
        Context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        Context.Dispose();
        _connection.Dispose();
    }
}

/// <summary>Override to remove PostgreSQL-specific defaults for SQLite compatibility.</summary>
public class TestMediPraxDbContext(DbContextOptions<MediPraxDbContext> options) : MediPraxDbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 1. Remove gen_random_uuid() and identity defaults — SQLite doesn't support them
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var idProp = entityType.FindProperty("Id");
            if (idProp is not null)
            {
                idProp.SetDefaultValueSql(null);
                if (idProp.ClrType == typeof(long))
                    idProp.ValueGenerated = Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.OnAdd;
            }
        }

        // 2. Remove filtered indexes — SQLite doesn't support them
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var index in entityType.GetIndexes().ToList())
            {
                if (index.GetFilter() is not null)
                    index.SetFilter(null);
            }
        }

        // 3. Convert ALL jsonb columns to TEXT — SQLite doesn't support jsonb
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.GetColumnType() == "jsonb")
                    property.SetColumnType("TEXT");
            }
        }

        // 4. Explicit JSON value converters for every JSONB property
        //    (SQLite needs these to serialize/deserialize complex types)

        // Encounter.Icd10Codes: List<string>
        modelBuilder.Entity<Encounter>()
            .Property(e => e.Icd10Codes)
            .HasConversion(JsonListStringConverter())
            .HasColumnType("TEXT");

        // TherapyCase.Diagnoses: List<string>
        modelBuilder.Entity<TherapyCase>()
            .Property(e => e.Diagnoses)
            .HasConversion(JsonListStringConverter())
            .HasColumnType("TEXT");

        // PsychopathologicalFinding.Findings: List<SymptomFinding>
        modelBuilder.Entity<PsychopathologicalFinding>()
            .Property(f => f.Findings)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<SymptomFinding>>(v, (JsonSerializerOptions?)null) ?? new())
            .HasColumnType("TEXT");

        // PsychometricTest.Responses: List<TestResponse>
        modelBuilder.Entity<PsychometricTest>()
            .Property(e => e.Responses)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<TestResponse>>(v, (JsonSerializerOptions?)null) ?? new())
            .HasColumnType("TEXT");

        // LabResult.Values: List<LabValue>
        modelBuilder.Entity<LabResult>()
            .Property(e => e.Values)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<LabValue>>(v, (JsonSerializerOptions?)null) ?? new())
            .HasColumnType("TEXT");

        // PtvForm.FormData: Dictionary<string, string>
        modelBuilder.Entity<PtvForm>()
            .Property(e => e.FormData)
            .HasConversion(JsonDictStringConverter())
            .HasColumnType("TEXT");

        // HeadacheDiary.Triggers: List<string>?
        modelBuilder.Entity<HeadacheDiary>()
            .Property(e => e.Triggers)
            .HasConversion(JsonNullableListStringConverter())
            .HasColumnType("TEXT");

        // ParkinsonDocumentation.NonMotorSymptoms: List<string>?
        modelBuilder.Entity<ParkinsonDocumentation>()
            .Property(e => e.NonMotorSymptoms)
            .HasConversion(JsonNullableListStringConverter())
            .HasColumnType("TEXT");

        // SuicidalityAssessment JSONB fields
        modelBuilder.Entity<SuicidalityAssessment>()
            .Property(e => e.RiskFactors)
            .HasConversion(JsonListStringConverter())
            .HasColumnType("TEXT");
        modelBuilder.Entity<SuicidalityAssessment>()
            .Property(e => e.ProtectiveFactors)
            .HasConversion(JsonListStringConverter())
            .HasColumnType("TEXT");
        modelBuilder.Entity<SuicidalityAssessment>()
            .Property(e => e.ActionsTaken)
            .HasConversion(JsonListStringConverter())
            .HasColumnType("TEXT");

        // WaitlistEntry.PreferredDays: List<DayOfWeek>?
        modelBuilder.Entity<WaitlistEntry>()
            .Property(e => e.PreferredDays)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<DayOfWeek>>(v, (JsonSerializerOptions?)null))
            .HasColumnType("TEXT");

        // DmpDocumentation.FormData: Dictionary<string, string>
        modelBuilder.Entity<DmpDocumentation>()
            .Property(e => e.FormData)
            .HasConversion(JsonDictStringConverter())
            .HasColumnType("TEXT");

        // PrivateInvoice.Items: List<InvoiceItem>
        modelBuilder.Entity<PrivateInvoice>()
            .Property(e => e.Items)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<InvoiceItem>>(v, (JsonSerializerOptions?)null) ?? new())
            .HasColumnType("TEXT");
    }

    // --- Reusable converter helpers ---

    private static ValueConverter<List<string>, string> JsonListStringConverter() => new(
        v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
        v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new());

    private static ValueConverter<List<string>?, string> JsonNullableListStringConverter() => new(
        v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
        v => JsonSerializer.Deserialize<List<string>?>(v, (JsonSerializerOptions?)null));

    private static ValueConverter<Dictionary<string, string>, string> JsonDictStringConverter() => new(
        v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
        v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions?)null) ?? new());
}
