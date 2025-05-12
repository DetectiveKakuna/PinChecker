using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PinChecker.Models;
using PinChecker.Models.Configurations;

namespace PinChecker.Services.Implementations;

public class JsonFileService : IJsonFileService
{
    private readonly string _filePath;
    private readonly JsonSerializerSettings _jsonSettings;

    public JsonFileService(IOptions<JsonSaveFileConfig> configs)
    {
        _filePath = $"{configs.Value.Directory}/{configs.Value.File}";
        _jsonSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented
        };

        // Create directory if it doesn't exist
        Directory.CreateDirectory(configs.Value.Directory);

        // Create the file if it doesn't exist
        if (!File.Exists(_filePath))
            File.WriteAllText(_filePath, JsonConvert.SerializeObject(new List<Shop>(), _jsonSettings));
    }

    public async Task<IEnumerable<Shop>> GetShopRecordsAsync()
    {
        string json = await File.ReadAllTextAsync(_filePath);
        return JsonConvert.DeserializeObject<List<Shop>>(json, _jsonSettings) ?? [];
    }

    public async Task SetShopRecordsAsync(IEnumerable<Shop> items)
    {
        string json = JsonConvert.SerializeObject(items, _jsonSettings);
        await File.WriteAllTextAsync(_filePath, json);
    }
}