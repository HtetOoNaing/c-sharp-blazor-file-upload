using Microsoft.AspNetCore.DataProtection;
using System.Security.Cryptography;

namespace BlazorDemo.Services;

/// <summary>
/// Service to protect sensitive file metadata using ASP.NET Core Data Protection.
/// This ensures that file names and other metadata are encrypted at rest and in transit.
/// </summary>
public class FileMetadataProtectionService : IFileMetadataProtectionService
{
    private readonly IDataProtector _protector;
    private const string Purpose = "FileUpload.Metadata.v1";

    public FileMetadataProtectionService(IDataProtectionProvider dataProtectionProvider)
    {
        _protector = dataProtectionProvider.CreateProtector(Purpose);
    }

    /// <summary>
    /// Protects (encrypts) sensitive file metadata like the original file name.
    /// </summary>
    public string ProtectFileName(string originalFileName)
    {
        if (string.IsNullOrEmpty(originalFileName))
            return originalFileName;

        return _protector.Protect(originalFileName);
    }

    /// <summary>
    /// Unprotects (decrypts) the original file name.
    /// </summary>
    public string? UnprotectFileName(string protectedFileName)
    {
        if (string.IsNullOrEmpty(protectedFileName))
            return protectedFileName;

        try
        {
            return _protector.Unprotect(protectedFileName);
        }
        catch (CryptographicException)
        {
            // Return null if decryption fails (tampered or invalid data)
            return null;
        }
    }

    /// <summary>
    /// Protects arbitrary metadata strings.
    /// </summary>
    public string ProtectMetadata(string metadata)
    {
        if (string.IsNullOrEmpty(metadata))
            return metadata;

        return _protector.Protect(metadata);
    }

    /// <summary>
    /// Unprotects arbitrary metadata strings.
    /// </summary>
    public string? UnprotectMetadata(string protectedMetadata)
    {
        if (string.IsNullOrEmpty(protectedMetadata))
            return protectedMetadata;

        try
        {
            return _protector.Unprotect(protectedMetadata);
        }
        catch (CryptographicException)
        {
            return null;
        }
    }
}
