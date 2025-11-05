using PinChecker.Models.Enums;

namespace PinChecker.Models;

/// <summary>
/// Model for error email template containing scraping error details.
/// </summary>
public class EmailErrorModel
{
    /// <summary>
    /// The name of the shop where the error occurred.
    /// </summary>
    public ShopName ShopName { get; set; }

    /// <summary>
    /// The error message.
    /// </summary>
    public string ErrorMessage { get; set; }

    /// <summary>
    /// The HTML content of the page at the time of the error.
    /// </summary>
    public string PageHtml { get; set; }

    /// <summary>
    /// The timestamp when the error occurred.
    /// </summary>
    public DateTime ErrorTimestamp { get; set; }

    /// <summary>
    /// The inner exception details if available.
    /// </summary>
    public string InnerExceptionDetails { get; set; }

    /// <summary>
    /// The stack trace of the exception.
    /// </summary>
    public string StackTrace { get; set; }

    // Email Appearance Properties
    public string EmailTitle { get; set; }
    public string EmailSubtitle { get; set; }
}