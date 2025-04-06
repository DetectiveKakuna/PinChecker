using PinChecker.Models;

namespace PinChecker.Databases;

/// <summary>
/// Interface for Azure Cosmos DB operations related to shop data.
/// </summary>
public interface ICosmosDb
{
    /// <summary>
    /// Retrieves all shop records from the Cosmos DB container.
    /// </summary>
    /// <returns>A collection of Shop objects retrieved from the database.</returns>
    Task<IEnumerable<Shop>> GetShopRecordsAsync();

    /// <summary>
    /// Creates or updates a list of shop records in the Cosmos DB container.
    /// </summary>
    /// <param name="shops">The list of Shops to create or update in the database.</param>
    Task UpsertShopRecordsAsync(List<Shop> shops);
}