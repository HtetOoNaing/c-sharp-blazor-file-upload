namespace BlazorDemo.Models;

public class UploadOptions
{
    public const string SectionName = "UploadOptions";
    public long MaxFileSize { get; set; } = 1024 * 1024 * 3;
    public int MaxFileCount { get; set; } = 3;
    public string[] AllowedExtensions { get; set; } = [
        ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp"
    ];
    public string UploadFolder { get; set; } = "uploads";
}
