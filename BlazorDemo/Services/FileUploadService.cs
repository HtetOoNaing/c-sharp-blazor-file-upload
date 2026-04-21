using BlazorDemo.Exceptions;
using BlazorDemo.Models;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;

namespace BlazorDemo.Services;

public class FileUploadService : IFileUploadService
{
    private readonly string _uploadDirectory;
    private readonly ILogger<FileUploadService> _logger;

    public FileUploadService(
        IWebHostEnvironment environment,
        IOptions<UploadOptions> options,
        ILogger<FileUploadService> logger)
    {
        ArgumentNullException.ThrowIfNull(environment);

        _logger = logger;
        _uploadDirectory = Path.Combine(environment.ContentRootPath, options.Value.UploadFolder);
    }

    public string GenerateUniqueFileName(string originalFileName)
    {
        ArgumentException.ThrowIfNullOrEmpty(originalFileName);
        return $"{Guid.NewGuid()}_{originalFileName}";
    }

    public async Task<UploadResult> SaveFileAsync(IBrowserFile fileData, long maxFileSize)
    {
        ArgumentNullException.ThrowIfNull(fileData);

        _logger.LogInformation("Starting upload for file '{FileName}' ({FileSize} bytes)",
            fileData.Name, fileData.Size);

        try
        {
            EnsureUploadDirectoryExists();

            var newFileName = GenerateUniqueFileName(fileData.Name);
            var filePath = Path.Combine(_uploadDirectory, newFileName);

            await using var stream = fileData.OpenReadStream(maxFileSize);
            await using var fileStream = new FileStream(filePath, FileMode.Create);
            await stream.CopyToAsync(fileStream);

            _logger.LogInformation("Successfully uploaded '{FileName}' as '{StoredName}'",
                fileData.Name, newFileName);

            return new UploadResult
            {
                Success = true,
                FileName = newFileName,
                StoredPath = filePath
            };
        }
        catch (IOException ex)
        {
            _logger.LogError(ex, "Disk I/O error uploading '{FileName}' to '{Directory}'",
                fileData.Name, _uploadDirectory);

            throw new FileUploadException(
                $"Failed to save '{fileData.Name}'. Please try again.",
                fileData.Name,
                ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error uploading '{FileName}'", fileData.Name);

            throw new FileUploadException(
                $"An unexpected error occurred uploading '{fileData.Name}'.",
                fileData.Name,
                ex);
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
            _logger.LogInformation("Creating upload directory: {Directory}", _uploadDirectory);
            Directory.CreateDirectory(_uploadDirectory);
        }
    }
}
