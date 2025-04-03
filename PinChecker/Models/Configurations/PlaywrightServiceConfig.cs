namespace PinChecker.Models.Configurations;

public class PlaywrightServiceConfig
{
    /// <summary>
    /// Gets or sets the target URL that the Playwright browser will navigate to when performing operations.
    /// This is a required property that must be set when configuring the service.
    /// </summary>
    public required string Url { get; set; }

    /// <summary>
    /// Gets or sets the timeout in milliseconds for Playwright operations.
    /// </summary>
    public int Timeout { get; set; } = 30000;

    /// <summary>
    /// Gets or sets whether to run the browser in headless mode.
    /// </summary>
    public bool Headless { get; set; } = true;
}