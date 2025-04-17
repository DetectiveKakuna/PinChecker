namespace PinChecker.Models;

/// <summary>
/// Represents a collection of changes made to a shop's inventory.
/// </summary>
public class ShopChanges
{
    /// <summary>
    /// The name identifier of the shop.
    /// </summary>
    public string ShopName { get; set; }

    /// <summary>
    /// Collection of new items added to the shop's inventory.
    /// </summary>
    public List<ShopItem> AddedItems { get; set; }

    /// <summary>
    /// Collection of existing items that have a new status in the shop's inventory.
    /// </summary>
    public List<(ShopItem oldState, ShopItem newState)> ChangedStatus { get; set; }
}