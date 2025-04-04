// Ignore Spelling: app

namespace PinChecker.Models.Database;

/// <summary>
/// Base class for Cosmos DB objects.
/// </summary>
public abstract class BaseCosmosDbObject
{
    /// <summary>
    /// The unique identifier for the Cosmos DB object.
    /// </summary>
    public string id { get; set; }

    /// <summary>
    /// The partition key associated with the Cosmos DB object.
    /// Format: ApplicationName_ObjectType
    /// </summary>
    public string app_type { get; set; }
}