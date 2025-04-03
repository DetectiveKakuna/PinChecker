using Microsoft.Extensions.Options;
using Microsoft.Playwright;
using PinChecker.Models.Configurations;

namespace PinChecker.Services.Implementations;

public abstract class BasePlaywrightService(IOptions<PlaywrightServiceConfig> config) : IPlaywrightService
{
    #pragma warning disable CS8618
    private IPlaywright _playwright;
    private IBrowser _browser;
    protected IPage _page;
    #pragma warning restore CS8618
    private readonly PlaywrightServiceConfig _config = config.Value;

    public async Task GetInventoryAsync()
    {
        await GoToPage();
        await GetInventoryAsync_Implementation();
    }
    protected abstract Task GetInventoryAsync_Implementation();

    /// <summary>
    /// Initializes Playwright, launches a browser instance, creates a new page,
    /// and navigates to the configured URL with specified timeout and load state.
    /// If the page already exists, it reloads the current page instead of creating a new instance.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected async Task GoToPage()
    {
        if (_page == null)
        {
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = _config.Headless });
            _page = await _browser.NewPageAsync();
            await _page.GotoAsync(_config.Url, new PageGotoOptions
            {
                WaitUntil = WaitUntilState.Load,
                Timeout = _config.Timeout,
            });
        }
        else
            await _page.ReloadAsync();
    }
}