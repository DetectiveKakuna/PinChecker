// Ignore Spelling: Upsert

using PinChecker.Models;
using PinChecker.Services;

namespace PinChecker.Repositories.Implementations;

/// <summary>
/// Implementation of the shop repository that manages shops.
/// </summary>
public class ShopRepository(IJsonFileService jsonFileService, IEnumerable<IPlaywrightService> playwrightServices) : IShopRepository
{
    private readonly IJsonFileService _jsonFileService = jsonFileService;
    private readonly List<IPlaywrightService> _playwrightServices = [.. playwrightServices];
    
    private List<Shop> _shops = [];

    public async Task<IEnumerable<ShopChanges>> GetShopChangesAsync()
    {
        try
        {
            _shops = await GetShopsAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching shop data: {ex.Message}");
            return [];
        }

        List<Shop> existingRecords = [];

        try
        {
            existingRecords = [.. (await _jsonFileService.GetShopRecordsAsync())];

            // If the file happens to be new, don't return changes for the email
            if (existingRecords.Count == 0)
                return [];
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching existing shop records: {ex.Message}");
            return [];
        }

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
                    ChangedItems = [],
                });
                continue;
            }

            // Compare items for existing shops
            var oldShopState = currentShopRecord.Items ?? [];
            var newShopState = shop.Items ?? [];

            var addedItems = newShopState.Where(item => !oldShopState.Any(ei => ei.Name == item.Name)).ToList();

            // Changes currently checking for:
            // 1. Status changes other than an item selling out
            var changedItems = newShopState
                .Where(newState => newState.Status != Models.Enums.ShopStatus.SoldOut && oldShopState.Any(oldState => oldState.Name == newState.Name && oldState.Status != newState.Status))
                .Select(newState => (oldState: oldShopState.First(oldState => oldState.Name == newState.Name), newState))
                .ToList();

            if (addedItems.Count > 0 || changedItems.Count > 0)
            {
                changes.Add(new ShopChanges
                {
                    ShopName = shop.Name,
                    AddedItems = addedItems,
                    ChangedItems = changedItems,
                });
            }
        }

        return changes;
    }

    public async Task UpdateShopRecordsAsync()
    {
        if (_shops.Count != 0 && _shops.All(shop => shop.Items.Count > 0))
            await _jsonFileService.SetShopRecordsAsync(_shops);
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