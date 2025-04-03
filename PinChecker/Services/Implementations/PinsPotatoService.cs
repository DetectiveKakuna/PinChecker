using Microsoft.Extensions.Options;
using PinChecker.Models.Configurations;

namespace PinChecker.Services.Implementations;

public class PinsPotatoService(IOptions<PlaywrightServiceConfig> config) : BasePlaywrightService(config)
{
    protected override Task GetInventoryAsync_Implementation()
    {
        throw new NotImplementedException();
    }
}