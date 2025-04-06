namespace PinChecker.Models.Database;

/// <summary>
/// Represents a shop entity stored in Cosmos DB.
/// </summary>
public class CosmosShop : BaseCosmosDbObject
{
    /// <summary>
    /// Collection of items available in the shop.
    /// </summary>
    public List<ShopItem> Items { get; set; }
}