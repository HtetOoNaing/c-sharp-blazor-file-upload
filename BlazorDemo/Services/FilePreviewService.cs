using Microsoft.AspNetCore.Components.Forms;

namespace BlazorDemo.Services;

public class FilePreviewService : IFilePreviewService
{
    private readonly IFileValidationService _validationService;
    private readonly ILogger<FilePreviewService> _logger;

    public FilePreviewService(IFileValidationService validationService, ILogger<FilePreviewService> logger)
    {
        _validationService = validationService;
        _logger = logger;
    }

    public async Task<FilePreviewDto> CreatePreviewAsync(IBrowserFile file, long maxFileSize)
    {
        ArgumentNullException.ThrowIfNull(file);

        string? previewUrl = null;

        if (_validationService.IsImageFile(file.Name))
        {
            try
            {
                using var stream = file.OpenReadStream(maxFileSize);
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                var bytes = memoryStream.ToArray();
                previewUrl = $"data:{file.ContentType};base64,{Convert.ToBase64String(bytes)}";

                _logger.LogDebug("Generated preview for '{FileName}' ({Size} bytes)",
                    file.Name, file.Size);
            }
            catch (Exception ex)
            {
                // Preview is non-critical — log the failure and continue without it
                _logger.LogWarning(ex, "Failed to generate preview for '{FileName}'. Continuing without preview.",
                    file.Name);
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
