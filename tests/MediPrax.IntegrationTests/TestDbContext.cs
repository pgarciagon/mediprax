using MediPrax.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace MediPrax.IntegrationTests;

/// <summary>
/// Creates a unique PostgreSQL database per test class using
/// the shared PostgreSQL instance (Testcontainers or local).
/// No SQLite hacks — tests run against real PostgreSQL.
/// </summary>
public class TestDbContextFactory : IDisposable
{
    public MediPraxDbContext Context { get; }
    private readonly string _baseConnectionString;
    private readonly string _dbName;

    public TestDbContextFactory(string baseConnectionString)
    {
        _baseConnectionString = baseConnectionString;
        _dbName = $"test_{Guid.NewGuid():N}";

        // Create the database (connect to default 'postgres' db)
        using (var adminConn = CreateAdminConnection())
        {
            adminConn.Open();
            using var cmd = adminConn.CreateCommand();
            cmd.CommandText = $"CREATE DATABASE \"{_dbName}\"";
            cmd.ExecuteNonQuery();
        }

        var connStr = BuildConnectionString(_dbName);
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connStr);
        dataSourceBuilder.EnableDynamicJson();
        var dataSource = dataSourceBuilder.Build();

        var options = new DbContextOptionsBuilder<MediPraxDbContext>()
            .UseNpgsql(dataSource)
            .Options;

        Context = new MediPraxDbContext(options);
        Context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        Context.Dispose();

        // Clear connection pools so PostgreSQL allows dropping the database
        NpgsqlConnection.ClearAllPools();

        // Drop the test database
        try
        {
            using var adminConn = CreateAdminConnection();
            adminConn.Open();
            using var cmd = adminConn.CreateCommand();
            cmd.CommandText = $"DROP DATABASE IF EXISTS \"{_dbName}\" WITH (FORCE)";
            cmd.ExecuteNonQuery();
        }
        catch
        {
            // Best effort cleanup
        }
    }

    private NpgsqlConnection CreateAdminConnection()
    {
        var builder = new NpgsqlConnectionStringBuilder(_baseConnectionString)
        {
            Database = "postgres",
            Pooling = false
        };
        return new NpgsqlConnection(builder.ToString());
    }

    private string BuildConnectionString(string database)
    {
        var builder = new NpgsqlConnectionStringBuilder(_baseConnectionString)
        {
            Database = database,
            MaxPoolSize = 5
        };
        return builder.ToString();
    }
}
