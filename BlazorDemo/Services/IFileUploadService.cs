using Microsoft.AspNetCore.Components.Forms;

namespace BlazorDemo.Services;

/// <summary>
/// Represents a file ready for preview/upload with metadata.
/// </summary>
public class FilePreviewDto
{
    public string Name { get; set; } = "";
    public long Size { get; set; }
    public string ContentType { get; set; } = "";
    public string? PreviewUrl { get; set; }
    public IBrowserFile? FileData { get; set; }
}

/// <summary>
/// Result of a file upload operation.
/// </summary>
public class UploadResult
{
    public bool Success { get; set; }
    public string? FileName { get; set; }
    public string? StoredPath { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Handles file storage operations on the server.
/// </summary>
public interface IFileUploadService
{
    /// <summary>
    /// Generates a unique filename with GUID prefix.
    /// </summary>
    /// <param name="originalFileName">The original file name.</param>
    /// <returns>Unique file name with format: {Guid}_{originalName}</returns>
    string GenerateUniqueFileName(string originalFileName);

    /// <summary>
    /// Saves a file to the upload directory.
    /// </summary>
    /// <param name="fileData">The browser file to save.</param>
    /// <param name="maxFileSize">Maximum allowed file size.</param>
    /// <returns>Upload result with success status and path.</returns>
    Task<UploadResult> SaveFileAsync(IBrowserFile fileData, long maxFileSize);

    /// <summary>
    /// Gets the full upload directory path.
    /// </summary>
    /// <returns>The absolute path to the uploads folder.</returns>
    string GetUploadDirectoryPath();

    /// <summary>
    /// Creates the upload directory if it doesn't exist.
    /// </summary>
    void EnsureUploadDirectoryExists();
}
