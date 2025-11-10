namespace PinChecker.Services;

/// <summary>
/// Service for managing error deduplication to prevent email spam.
/// </summary>
public interface IErrorDeduplicationService
{
    /// <summary>
    /// Records an error occurrence and checks if an email should be sent.
    /// Returns true if the error has occurred 5 times consecutively and an email should be sent.
    /// </summary>
    /// <param name="pageHtml">The page HTML content from the error.</param>
    /// <returns>True if an email should be sent (5 consecutive occurrences), false otherwise.</returns>
    Task<bool> ShouldSendErrorEmailAsync(string pageHtml);

    /// <summary>
    /// Clears the error history when the application runs successfully.
    /// </summary>
    Task ClearErrorHistoryAsync();
}