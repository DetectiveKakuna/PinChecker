using PinChecker.Models;
using PinChecker.Models.Exceptions;

namespace PinChecker.Repositories;

public interface IEmailRepository
{
    Task<bool> SendUpdateEmailAsync(List<ShopChanges> shopChangesList);
    Task<bool> SendErrorEmailAsync(ShopScrapeException exception);
    Task<bool> SendNotificationEmailAsync(ShopScrapeException exception, string shopUrl);
}