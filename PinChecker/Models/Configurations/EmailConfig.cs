namespace PinChecker.Models.Configurations;

public class EmailConfig
{
    public required List<string> UpdateRecipients { get; set; }
    public required string ErrorRecipient { get; set; }
    public required string SmtpServer { get; set; }
    public required int Port { get; set; }
    public required string UserName { get; set; }
    public required string Password { get; set; }
    public required string SenderEmail { get; set; }
    public required string SenderName { get; set; }
}