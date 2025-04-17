// Ignore Spelling: Upsert

using PinChecker.Databases;
using PinChecker.Models;
using PinChecker.Services;
using static System.Formats.Asn1.AsnWriter;

namespace PinChecker.Repositories.Implementations;

/// <summary>
/// Implementation of the shop repository that manages shops.
/// </summary>
public class ShopRepository(ICosmosDb cosmosDb, IEnumerable<IPlaywrightService> playwrightServices) : IShopRepository
{
    private readonly ICosmosDb _cosmosDb = cosmosDb;
    private readonly List<IPlaywrightService> _playwrightServices = [.. playwrightServices];
    
    private List<Shop> _shops = [];

    public async Task<IEnumerable<ShopChanges>> GetShopChangesAsync()
    {
        _shops = await GetShopsAsync();
        List<Shop> existingRecords = [.. (await _cosmosDb.GetShopRecordsAsync())];

        // Find changes between shops and existing records
        List<ShopChanges> changes = [];

        foreach (var shop in _shops)
        {
            var existingShop = existingRecords.FirstOrDefault(s => s.Name == shop.Name);

            // New shop
            if (existingShop == null)
            {
                changes.Add(new ShopChanges
                {
                    ShopName = shop.Name,
                    AddedItems = shop.Items ?? [],
                    ChangedStatus = [],
                });
                continue;
            }

            // Compare items for existing shops
            var existingItems = existingShop.Items ?? [];
            var currentItems = shop.Items ?? [];

            var addedItems = currentItems.Where(item => !existingItems.Any(ei => ei.Name == item.Name)).ToList();

            var changedStatus = currentItems
                .Where(newItem => existingItems.Any(oldItem => oldItem.Status != newItem.Status))
                .Select(newItem => (
                    oldState: existingItems.First(oldItem => oldItem.Name == newItem.Name),
                    newState: newItem))
                .ToList();

            if (addedItems.Count > 0 || changedStatus.Count > 0)
            {
                changes.Add(new ShopChanges
                {
                    ShopName = shop.Name,
                    AddedItems = addedItems,
                    ChangedStatus = changedStatus,
                });
            }
        }

        return changes;
    }

    public async Task UpdateShopRecordsAsync()
    {
        if (_shops.Count == 0)
            _shops = await GetShopsAsync();
        else
            await _cosmosDb.UpsertShopRecordsAsync(_shops);
            
    }

    #region Private Methods
    /// <summary>
    /// Aggregates shop data from all registered Playwright services.
    /// </summary>
    /// <returns>A list of shops with their current inventory status.</returns>
    private async Task<List<Shop>> GetShopsAsync()
    {
        return [.. (await Task.WhenAll(_playwrightServices.Select(service => service.GetShopStatusAsync())))];
    }
    #endregion
}