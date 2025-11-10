using Microsoft.Extensions.Options;
using PinChecker.Models.Configurations;
using System.Text.Json;

namespace PinChecker.Services.Implementations;

/// <summary>
/// Service implementation for managing error deduplication to prevent email spam.
/// Tracks consecutive occurrences of the same error and only sends email after 5 consecutive occurrences.
/// </summary>
public class ErrorDeduplicationService : IErrorDeduplicationService
{
    private readonly string _errorFilePath;
    private const int ERROR_THRESHOLD = 5;

    public ErrorDeduplicationService(IOptions<JsonSaveFileConfig> config)
    {
        var directory = config.Value.Directory;
        
        // Ensure directory exists
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
            
        _errorFilePath = Path.Combine(directory, config.Value.ErrorFile);
    }

    public async Task<bool> ShouldSendErrorEmailAsync(string pageHtml)
    {
        try
        {
            var currentHtml = pageHtml ?? string.Empty;
            ErrorRecord errorRecord;

            // If no error file exists, this is the first occurrence
            if (!File.Exists(_errorFilePath))
            {
                errorRecord = new ErrorRecord
                {
                    ErrorHtml = currentHtml,
                    ConsecutiveCount = 1,
                    LastEmailSentCount = 0
                };
                await SaveErrorRecordAsync(errorRecord);
                return false; // Don't send email yet
            }

            // Read the existing error record
            var json = await File.ReadAllTextAsync(_errorFilePath);
            errorRecord = JsonSerializer.Deserialize<ErrorRecord>(json) ?? new ErrorRecord();

            // Check if this is the same error as before
            if (string.Equals(currentHtml, errorRecord.ErrorHtml, StringComparison.Ordinal))
            {
                // Same error - increment count
                errorRecord.ConsecutiveCount++;
                
                // Send email only if count is exactly 5 and we haven't sent one yet
                bool shouldSend = errorRecord.ConsecutiveCount == ERROR_THRESHOLD && errorRecord.LastEmailSentCount == 0;
                
                if (shouldSend)
                {
                    errorRecord.LastEmailSentCount = errorRecord.ConsecutiveCount;
                }
                
                await SaveErrorRecordAsync(errorRecord);
                return shouldSend;
            }
            else
            {
                // Different error - reset the count
                errorRecord = new ErrorRecord
                {
                    ErrorHtml = currentHtml,
                    ConsecutiveCount = 1,
                    LastEmailSentCount = 0
                };
                await SaveErrorRecordAsync(errorRecord);
                return false; // Don't send email yet for new error
            }
        }
        catch (Exception)
        {
            // If we can't read/write the file, default to not sending
            return false;
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

    private async Task SaveErrorRecordAsync(ErrorRecord record)
    {
        try
        {
            var json = JsonSerializer.Serialize(record, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_errorFilePath, json);
        }
        catch (Exception)
        {
            // If we can't write the file, silently continue
        }
    }

    private class ErrorRecord
    {
        public string ErrorHtml { get; set; } = string.Empty;
        public int ConsecutiveCount { get; set; }
        public int LastEmailSentCount { get; set; }
    }
}