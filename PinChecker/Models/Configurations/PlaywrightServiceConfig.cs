namespace PinChecker.Models.Configurations;

/// <summary>
/// Configuration settings for Playwright browser automation services.
/// </summary>
public class PlaywrightServiceConfig
{
    /// <summary>
    /// The target URL that the Playwright browser will navigate to when performing operations.
    /// </summary>
    public required string Url { get; set; }

    /// <summary>
    /// The timeout in milliseconds for Playwright operations.
    /// </summary>
    public required int Timeout { get; set; }
}