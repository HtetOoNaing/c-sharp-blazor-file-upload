namespace BlazorDemo.Exceptions;

public class FileUploadException : Exception
{
    public string? FileName { get; }

    public FileUploadException(string message)
        : base(message) { }

    public FileUploadException(string message, string fileName)
        : base(message)
    {
        FileName = fileName;
    }

    public FileUploadException(string message, string fileName, Exception innerException)
        : base(message, innerException)
    {
        FileName = fileName;
    }
}
