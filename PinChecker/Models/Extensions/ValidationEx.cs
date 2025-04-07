namespace PinChecker.Models.Extensions;

/// <summary>
/// Contains extension methods for model validation.
/// </summary>
public static class ValidationEx
{
    /// <summary>
    /// Determines whether the specified Shop is valid.
    /// </summary>
    /// <param name="shop">The Shop to validate.</param>
    /// <returns>true if name is not null or whitespace, items count is greater than 1, and all items are valid; otherwise, false.</returns>
    public static bool IsValid(this Shop shop)
    {
        if (shop == null)
            return false;

        // Check if name is valid
        if (string.IsNullOrWhiteSpace(shop.Name))
            return false;

        // Check if items exists and has more than 1 item
        if (shop.Items == null || shop.Items.Count <= 1)
            return false;

        // Validate all items in the shop
        if (!shop.Items.All(item => item.IsValid()))
            return false;

        return true;
    }

    /// <summary>
    /// Determines whether the specified ShopChanges is valid.
    /// </summary>
    /// <param name="changes">The ShopChanges to validate.</param>
    /// <returns>true if name is not null or whitespace, at least one list has items, and all items are valid; otherwise, false.</returns>
    public static bool IsValid(this ShopChanges changes)
    {
        if (changes == null)
            return false;

        // Check if name is valid
        if (string.IsNullOrWhiteSpace(changes.ShopName))
            return false;

        // Check if at least one list has count > 1
        bool hasItems = (changes.AddedItems?.Count > 1) ||
                        (changes.ChangedItems?.Count > 1) ||
                        (changes.RemovedItems?.Count > 1);

        if (!hasItems)
            return false;

        // Validate all items in AddedItems
        if (changes.AddedItems != null && !changes.AddedItems.All(item => item.IsValid()))
            return false;

        // Validate all items in ChangedItems
        if (changes.ChangedItems != null &&
            changes.ChangedItems.Any(tuple =>
                (tuple.oldState != null && !tuple.oldState.IsValid()) ||
                (tuple.newState != null && !tuple.newState.IsValid())))
            return false;

        // Validate all items in RemovedItems
        if (changes.RemovedItems != null && !changes.RemovedItems.All(item => item.IsValid()))
            return false;

        return true;
    }

    /// <summary>
    /// Determines whether the specified ShopItem is valid.
    /// </summary>
    /// <param name="item">The ShopItem to validate.</param>
    /// <returns>true if all string properties are not null or whitespace and cost is greater than 0; otherwise, false.</returns>
    public static bool IsValid(this ShopItem item)
    {
        if (item == null)
            return false;

        return !string.IsNullOrWhiteSpace(item.Name) &&
               !string.IsNullOrWhiteSpace(item.Link) &&
               item.Status != Enums.ShopStatus.Unknown &&
               item.Cost > 0;
    }
}