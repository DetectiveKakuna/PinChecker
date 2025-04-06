using PinChecker.Models;

namespace PinChecker.Services;

/// <summary>
/// Defines the contract for services that retrieve shop information using Playwright browser automation.
/// </summary>
public interface IPlaywrightService
{
    /// <summary>
    /// Retrieves the current status of a shop including its available inventory items.
    /// </summary>
    /// <returns>Shop information.</returns>
    Task<Shop> GetShopStatusAsync();
}