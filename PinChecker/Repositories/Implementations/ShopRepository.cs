using PinChecker.Models;
using PinChecker.Services;

namespace PinChecker.Repositories.Implementations;

public class ShopRepository(IEnumerable<IPlaywrightService> playwrightServices) : IShopRepository
{
    private readonly List<IPlaywrightService> _playwrightServices = [.. playwrightServices];

    public async Task GetShopChanges()
    {
        List<Shop> shops = [.. (await Task.WhenAll(_playwrightServices.Select(service => service.GetShopStatusAsync())))];
    }
}