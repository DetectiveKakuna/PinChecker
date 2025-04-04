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

    /// <summary>
    /// Determines whether the specified object is equal to the current shop item.
    /// </summary>
    /// <param name="obj">The object to compare with the current shop item.</param>
    /// <returns>true if the specified object is equal to the current shop item; otherwise, false.</returns>
    public override bool Equals(object obj)
    {
        if (obj is not ShopItem other)
            return false;

        return ItemName == other.ItemName &&
                           Math.Abs(Cost - other.Cost) < 0.001 &&
                           Status == other.Status;
    }

    /// <summary>
    /// Serves as the default hash function for the shop item.
    /// </summary>
    /// <returns>A hash code for the current shop item.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(ItemName, Cost, Status);
    }
}