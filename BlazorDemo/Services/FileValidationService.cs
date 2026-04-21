using BlazorDemo.Models;
using Microsoft.Extensions.Options;

namespace BlazorDemo.Services;

public class FileValidationService : IFileValidationService
{
    private readonly string[] _allowedExtensions;
    private readonly ILogger<FileValidationService> _logger;
    private static readonly string[] SizeSuffixes = { "B", "KB", "MB", "GB" };

    public FileValidationService(IOptions<UploadOptions> options, ILogger<FileValidationService> logger)
    {
        _allowedExtensions = options.Value.AllowedExtensions;
        _logger = logger;
    }

    public bool IsImageFile(string? fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            _logger.LogDebug("IsImageFile called with null/empty filename");
            return false;
        }

        var extension = Path.GetExtension(fileName)?.ToLowerInvariant();
        var isValid = _allowedExtensions.Contains(extension);

        if (!isValid)
        {
            _logger.LogDebug("File '{FileName}' has extension '{Extension}' which is not in allowed list",
                fileName, extension);
        }

        return isValid;
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
        return _allowedExtensions.ToArray();
    }
}
