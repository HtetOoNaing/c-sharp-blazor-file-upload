using BlazorDemo.Models;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;

namespace BlazorDemo.Services;

/// <summary>
/// Handles server-side file storage operations.
/// Reads upload folder from UploadOptions in appsettings.json.
/// </summary>
public class FileUploadService : IFileUploadService
{
    private readonly string _uploadDirectory;

    public FileUploadService(IWebHostEnvironment environment, IOptions<UploadOptions> options)
    {
        ArgumentNullException.ThrowIfNull(environment);

        // Upload folder name comes from config (default: "uploads")
        _uploadDirectory = Path.Combine(environment.WebRootPath, options.Value.UploadFolder);
    }

    public string GenerateUniqueFileName(string originalFileName)
    {
        ArgumentException.ThrowIfNullOrEmpty(originalFileName);
        return $"{Guid.NewGuid()}_{originalFileName}";
    }

    public async Task<UploadResult> SaveFileAsync(IBrowserFile fileData, long maxFileSize)
    {
        ArgumentNullException.ThrowIfNull(fileData);

        try
        {
            EnsureUploadDirectoryExists();

            var newFileName = GenerateUniqueFileName(fileData.Name);
            var filePath = Path.Combine(_uploadDirectory, newFileName);

            await using var stream = fileData.OpenReadStream(maxFileSize);
            await using var fileStream = new FileStream(filePath, FileMode.Create);
            await stream.CopyToAsync(fileStream);

            return new UploadResult
            {
                Success = true,
                FileName = newFileName,
                StoredPath = filePath
            };
        }
        catch (Exception ex)
        {
            return new UploadResult
            {
                Success = false,
                ErrorMessage = $"Error uploading '{fileData.Name}': {ex.Message}"
            };
        }
    }

    public string GetUploadDirectoryPath()
    {
        return _uploadDirectory;
    }

    public void EnsureUploadDirectoryExists()
    {
        if (!Directory.Exists(_uploadDirectory))
        {
            Directory.CreateDirectory(_uploadDirectory);
        }
    }
}
