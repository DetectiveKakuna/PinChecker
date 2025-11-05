using Microsoft.Extensions.Options;
using PinChecker.Models.Configurations;

namespace PinChecker.Services.Implementations;

/// <summary>
/// Service implementation for managing error deduplication to prevent email spam.
/// </summary>
public class ErrorDeduplicationService : IErrorDeduplicationService
{
    private readonly string _errorFilePath;

    public ErrorDeduplicationService(IOptions<JsonSaveFileConfig> config)
    {
        var directory = config.Value.Directory;
        
        // Ensure directory exists
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
            
        _errorFilePath = Path.Combine(directory, config.Value.ErrorFile);
    }

    public async Task<bool> IsErrorDuplicateAsync(string pageHtml)
    {
        try
        {
            // If no error file exists, this is not a duplicate
            if (!File.Exists(_errorFilePath))
                return false;

            // Read the last error HTML
            var lastErrorHtml = await File.ReadAllTextAsync(_errorFilePath);

            // Compare the page HTML content (normalize null/empty strings)
            var currentHtml = pageHtml ?? string.Empty;
            return string.Equals(currentHtml, lastErrorHtml, StringComparison.Ordinal);
        }
        catch (Exception)
        {
            // If we can't read the file or any other issue, assume not duplicate
            return false;
        }
    }

    public async Task RecordErrorAsync(string pageHtml)
    {
        try
        {
            // Write the page HTML to the error file (even if null or empty)
            var htmlToWrite = pageHtml ?? string.Empty;
            await File.WriteAllTextAsync(_errorFilePath, htmlToWrite);
        }
        catch (Exception)
        {
            // If we can't write the file, silently continue
            // This shouldn't break the application flow
        }
    }

    public async Task ClearErrorHistoryAsync()
    {
        try
        {
            // Delete the error file if it exists
            if (File.Exists(_errorFilePath))
            {
                File.Delete(_errorFilePath);
            }
        }
        catch (Exception)
        {
            // If we can't delete the file, silently continue
            // This shouldn't break the application flow
        }
        
        await Task.CompletedTask;
    }
}