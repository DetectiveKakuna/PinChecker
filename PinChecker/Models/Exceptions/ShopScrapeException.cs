using PinChecker.Models.Enums;

namespace PinChecker.Models.Exceptions;

/// <summary>
/// Exception thrown when an error occurs during shop scraping operations.
/// Contains page HTML content to assist with debugging.
/// </summary>
public class ShopScrapeException : Exception
{
    /// <summary>
    /// The name of the shop where the scraping error occurred.
    /// </summary>
    public ShopName ShopName { get; }

    /// <summary>
    /// The HTML content of the page at the time of the error.
    /// </summary>
    public string PageHtml { get; }

    /// <summary>
    /// Initializes a new instance of the ShopScrapeException class.
    /// </summary>
    /// <param name="shopName">The name of the shop where the error occurred.</param>
    /// <param name="message">The error message.</param>
    /// <param name="pageHtml">The HTML content of the page at the time of the error.</param>
    public ShopScrapeException(ShopName shopName, string message, string pageHtml)
        : base(message)
    {
        ShopName = shopName;
        PageHtml = pageHtml ?? string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the ShopScrapeException class.
    /// </summary>
    /// <param name="shopName">The name of the shop where the error occurred.</param>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception that caused this exception.</param>
    /// <param name="pageHtml">The HTML content of the page at the time of the error.</param>
    public ShopScrapeException(ShopName shopName, string message, Exception innerException, string pageHtml)
        : base(message, innerException)
    {
        ShopName = shopName;
        PageHtml = pageHtml ?? string.Empty;
    }
}
