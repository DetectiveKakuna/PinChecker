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
    /// Collection of existing items that have been modified in the shop's inventory.
    /// </summary>
    public List<(ShopItem oldState, ShopItem newState)> ChangedItems { get; set; }

    /// <summary>
    /// Collection of items removed from the shop's inventory.
    /// </summary>
    public List<ShopItem> RemovedItems { get; set; }
}