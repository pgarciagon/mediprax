using Microsoft.Playwright;

namespace MediPrax.E2ETests;

/// <summary>
/// Base class for E2E tests providing authenticated browser context.
/// Each test gets a fresh browser context (isolated cookies/storage).
/// </summary>
public abstract class E2ETestBase : IAsyncLifetime
{
    protected readonly PlaywrightFixture Playwright;
    protected readonly MediPraxServerFixture Server;

    protected IBrowserContext Context { get; private set; } = default!;
    protected IPage Page { get; private set; } = default!;

    protected E2ETestBase(PlaywrightFixture playwright, MediPraxServerFixture server)
    {
        Playwright = playwright;
        Server = server;
    }

    public async Task InitializeAsync()
    {
        Context = await Playwright.Browser.NewContextAsync();
        Page = await Context.NewPageAsync();
        await LoginAsync();
    }

    public async Task DisposeAsync()
    {
        await Context.DisposeAsync();
    }

    /// <summary>
    /// Log in as the demo doctor user via the login form.
    /// </summary>
    protected async Task LoginAsync(string email = "meier@neuropsych-bremen.de", string password = "mediprax2026")
    {
        await Page.GotoAsync($"{Server.BaseUrl}/login");
        await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        // Blazor SSR EditForm with FormName="loginForm" generates inputs with name "Form.Email" etc.
        // Try multiple selectors to handle different Blazor rendering modes
        var emailInput = Page.Locator("input[name='Form.Email'], input[placeholder='email@praxis.de']").First;
        await emailInput.WaitForAsync(new() { Timeout = 10_000 });
        await emailInput.FillAsync(email);

        var passwordInput = Page.Locator("input[name='Form.Password'], input[placeholder='Passwort']").First;
        await passwordInput.FillAsync(password);

        // Submit
        await Page.ClickAsync("button[type='submit']");

        // Wait for redirect to dashboard (force reload navigates away from /login)
        await Page.WaitForURLAsync(url => !url.Contains("/login"), new() { Timeout = 15_000 });
    }

    /// <summary>
    /// Navigate to a page and wait for it to be ready.
    /// </summary>
    protected async Task NavigateToAsync(string path)
    {
        await Page.GotoAsync($"{Server.BaseUrl}{path}");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }
}
