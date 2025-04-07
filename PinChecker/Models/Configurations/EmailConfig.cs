namespace PinChecker.Models.Configurations;

public class EmailConfig
{
    // SMTP Settings
    public required string SmtpServer { get; set; }
    public required int SmtpPort { get; set; }
    public required string SmtpUsername { get; set; }
    public required string SmtpPassword { get; set; }
    public required bool EnableSsl { get; set; }

    // Template Settings
    public required string TemplateName { get; set; }
    public required string TemplatePath { get; set; }

    // Email Settings
    public required string SenderName { get; set; }
    public required List<string> RecipientEmails { get; set; }
    public required string Subject { get; set; }

    // Email Appearance
    public required string EmailTitle { get; set; }
    public required string EmailSubtitle { get; set; }
    public required string FooterMessage { get; set; }

    // Section Headers
    public required string NewItemsHeader { get; set; }
    public required string ChangedItemsHeader { get; set; }
    public required string RemovedItemsHeader { get; set; }
}