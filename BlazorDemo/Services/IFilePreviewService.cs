using Microsoft.AspNetCore.Components.Forms;

namespace BlazorDemo.Services;

public interface IFilePreviewService
{
    Task<FilePreviewDto> CreatePreviewAsync(IBrowserFile file, long maxFileSize);
}
