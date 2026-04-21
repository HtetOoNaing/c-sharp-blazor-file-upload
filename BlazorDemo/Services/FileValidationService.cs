using BlazorDemo.Models;
using Microsoft.Extensions.Options;

namespace BlazorDemo.Services;

public class FileValidationService : IFileValidationService
{
    private readonly string[] _allowedExtensions;
    private readonly ILogger<FileValidationService> _logger;
    private static readonly string[] SizeSuffixes = { "B", "KB", "MB", "GB" };

    private static readonly Dictionary<string, byte[][]> FileSignatures = new()
    {
        { ".jpg",  [new byte[] { 0xFF, 0xD8, 0xFF }] },
        { ".jpeg", [new byte[] { 0xFF, 0xD8, 0xFF }] },
        { ".png",  [new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }] },
        { ".gif",  [new byte[] { 0x47, 0x49, 0x46, 0x38, 0x37, 0x61 },
                     new byte[] { 0x47, 0x49, 0x46, 0x38, 0x39, 0x61 }] },
        { ".bmp",  [new byte[] { 0x42, 0x4D }] },
        { ".webp", [new byte[] { 0x52, 0x49, 0x46, 0x46 }] }
    };

    public const int SignatureMaxLength = 8;

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

    public bool HasValidFileSignature(byte[] headerBytes, string fileName)
    {
        ArgumentNullException.ThrowIfNull(headerBytes);
        ArgumentException.ThrowIfNullOrEmpty(fileName);

        var extension = Path.GetExtension(fileName)?.ToLowerInvariant();

        if (string.IsNullOrEmpty(extension) || !FileSignatures.TryGetValue(extension, out var signatures))
        {
            _logger.LogWarning("No known file signature for extension '{Extension}' on file '{FileName}'",
                extension, fileName);
            return false;
        }

        foreach (var signature in signatures)
        {
            if (headerBytes.Length >= signature.Length &&
                headerBytes.AsSpan(0, signature.Length).SequenceEqual(signature))
            {
                return true;
            }
        }

        _logger.LogWarning(
            "File '{FileName}' has extension '{Extension}' but its bytes do not match any known signature — possible spoofed file",
            fileName, extension);
        return false;
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
