namespace BlazorDemo.Services;

public interface IFileValidationService
{
    bool IsImageFile(string? fileName);
    bool HasValidFileSignature(byte[] headerBytes, string fileName);
    string FormatFileSize(long bytes);
    bool IsWithinSizeLimit(long fileSize, long maxSize);
    string[] GetAllowedExtensions();
}
