namespace PinChecker.Models;

/// <summary>
/// Represents a shop entity with its inventory of items.
/// </summary>
public record Shop
{
    /// <summary>
    /// The name identifier of the shop.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Collection of items available in the shop.
    /// </summary>
    public List<ShopItem> Items { get; set; }
}