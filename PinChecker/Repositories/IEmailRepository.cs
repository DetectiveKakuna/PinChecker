using PinChecker.Models;

namespace PinChecker.Repositories;

public interface IEmailRepository
{
    Task<bool> SendUpdateEmailAsync(List<ShopChanges> shopChangesList);
}