using DotNet.Testcontainers.Configurations;
using Testcontainers.PostgreSql;

namespace MediPrax.IntegrationTests;

/// <summary>
/// Shared PostgreSQL connection for all integration tests.
/// Uses Testcontainers when Docker is available (CI), otherwise
/// connects to the local development PostgreSQL.
/// </summary>
public class PostgresFixture : IAsyncLifetime
{
    private PostgreSqlContainer? _container;

    public string ConnectionString { get; private set; } = default!;

    public async Task InitializeAsync()
    {
        if (IsDockerAvailable())
        {
            _container = new PostgreSqlBuilder()
                .WithImage("postgres:16-alpine")
                .Build();
            await _container.StartAsync();
            ConnectionString = _container.GetConnectionString();
        }
        else
        {
            // Fall back to the local development PostgreSQL
            ConnectionString = "Host=localhost;Port=5432;Username=mediprax;Password=mediprax_dev;Database=postgres";
        }
    }

    public async Task DisposeAsync()
    {
        if (_container is not null)
            await _container.DisposeAsync().AsTask();
    }

    private static bool IsDockerAvailable()
    {
        try
        {
            return TestcontainersSettings.OS.DockerEndpointAuthConfig is not null;
        }
        catch
        {
            return false;
        }
    }
}

[CollectionDefinition("Postgres")]
public class PostgresCollection : ICollectionFixture<PostgresFixture>;
