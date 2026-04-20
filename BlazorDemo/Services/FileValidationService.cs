using BlazorDemo.Models;
using Microsoft.Extensions.Options;

namespace BlazorDemo.Services;

/// <summary>
/// Implementation of file validation logic.
/// Reads allowed extensions from UploadOptions in appsettings.json.
/// </summary>
public class FileValidationService : IFileValidationService
{
    private readonly string[] _allowedExtensions;
    private static readonly string[] SizeSuffixes = { "B", "KB", "MB", "GB" };

    public FileValidationService(IOptions<UploadOptions> options)
    {
        _allowedExtensions = options.Value.AllowedExtensions;
    }

    public bool IsImageFile(string? fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return false;

        var extension = Path.GetExtension(fileName)?.ToLowerInvariant();
        return _allowedExtensions.Contains(extension);
    }

    public string FormatFileSize(long bytes)
    {
        if (bytes < 0)
            throw new ArgumentOutOfRangeException(nameof(bytes), "File size cannot be negative.");

        double len = bytes;
        int order = 0;

        while (len >= 1024 && order < SizeSuffixes.Length - 1)
        {
            order++;
            len /= 1024;
        }

        return $"{len:0.##} {SizeSuffixes[order]}";
    }

    public bool IsWithinSizeLimit(long fileSize, long maxSize)
    {
        return fileSize <= maxSize;
    }

    public string[] GetAllowedExtensions()
    {
        // Return a copy to prevent external modification
        return _allowedExtensions.ToArray();
    }
}
