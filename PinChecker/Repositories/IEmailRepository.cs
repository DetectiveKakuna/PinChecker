using PinChecker.Models;

namespace PinChecker.Repositories;

public interface IEmailRepository
{
    void SendShopChangesEmail(List<ShopChanges> changes);
}