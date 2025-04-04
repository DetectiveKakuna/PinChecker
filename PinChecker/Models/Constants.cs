namespace PinChecker.Models;

/// <summary>
/// Contains application-wide constant values.
/// </summary>
public static class Constants
{
    /// <summary>
    /// Configuration key for accessing potato pins settings.
    /// </summary>
    public const string ConfigPotatoPins = "PotatoPins";

    /// <summary>
    /// The partition key used for shop documents in Cosmos DB.
    /// </summary>
    public const string CosmosShopPartition = "PinChecker_Shop";
}