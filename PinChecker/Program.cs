using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PinChecker.Models.Configurations;
using PinChecker.Repositories;
using PinChecker.Repositories.Implementations;
using PinChecker.Services;
using PinChecker.Services.Implementations;

// Set up configuration
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
Console.WriteLine($"Environment: {environment}");

var configurationBuilder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Override with local settings if in local environment
if (environment.Equals("local", StringComparison.OrdinalIgnoreCase))
{
    configurationBuilder.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);
}

var configuration = configurationBuilder.Build();

// Configure services
var services = new ServiceCollection();

// Register configuration sections
services.Configure<PlaywrightServiceConfig>(configuration.GetSection("FantasyPinGarden"));
services.Configure<PlaywrightServiceConfig>(configuration.GetSection("PinsPotato"));

// Register services
services.AddTransient<IPlaywrightService, FantasyPinGardenService>();
services.AddTransient<IPlaywrightService, PinsPotatoService>();

// Register repositories
services.AddTransient<IShopRepository, ShopRepository>();

// Build service provider
var serviceProvider = services.BuildServiceProvider();

// Execute repository method
using var scope = serviceProvider.CreateScope();

try
{
    var shopRepository = scope.ServiceProvider.GetRequiredService<IShopRepository>();
    Console.WriteLine("Running IShopRepository.Test()...");
    await shopRepository.Test();
    Console.WriteLine("Test completed successfully!");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
