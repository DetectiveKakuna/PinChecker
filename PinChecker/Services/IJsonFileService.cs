using PinChecker.Models;

namespace PinChecker.Services;

public interface IJsonFileService
{
    Task<IEnumerable<Shop>> GetShopRecordsAsync();
    Task SetShopRecordsAsync(IEnumerable<Shop> items);
}