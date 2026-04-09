using MediPrax.Application.DTOs;
using MediPrax.Application.Services;
using Microsoft.Extensions.Options;

namespace MediPrax.Server.Services;

/// <summary>
/// Background service that monitors configured directories for incoming GDT result files.
/// </summary>
public class GdtFileWatcherService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<GdtFileWatcherService> _logger;
    private readonly GdtDevicesOptions _options;
    private readonly List<FileSystemWatcher> _watchers = [];

    public GdtFileWatcherService(
        IServiceProvider serviceProvider,
        ILogger<GdtFileWatcherService> logger,
        IOptions<GdtDevicesOptions> options)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _options = options.Value;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        foreach (var device in _options.Devices)
        {
            if (string.IsNullOrEmpty(device.ImportDirectory) || !Directory.Exists(device.ImportDirectory))
            {
                _logger.LogWarning("GDT import directory not found for {Device}: {Path}", device.DeviceName, device.ImportDirectory);
                continue;
            }

            var watcher = new FileSystemWatcher(device.ImportDirectory)
            {
                Filter = "*.gdt",
                EnableRaisingEvents = true
            };

            watcher.Created += (_, e) => _ = ProcessFileAsync(e.FullPath, device);
            _watchers.Add(watcher);

            _logger.LogInformation("GDT watcher started for {Device} at {Path}", device.DeviceName, device.ImportDirectory);
        }

        return Task.CompletedTask;
    }

    private async Task ProcessFileAsync(string filePath, GdtDeviceConfig device)
    {
        try
        {
            // Small delay to ensure file is fully written
            await Task.Delay(500);

            var encoding = System.Text.Encoding.GetEncoding(device.Encoding);
            var content = await File.ReadAllTextAsync(filePath, encoding);

            if (!GdtParser.IsValidGdt(content))
            {
                _logger.LogWarning("Invalid GDT file: {Path}", filePath);
                return;
            }

            using var scope = _serviceProvider.CreateScope();
            var gdtService = scope.ServiceProvider.GetRequiredService<IGdtService>();
            var result = await gdtService.ImportResultAsync(content, device.DeviceType);

            _logger.LogInformation("GDT import successful: {Device} — {Lines} result lines",
                device.DeviceName, result.ResultLines.Count);

            // Move processed file
            var processedDir = Path.Combine(device.ImportDirectory, "processed");
            Directory.CreateDirectory(processedDir);
            var destPath = Path.Combine(processedDir, $"{DateTime.Now:yyyyMMdd_HHmmss}_{Path.GetFileName(filePath)}");
            File.Move(filePath, destPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing GDT file: {Path}", filePath);
        }
    }

    public override void Dispose()
    {
        foreach (var watcher in _watchers)
            watcher.Dispose();
        base.Dispose();
    }
}
