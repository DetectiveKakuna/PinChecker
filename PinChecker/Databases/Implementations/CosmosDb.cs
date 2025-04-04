using AutoMapper;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using PinChecker.Models;
using PinChecker.Models.Configurations;
using PinChecker.Models.Database;

namespace PinChecker.Databases.Implementations;

/// <summary>
/// Implementation of the Cosmos DB service for accessing and managing shop data.
/// </summary>
public class CosmosDb : ICosmosDb
{
    #region Private Fields
    private readonly Container _floofNLizContainer;
    private readonly CosmosClient _cosmosClient;
    private readonly CosmosDbConfig _config;
    private readonly Database _floofNLizDatabase;
    private readonly IMapper _mapper;
    #endregion

    public CosmosDb(IMapper mapper, IOptions<CosmosDbConfig> configs)
    {
        _config = configs.Value;
        _cosmosClient = new(_config.Uri, _config.Key, new CosmosClientOptions() { ApplicationName = _config.AppName });
        _floofNLizDatabase = _cosmosClient.GetDatabase(_config.DatabaseId);
        _floofNLizContainer = _floofNLizDatabase.GetContainer(_config.ContainerId);
        _mapper = mapper;
    }

    public async Task<IEnumerable<Shop>> GetShopRecordsAsync()
    {
        var shops = new List<Shop>();

        var shopIterator = _floofNLizContainer.GetItemQueryIterator<CosmosShop>(ToGetQueryDefinition(Constants.CosmosShopPartition));

        while (shopIterator.HasMoreResults)
            shops.AddRange(_mapper.Map<List<Shop>>(await shopIterator.ReadNextAsync()));

        return shops;
    }

    public async Task UpsertShopRecordsAsync(List<Shop> shops)
    {
        await Task.WhenAll(shops.Select(shop => _floofNLizContainer.UpsertItemAsync(_mapper.Map<CosmosShop>(shop))));
    }

    #region Private Methods
    /// <summary>
    /// Creates a SQL query definition to filter Cosmos DB items by partition key.
    /// </summary>
    /// <param name="partitionKey">The partition key value used to filter records.</param>
    /// <returns>A QueryDefinition object containing the SQL query.</returns>
    private static QueryDefinition ToGetQueryDefinition(string partitionKey)
    {
        return new QueryDefinition($"SELECT * FROM c WHERE c.app_type = \"{partitionKey}\"");
    }
    #endregion
}