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
    
    // Check if we should send an email (5 consecutive occurrences of the same error)
    var shouldSendEmail = await errorDeduplicationService.ShouldSendErrorEmailAsync(shopEx.PageHtml);
    
    if (!shouldSendEmail)
    {
        Console.WriteLine("Error threshold not met (need 5 consecutive occurrences), skipping email notification.");
    }
    else
    {
        Console.WriteLine("Error threshold reached (5 consecutive occurrences), sending email notifications.");
        
        // Send error email with page HTML
        try
        {
            var emailSent = await emailRepository.SendErrorEmailAsync(shopEx);
            Console.WriteLine($"Error email sent: {emailSent}");
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