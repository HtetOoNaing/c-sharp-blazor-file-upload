namespace BlazorDemo.Services;

/// <summary>
/// Provides file validation logic including type checking, size validation, and formatting.
/// </summary>
public interface IFileValidationService
{
    /// <summary>
    /// Validates if a file has an allowed image extension.
    /// </summary>
    /// <param name="fileName">The file name to check.</param>
    /// <returns>True if the file is an allowed image type.</returns>
    bool IsImageFile(string? fileName);

    /// <summary>
    /// Formats a file size in bytes to a human-readable string.
    /// </summary>
    /// <param name="bytes">The file size in bytes.</param>
    /// <returns>Formatted string like "1.5 MB".</returns>
    string FormatFileSize(long bytes);

    /// <summary>
    /// Checks if a file size is within the allowed limit.
    /// </summary>
    /// <param name="fileSize">The file size in bytes.</param>
    /// <param name="maxSize">The maximum allowed size in bytes.</param>
    /// <returns>True if within limit.</returns>
    bool IsWithinSizeLimit(long fileSize, long maxSize);

    /// <summary>
    /// Gets the list of allowed file extensions.
    /// </summary>
    /// <returns>Array of allowed extensions including the dot.</returns>
    string[] GetAllowedExtensions();
}
