namespace PinChecker.Models;

/// <summary>
/// Represents an item available in a shop inventory.
/// </summary>
public class ShopItem
{
    /// <summary>
    /// The name identifier of the item.
    /// </summary>
    public string ItemName { get; set; }

    /// <summary>
    /// The price of the item.
    /// </summary>
    public double Cost { get; set; }

    /// <summary>
    /// The current availability status of the item.
    /// </summary>
    public string Status { get; set; }
}
