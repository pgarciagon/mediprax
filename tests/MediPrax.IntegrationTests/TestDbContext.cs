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

        // Remove gen_random_uuid() default — SQLite doesn't support it
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var idProp = entityType.FindProperty("Id");
            if (idProp is not null && idProp.ClrType == typeof(Guid))
            {
                idProp.SetDefaultValueSql(null);
            }

            // Remove UseIdentityAlwaysColumn for AuditLog.Id (long)
            if (idProp is not null && idProp.ClrType == typeof(long))
            {
                idProp.SetDefaultValueSql(null);
                idProp.ValueGenerated = Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.OnAdd;
            }
        }

        // SQLite doesn't support filtered indexes — remove them
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var index in entityType.GetIndexes().ToList())
            {
                if (index.GetFilter() is not null)
                    index.SetFilter(null);
            }
        }

        // SQLite doesn't support jsonb — override to TEXT
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.GetColumnType() == "jsonb")
                {
                    property.SetColumnType(null);
                }
            }
        }

        // JSON value converters for JSONB columns in SQLite
        modelBuilder.Entity<PsychopathologicalFinding>()
            .Property(f => f.Findings)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<SymptomFinding>>(v, (JsonSerializerOptions?)null) ?? new List<SymptomFinding>());

        modelBuilder.Entity<PsychometricTest>()
            .Property(e => e.Responses)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<MediPrax.Core.ValueObjects.TestResponse>>(v, (JsonSerializerOptions?)null) ?? new())
            .HasColumnType("TEXT");
    }
}
