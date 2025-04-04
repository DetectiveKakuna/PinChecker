namespace PinChecker.Models;

/// <summary>
/// Represents a collection of changes made to a shop's inventory.
/// </summary>
public class ShopChanges
{
#pragma warning disable CS8618
    /// <summary>
    /// The name identifier of the shop.
    /// </summary>
    public string Name { get; set; }

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
#pragma warning restore CS8618
}