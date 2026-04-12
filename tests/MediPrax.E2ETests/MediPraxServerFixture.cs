using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace MediPrax.E2ETests;

/// <summary>
/// Launches the MediPrax server as a real process on a random port for E2E/Playwright testing.
/// The local development PostgreSQL must be running (docker compose up -d).
/// </summary>
public class MediPraxServerFixture : IAsyncLifetime
{
    private Process? _serverProcess;

    /// <summary>Base URL of the running server (e.g. http://localhost:5XXX).</summary>
    public string BaseUrl { get; private set; } = default!;

    public async Task InitializeAsync()
    {
        var port = GetFreePort();
        BaseUrl = $"http://localhost:{port}";

        // Find the server project directory
        var solutionDir = FindSolutionDirectory();
        var serverProject = Path.Combine(solutionDir, "src", "MediPrax.Server");

        _serverProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"run --project \"{serverProject}\" --urls \"{BaseUrl}\" --no-build",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                Environment =
                {
                    ["ASPNETCORE_ENVIRONMENT"] = "Development",
                    ["DOTNET_NOLOGO"] = "1"
                }
            }
        };

        _serverProcess.Start();

        // Discard output to prevent buffer blocking
        _serverProcess.BeginOutputReadLine();
        _serverProcess.BeginErrorReadLine();

        // Wait for server to be ready
        using var client = new HttpClient();
        for (var i = 0; i < 60; i++)
        {
            if (_serverProcess.HasExited)
                throw new InvalidOperationException(
                    $"Server process exited with code {_serverProcess.ExitCode}. " +
                    "Make sure PostgreSQL is running (docker compose up -d) and the project builds.");

            try
            {
                var response = await client.GetAsync($"{BaseUrl}/health");
                if (response.IsSuccessStatusCode)
                    return;
            }
            catch { }

            await Task.Delay(500);
        }

        throw new InvalidOperationException(
            $"Server did not start within 30 seconds at {BaseUrl}. " +
            "Make sure PostgreSQL is running (docker compose up -d).");
    }

    public Task DisposeAsync()
    {
        if (_serverProcess is { HasExited: false })
        {
            try
            {
                _serverProcess.Kill(entireProcessTree: true);
                _serverProcess.WaitForExit(5000);
            }
            catch { }
        }
        _serverProcess?.Dispose();
        return Task.CompletedTask;
    }

    private static int GetFreePort()
    {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    private static string FindSolutionDirectory()
    {
        var dir = Directory.GetCurrentDirectory();
        while (dir is not null)
        {
            if (Directory.GetFiles(dir, "*.slnx").Length > 0 ||
                Directory.GetFiles(dir, "*.sln").Length > 0)
                return dir;
            dir = Directory.GetParent(dir)?.FullName;
        }
        throw new InvalidOperationException("Could not find solution directory.");
    }
}

[CollectionDefinition("Playwright")]
public class PlaywrightCollection : ICollectionFixture<PlaywrightFixture>, ICollectionFixture<MediPraxServerFixture>;
