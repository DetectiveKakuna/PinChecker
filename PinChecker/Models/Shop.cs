namespace PinChecker.Models;

/// <summary>
/// Represents a shop entity with its inventory of items.
/// </summary>
public class Shop
{
#pragma warning disable CS8618
    /// <summary>
    /// The name identifier of the shop.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Collection of items available in the shop.
    /// </summary>
    public List<ShopItem> Items { get; set; }
#pragma warning restore CS8618

    /// <summary>
    /// Determines whether the specified object is equal to the current shop.
    /// </summary>
    /// <param name="obj">The object to compare with the current shop.</param>
    /// <returns>true if the specified object is equal to the current shop; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is not Shop other)
            return false;

        if (Name != other.Name)
            return false;

        // If one list is null and the other isn't, they're not equal
        if (Items == null && other.Items != null || Items != null && other.Items == null)
            return false;

        // If both are null, they're equal
        if (Items == null && other.Items == null)
            return true;

        // If the counts don't match, they're not equal
        if (Items!.Count != other.Items!.Count)
            return false;

        // Compare all items
        return !Items.Except(other.Items).Any() && !other.Items.Except(Items).Any();
    }

    /// <summary>
    /// Serves as the default hash function for the shop entity.
    /// </summary>
    /// <returns>A hash code for the current shop.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Items?.Count ?? 0);
    }
}