using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PinChecker.Databases;
using PinChecker.Databases.Implementations;
using PinChecker.Models;
using PinChecker.Models.Configurations;
using PinChecker.Repositories;
using PinChecker.Repositories.Implementations;
using PinChecker.Services;
using PinChecker.Services.Implementations;
using System.Reflection;


var configurationBuilder = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

var configuration = configurationBuilder.Build();

// Configure services
var services = new ServiceCollection();

// Register AutoMapper
services.AddAutoMapper(Assembly.GetExecutingAssembly());

// Register Configs
services.Configure<PlaywrightServiceConfig>(Constants.ConfigPotatoPins, configuration.GetSection(Constants.ConfigPotatoPins));
services.Configure<CosmosDbConfig>(configuration.GetSection("CosmosDb"));

// Register Cosmos DB service
services.AddScoped<ICosmosDb, CosmosDb>();

// Register Services
services.AddScoped<IPlaywrightService>(sp => new PotatoPinsService(Options.Create(sp.GetRequiredService<IOptionsMonitor<PlaywrightServiceConfig>>().Get(Constants.ConfigPotatoPins))));

// Register Repositories
services.AddScoped<IShopRepository, ShopRepository>();

// Build Service Provider
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