using Microsoft.Extensions.Options;
using PinChecker.Models;
using PinChecker.Models.Configurations;
using PinChecker.Models.Exceptions;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Logging;
using FluentEmail.Core;
using FluentEmail.Smtp;
using RazorLight;

namespace PinChecker.Repositories.Implementations;

public class EmailRepository : IEmailRepository
{
    private readonly ILogger<EmailRepository> _logger;
    private readonly EmailConfig _emailUpdateConfig;
    private readonly RazorLightEngine _razorEngine;

    public EmailRepository(ILogger<EmailRepository> logger, IOptions<EmailConfig> config)
    {
        _logger = logger;
        _emailUpdateConfig = config.Value;

        // Get templates path - use absolute path if relative path provided
        string templatePath = _emailUpdateConfig.TemplatePath;
        if (!Path.IsPathRooted(templatePath))
            templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, templatePath);

        // Initialize RazorLight engine
        _razorEngine = new RazorLightEngineBuilder()
            .UseFileSystemProject(templatePath)
            .UseMemoryCachingProvider()
            .Build();

        // Configure FluentEmail
        Email.DefaultSender = new SmtpSender(() => new SmtpClient(_emailUpdateConfig.SmtpServer)
        {
            Port = _emailUpdateConfig.SmtpPort,
            Credentials = new NetworkCredential(_emailUpdateConfig.SmtpUsername, _emailUpdateConfig.SmtpPassword),
            EnableSsl = _emailUpdateConfig.EnableSsl
        });
    }

    public async Task<bool> SendUpdateEmailAsync(List<ShopChanges> shopChangesList)
    {
        try
        {
            // Create the email template model
            var emailModel = new EmailUpdateModel
            {
                Changes = shopChangesList,
                EmailTitle = _emailUpdateConfig.EmailTitle,
                EmailSubtitle = _emailUpdateConfig.EmailSubtitle,
                FooterMessage = _emailUpdateConfig.FooterMessage,
                NewItemsHeader = _emailUpdateConfig.NewItemsHeader,
                ChangedItemsHeader = _emailUpdateConfig.ChangedItemsHeader,
                RemovedItemsHeader = _emailUpdateConfig.RemovedItemsHeader
            };

            // Render email template
            var emailHtml = await RenderEmailTemplateAsync("EmailUpdate", emailModel);

            // Build email
            var email = Email
                .From(_emailUpdateConfig.SmtpUsername, _emailUpdateConfig.SenderName)
                .Subject(_emailUpdateConfig.Subject)
                .Body(emailHtml, true);

            // Add all recipients as BCC from the config
            foreach (var recipientEmail in _emailUpdateConfig.RecipientEmails)
                email.BCC(recipientEmail);

            // Send the email
            var response = await email.SendAsync();

            _logger.LogInformation($"Email sent to {_emailUpdateConfig.RecipientEmails.Count} recipients with status: {response.Successful}");
            return response.Successful;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error sending pin update email to {_emailUpdateConfig.RecipientEmails.Count} recipients");
            return false;
        }
    }

    public async Task<bool> SendErrorEmailAsync(ShopScrapeException exception)
    {
        try
        {
            // Create the error email template model
            var emailModel = new EmailErrorModel
            {
                ShopName = exception.ShopName,
                ErrorMessage = exception.Message,
                PageHtml = exception.PageHtml,
                ErrorTimestamp = DateTime.UtcNow,
                InnerExceptionDetails = exception.InnerException?.ToString() ?? string.Empty,
                StackTrace = exception.StackTrace ?? string.Empty,
                EmailTitle = "Shop Scraping Error",
                EmailSubtitle = $"Error occurred while scraping {exception.ShopName}"
            };

            // Render error email template
            var emailHtml = await RenderEmailTemplateAsync("EmailError", emailModel);

            // Build email
            var email = Email
                .From(_emailUpdateConfig.SmtpUsername, _emailUpdateConfig.SenderName)
                .Subject($"[ERROR] Shop Scraping Failed - {exception.ShopName}")
                .Body(emailHtml, true);

            // Add all recipients as BCC from the config
            foreach (var recipientEmail in _emailUpdateConfig.ErrorRecipientEmails)
                email.BCC(recipientEmail);

            // Send the email
            var response = await email.SendAsync();

            _logger.LogInformation($"Error email sent to {_emailUpdateConfig.ErrorRecipientEmails.Count} recipients with status: {response.Successful}");
            return response.Successful;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error sending error email for {exception.ShopName} scraping failure to {_emailUpdateConfig.ErrorRecipientEmails.Count} recipients");
            return false;
        }
    }

    public async Task<bool> SendNotificationEmailAsync(ShopScrapeException exception, string shopUrl)
    {
        try
        {
            // Create the notification email template model
            var emailModel = new EmailNotificationModel
            {
                ShopName = exception.ShopName,
                ShopUrl = shopUrl,
                ErrorTimestamp = DateTime.UtcNow,
                EmailTitle = "Shop Monitoring Alert",
                EmailSubtitle = "Scraping issue detected"
            };

            // Render notification email template
            var emailHtml = await RenderEmailTemplateAsync("EmailNotification", emailModel);

            // Build email
            var email = Email
                .From(_emailUpdateConfig.SmtpUsername, _emailUpdateConfig.SenderName)
                .Subject($"[ALERT] {exception.ShopName} - Scraping Issue")
                .Body(emailHtml, true);

            // Add all regular recipients as BCC
            foreach (var recipientEmail in _emailUpdateConfig.RecipientEmails)
                email.BCC(recipientEmail);

            // Send the email
            var response = await email.SendAsync();

            _logger.LogInformation($"Notification email sent to {_emailUpdateConfig.RecipientEmails.Count} recipients with status: {response.Successful}");
            return response.Successful;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error sending notification email for {exception.ShopName} scraping failure to {_emailUpdateConfig.RecipientEmails.Count} recipients");
            return false;
        }
    }

    private async Task<string> RenderEmailTemplateAsync<T>(string templateName, T model)
    {
        try
        {
            return await _razorEngine.CompileRenderAsync(templateName, model); ;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error rendering email template {templateName}");
            throw;
        }
    }
}