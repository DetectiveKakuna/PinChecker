namespace PinChecker.Models.Configurations;

/// <summary>
/// Configuration settings for Azure Cosmos DB connection.
/// </summary>
public class CosmosDbConfig
{
    /// <summary>
    /// The Azure Cosmos DB connection string.
    /// </summary>
    public required string Uri { get; set; }

    /// <summary>
    /// The authentication key for accessing the Cosmos DB account.
    /// </summary>
    public required string Key { get; set; }

    /// <summary>
    /// The application name identifier used in Cosmos DB client options.
    /// </summary>
    public required string AppName { get; set; }

    /// <summary>
    /// The database ID for Cosmos DB.
    /// </summary>
    public required string DatabaseId { get; set; }

    /// <summary>
    /// The container ID for Cosmos DB.
    /// </summary>
    public required string ContainerId { get; set; }
}