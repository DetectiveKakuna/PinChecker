using Microsoft.Extensions.Options;
using Microsoft.Playwright;
using PinChecker.Models.Configurations;

namespace PinChecker.Services.Implementations;

public class FantasyPinGardenService(IOptions<PlaywrightServiceConfig> config) : BasePlaywrightService(config)
{
    private const string PinDivClass = "a.listing-link";
    private readonly PageWaitForSelectorOptions SelectorOptions = new() { State = WaitForSelectorState.Visible };

    protected async override Task GetInventoryAsync_Implementation()
    {
        bool pageHasInventory = true;

        while (pageHasInventory)
        {

        }
        await _page.WaitForSelectorAsync(PinDivClass, SelectorOptions);

        var htmlContent = await _page.ContentAsync();
    }
}