namespace PinChecker.Services;

/// <summary>
/// Service for managing error deduplication to prevent email spam.
/// </summary>
public interface IErrorDeduplicationService
{
    /// <summary>
    /// Checks if the error with the given page HTML is a duplicate of the last error.
    /// </summary>
    /// <param name="pageHtml">The page HTML content from the error.</param>
    /// <returns>True if this error is a duplicate, false otherwise.</returns>
    Task<bool> IsErrorDuplicateAsync(string pageHtml);

    /// <summary>
    /// Records the error after successfully sending an email.
    /// </summary>
    /// <param name="pageHtml">The page HTML content from the error.</param>
    Task RecordErrorAsync(string pageHtml);

    /// <summary>
    /// Clears the error history when the application runs successfully.
    /// </summary>
    Task ClearErrorHistoryAsync();
}