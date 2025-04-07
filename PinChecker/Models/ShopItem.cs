using PinChecker.Models.Enums;

namespace PinChecker.Models;

/// <summary>
/// Represents an item available in a shop inventory.
/// </summary>
public record ShopItem
{
    /// <summary>
    /// The name identifier of the item.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The price of the item.
    /// </summary>
    public double Cost { get; set; }

    /// <summary>
    /// The current availability status of the item.
    /// </summary>
    public ShopStatus Status { get; set; }

    /// <summary>
    /// The URL link to the item's web page or resource.
    /// </summary>
    public string Link { get; set; }
}