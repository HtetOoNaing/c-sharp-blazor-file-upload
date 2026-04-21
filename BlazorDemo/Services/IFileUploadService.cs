using Microsoft.AspNetCore.Components.Forms;

namespace BlazorDemo.Services;

public class FilePreviewDto
{
    public string Name { get; set; } = "";
    public long Size { get; set; }
    public string ContentType { get; set; } = "";
    public string? PreviewUrl { get; set; }
    public IBrowserFile? FileData { get; set; }
}

public class UploadResult
{
    public bool Success { get; set; }
    public string? FileName { get; set; }
    public string? StoredPath { get; set; }
    public string? ErrorMessage { get; set; }
}

public interface IFileUploadService
{
    string GenerateUniqueFileName(string originalFileName);
    Task<UploadResult> SaveFileAsync(IBrowserFile fileData, long maxFileSize);
    string GetUploadDirectoryPath();
    void EnsureUploadDirectoryExists();
}
