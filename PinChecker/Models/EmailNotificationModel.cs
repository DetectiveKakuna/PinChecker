using PinChecker.Models.Enums;

namespace PinChecker.Models;

/// <summary>
/// Model for simple notification email when scraping fails.
/// </summary>
public class EmailNotificationModel
{
    /// <summary>
    /// The name of the shop that failed to scrape.
    /// </summary>
    public ShopName ShopName { get; set; }

    /// <summary>
    /// The URL of the shop site.
    /// </summary>
    public string ShopUrl { get; set; }

    /// <summary>
    /// The timestamp when the error occurred.
    /// </summary>
    public DateTime ErrorTimestamp { get; set; }

    // Email Appearance Properties
    public string EmailTitle { get; set; }
    public string EmailSubtitle { get; set; }
}