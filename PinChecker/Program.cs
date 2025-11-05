using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PinChecker.Models;
using PinChecker.Models.Configurations;
using PinChecker.Models.Exceptions;
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
services.Configure<EmailConfig>(configuration.GetSection("Email"));
services.Configure<JsonSaveFileConfig>(configuration.GetSection("JsonSaveFile"));
services.Configure<PlaywrightServiceConfig>(Constants.ConfigPotatoPins, configuration.GetSection(Constants.ConfigPotatoPins));

// Register Services
services.AddScoped<IJsonFileService, JsonFileService>();
services.AddScoped<IErrorDeduplicationService, ErrorDeduplicationService>();
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
var errorDeduplicationService = scope.ServiceProvider.GetRequiredService<IErrorDeduplicationService>();
var shopConfig = scope.ServiceProvider.GetRequiredService<IOptionsMonitor<PlaywrightServiceConfig>>().Get(Constants.ConfigPotatoPins);

try
{
    List<ShopChanges> shopChanges = [.. (await shopRepository.GetShopChangesAsync())];

    if (shopChanges.Count > 0)
    {
        // Send email with the changes
        var response = await emailRepository.SendUpdateEmailAsync(shopChanges);
        Console.WriteLine($"Email sent: ({shopChanges.SelectMany(s => s.AddedItems).Count()}) items added." +
            $" ({shopChanges.SelectMany(s => s.ChangedItems).Count()}) statuses changed.");
    }

    // Log shop updates to capture changes not tracked for the email
    await shopRepository.UpdateShopRecordsAsync();

    // Clear error history on successful execution
    await errorDeduplicationService.ClearErrorHistoryAsync();

    Console.WriteLine($"Successful Execution");
}
catch (ShopScrapeException shopEx)
{
    Console.WriteLine($"Shop scraping error: {shopEx.Message}");
    
    // Check if this error is a duplicate
    var isDuplicate = await errorDeduplicationService.IsErrorDuplicateAsync(shopEx.PageHtml);
    
    if (isDuplicate)
    {
        Console.WriteLine("Error is duplicate of previous error, skipping email notification.");
    }
    else
    {
        // Send error email with page HTML
        try
        {
            var emailSent = await emailRepository.SendErrorEmailAsync(shopEx);
            Console.WriteLine($"Error email sent: {emailSent}");
            
            // Record this error after successfully sending email
            if (emailSent)
            {
                await errorDeduplicationService.RecordErrorAsync(shopEx.PageHtml);
                Console.WriteLine("Error recorded for deduplication.");
            }
        }
        catch (Exception emailEx)
        {
            Console.WriteLine($"Failed to send error email: {emailEx.Message}");
        }

        // Send notification email to regular users
        try
        {
            var notificationSent = await emailRepository.SendNotificationEmailAsync(shopEx, shopConfig.Url);
            Console.WriteLine($"Notification email sent: {notificationSent}");
        }
        catch (Exception notificationEx)
        {
            Console.WriteLine($"Failed to send notification email: {notificationEx.Message}");
        }
    }
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
}