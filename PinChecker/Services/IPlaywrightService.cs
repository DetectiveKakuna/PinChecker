// Ignore Spelling: Etsy

using PinChecker.Models;

namespace PinChecker.Services;

public interface IPlaywrightService
{
    Task<Shop> GetShopStatusAsync();
}