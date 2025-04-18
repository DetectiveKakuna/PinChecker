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
            var currentShopRecord = existingRecords.FirstOrDefault(s => s.Name == shop.Name);

            // New shop
            if (currentShopRecord == null)
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
            var oldShopState = currentShopRecord.Items ?? [];
            var newShopState = shop.Items ?? [];

            var addedItems = newShopState.Where(item => !oldShopState.Any(ei => ei.Name == item.Name)).ToList();

            var changedStatus = newShopState
                .Where(newState => newState.Status != Models.Enums.ShopStatus.SoldOut && oldShopState.Any(oldState => oldState.Name == newState.Name && oldState.Status != newState.Status))
                .Select(newState => (oldState: oldShopState.First(oldState => oldState.Name == newState.Name), newState: newState))
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