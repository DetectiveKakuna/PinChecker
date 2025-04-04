using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PinChecker.Models.Configurations;
using PinChecker.Repositories;
using PinChecker.Repositories.Implementations;
using PinChecker.Services;
using PinChecker.Services.Implementations;


var configurationBuilder = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

var configuration = configurationBuilder.Build();

// Configure services
var services = new ServiceCollection();

// Register configurations with named options
services.Configure<PlaywrightServiceConfig>("PotatoPins", configuration.GetSection("PotatoPins"));

// Register services
services.AddScoped<IPlaywrightService>(sp => new PotatoPinsService(Options.Create(sp.GetRequiredService<IOptionsMonitor<PlaywrightServiceConfig>>().Get("PinsPotato"))));

// Register Repositories
services.AddScoped<IShopRepository, ShopRepository>();

// Build service provider
var serviceProvider = services.BuildServiceProvider();

// Execute repository method
using var scope = serviceProvider.CreateScope();

try
{
    var shopRepository = scope.ServiceProvider.GetRequiredService<IShopRepository>();
    Console.WriteLine("Running IShopRepository.Test()...");
    await shopRepository.GetShopChanges();
    Console.WriteLine("Test completed successfully!");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}