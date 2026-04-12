using Microsoft.Playwright;

namespace MediPrax.E2ETests;

/// <summary>
/// Manages a single Playwright browser instance shared across all E2E tests.
/// Chromium must be installed: node tests/MediPrax.E2ETests/bin/Debug/net10.0/.playwright/package/cli.js install chromium
/// </summary>
public class PlaywrightFixture : IAsyncLifetime
{
    private IPlaywright? _playwright;

    public IBrowser Browser { get; private set; } = default!;

    public async Task InitializeAsync()
    {
        _playwright = await Playwright.CreateAsync();
        Browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });
    }

    public async Task DisposeAsync()
    {
        if (Browser is not null)
            await Browser.DisposeAsync();
        _playwright?.Dispose();
    }
}
