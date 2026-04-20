using Microsoft.AspNetCore.Components.Forms;

namespace BlazorDemo.Services;

/// <summary>
/// Generates file previews including base64 image thumbnails.
/// </summary>
public class FilePreviewService : IFilePreviewService
{
    private readonly IFileValidationService _validationService;

    public FilePreviewService(IFileValidationService validationService)
    {
        _validationService = validationService;
    }

    public async Task<FilePreviewDto> CreatePreviewAsync(IBrowserFile file, long maxFileSize)
    {
        ArgumentNullException.ThrowIfNull(file);

        string? previewUrl = null;

        // Only generate preview for image files
        if (_validationService.IsImageFile(file.Name))
        {
            try
            {
                using var stream = file.OpenReadStream(maxFileSize);
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                var bytes = memoryStream.ToArray();
                previewUrl = $"data:{file.ContentType};base64,{Convert.ToBase64String(bytes)}";
            }
            catch (Exception)
            {
                // If preview generation fails, continue without preview
                previewUrl = null;
            }
        }

        return new FilePreviewDto
        {
            Name = file.Name,
            Size = file.Size,
            ContentType = file.ContentType,
            PreviewUrl = previewUrl,
            FileData = file
        };
    }
}
