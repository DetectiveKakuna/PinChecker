using PinChecker.Databases;
using PinChecker.Models;
using PinChecker.Services;

namespace PinChecker.Repositories.Implementations;

/// <summary>
/// Implementation of the shop repository that manages shop inventory changes using Cosmos DB and Playwright services.
/// </summary>
public class ShopRepository(ICosmosDb cosmosDb, IEnumerable<IPlaywrightService> playwrightServices) : IShopRepository
{
    private readonly ICosmosDb _cosmosDb = cosmosDb;
    private readonly List<IPlaywrightService> _playwrightServices = [.. playwrightServices];

    public async Task GetShopChanges()
    {
        List<Shop> shops = [.. (await Task.WhenAll(_playwrightServices.Select(service => service.GetShopStatusAsync())))];
        List<Shop> existingRecords = [.. (await _cosmosDb.GetShopRecordsAsync())];
    }
}