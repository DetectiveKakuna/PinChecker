using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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

// Add logging
services.AddLogging(builder =>
{
    builder.AddConfiguration(configuration.GetSection("Logging"));
    builder.AddConsole();
});

// Register AutoMapper
services.AddAutoMapper(Assembly.GetExecutingAssembly());

// Register Configs
services.Configure<CosmosDbConfig>(configuration.GetSection("CosmosDb"));
services.Configure<EmailConfig>(configuration.GetSection("Email"));
services.Configure<PlaywrightServiceConfig>(Constants.ConfigPotatoPins, configuration.GetSection(Constants.ConfigPotatoPins));

// Register Cosmos DB service
services.AddScoped<ICosmosDb, CosmosDb>();

// Register Services
services.AddScoped<IPlaywrightService>(sp => new PotatoPinsService(Options.Create(sp.GetRequiredService<IOptionsMonitor<PlaywrightServiceConfig>>().Get(Constants.ConfigPotatoPins))));

// Register Repositories
services.AddScoped<IEmailRepository, EmailRepository>();
services.AddScoped<IShopRepository, ShopRepository>();

// Build Service Provider
var serviceProvider = services.BuildServiceProvider();
using var scope = serviceProvider.CreateScope();

// Resolve services
var emailRepository = scope.ServiceProvider.GetRequiredService<IEmailRepository>();
var shopRepository = scope.ServiceProvider.GetRequiredService<IShopRepository>();

try
{
    List<ShopChanges> shopChanges = [.. (await shopRepository.GetShopChangesAsync())];

    if (shopChanges.Count > 0)
    {
        // Send email with the changes
        var response = await emailRepository.SendUpdateEmailAsync(shopChanges);
        Console.WriteLine($"Email sent: ({shopChanges.SelectMany(s => s.AddedItems).Count()}) items added." +
            $" ({shopChanges.SelectMany(s => s.ChangedStatus).Count()}) statuses changed.");
    }

    // Log the changes
    await shopRepository.UpdateShopRecordsAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
finally
{
    // Dispose of the service provider
    if (serviceProvider is IDisposable disposable)
    {
        disposable.Dispose();
    }
    Console.WriteLine($"Successful Execution");
}