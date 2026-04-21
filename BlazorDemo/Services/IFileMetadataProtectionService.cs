namespace BlazorDemo.Services;

/// <summary>
/// Interface for protecting sensitive file metadata using encryption.
/// </summary>
public interface IFileMetadataProtectionService
{
    /// <summary>
    /// Protects (encrypts) sensitive file metadata like the original file name.
    /// </summary>
    string ProtectFileName(string originalFileName);

    /// <summary>
    /// Unprotects (decrypts) the original file name.
    /// Returns null if decryption fails.
    /// </summary>
    string? UnprotectFileName(string protectedFileName);

    /// <summary>
    /// Protects arbitrary metadata strings.
    /// </summary>
    string ProtectMetadata(string metadata);

    /// <summary>
    /// Unprotects arbitrary metadata strings.
    /// Returns null if decryption fails.
    /// </summary>
    string? UnprotectMetadata(string protectedMetadata);
}
