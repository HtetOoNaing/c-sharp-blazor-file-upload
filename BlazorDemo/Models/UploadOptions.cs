namespace BlazorDemo.Models;

/// <summary>
/// Strongly-typed configuration for file upload settings.
/// Bound to the "UploadOptions" section in appsettings.json.
/// </summary>
public class UploadOptions
{
    /// <summary>
    /// The configuration section name in appsettings.json.
    /// </summary>
    public const string SectionName = "UploadOptions";

    /// <summary>
    /// Maximum file size in bytes. Default: 3 MB (3,145,728 bytes).
    /// </summary>
    public long MaxFileSize { get; set; } = 1024 * 1024 * 3;

    /// <summary>
    /// Maximum number of files allowed per upload. Default: 3.
    /// </summary>
    public int MaxFileCount { get; set; } = 3;

    /// <summary>
    /// Allowed image file extensions (including the dot). 
    /// Default: [".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp"]
    /// </summary>
    public string[] AllowedExtensions { get; set; } = [
        ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp"
    ];

    /// <summary>
    /// Upload destination folder name inside wwwroot. Default: "uploads".
    /// </summary>
    public string UploadFolder { get; set; } = "uploads";
}
