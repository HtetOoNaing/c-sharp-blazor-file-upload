namespace BlazorDemo.Exceptions;

public class FileValidationException : Exception
{
    public string? FileName { get; }

    public FileValidationException(string message)
        : base(message) { }

    public FileValidationException(string message, string fileName)
        : base(message)
    {
        FileName = fileName;
    }

    public FileValidationException(string message, string fileName, Exception innerException)
        : base(message, innerException)
    {
        FileName = fileName;
    }
}
