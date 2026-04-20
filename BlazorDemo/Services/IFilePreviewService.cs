using Microsoft.AspNetCore.Components.Forms;

namespace BlazorDemo.Services;

/// <summary>
/// Handles generation of file previews (thumbnails, base64 images, etc.).
/// </summary>
public interface IFilePreviewService
{
    /// <summary>
    /// Creates a file preview DTO from an uploaded browser file.
    /// </summary>
    /// <param name="file">The browser file.</param>
    /// <param name="maxFileSize">Maximum allowed file size for reading.</param>
    /// <returns>File preview DTO with optional base64 preview for images.</returns>
    Task<FilePreviewDto> CreatePreviewAsync(IBrowserFile file, long maxFileSize);
}
