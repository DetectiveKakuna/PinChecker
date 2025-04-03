using PinChecker.Services;

namespace PinChecker.Repositories.Implementations;

public class ShopRepository(IEnumerable<IPlaywrightService> playwrightServices) : IShopRepository
{
    private readonly List<IPlaywrightService> _playwrightServices = [.. playwrightServices];

    public async Task Test()
    {
        foreach (var playwrightService in _playwrightServices)
        {
            await playwrightService.GetInventoryAsync();
        }
    }
}