using PinChecker.Models;

namespace PinChecker.Repositories;

/// <summary>
/// Defines the interface for accessing and managing shop inventory changes.
/// </summary>
public interface IShopRepository
{
    /// <summary>
    /// Retrieves a collection of changes made to shops' inventories asynchronously.
    /// </summary>
    /// <returns>A list of shop changes.</returns>
    Task<IEnumerable<ShopChanges>> GetShopChangesAsync();

    /// <summary>
    /// Updates the shop records in the database with current shop data.
    /// </summary>
    Task UpdateShopRecordsAsync();
}