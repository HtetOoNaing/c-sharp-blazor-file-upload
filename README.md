# Blazor File Upload Demo

A comprehensive demonstration of file upload functionality in Blazor Server, showcasing file filtering, size limitations, multi-file upload, and progress tracking.

## Features

- **Multi-file upload** - Select and upload multiple files at once
- **File type filtering** - Restrict uploads to specific file types
- **Size limitation** - Configurable maximum file size (default: 3MB per file)
- **Upload progress** - Visual feedback during file upload
- **Error handling** - Clear error messages for validation failures
- **Success confirmation** - List of successfully uploaded files

## Prerequisites

- [.NET 10.0 or later](https://dotnet.microsoft.com/download)
- Visual Studio 2022 / VS Code / JetBrains Rider
- Git (optional)

## Installation

1. **Clone the repository**
    ```bash
    git clone https://github.com/{{yourusername}}/c-sharp-blazor-file-upload.git
    cd c-sharp-blazor-file-upload
    ```
2. **Restore dependencies**
    ```bash
    dotnet restore
    ```
3. **Build the project**
    ```bash
    dotnet build
    ```
4. **Run the application**
    ```bash
    dotnet run
    ```
5. **Navigate to the application**
    ```bash
    https://localhost:5001
    ```

## Project Structure
```bash
BlazorFileUploadDemo/
├── Components/
│   ├── Layout/
│   │   └── MainLayout.razor
│   │   └── MainLayout.razor.css
│   │   └── NavMenu.razor
│   │   └── NavMenu.razor.css
│   │   └── ReconnectModal.razor
│   │   └── ReconnectModal.razor.css
│   │   └── ReconnectModal.razor.js
│   ├── Pages/
│   │   └── Home.razor
│   │   └── NotFound.razor
│   └── Shared/
│   └── App.razor
│   └── Routes.razor
├── wwwroot/
│   └── uploads/
├── Program.cs
├── appsettings.json
└── README.md
```

## File Size Limits
```bash
Setting	Default	Maximum	Location
Per file size	3 MB	10 MB	Home.razor
Max files	3	Unlimited	Home.razor
SignalR message	10 MB	Configurable	Program.cs
```

## Troubleshooting

1. **Files not uploading**
* Check wwwroot/uploads folder exists and is writable
* Verify SignalR message size limits in Program.cs
* Check browser console for errors (F12)

2. **Large files failing**
* Increase MaximumReceiveMessageSize in Program.cs
* Adjust maxFileSize in Home.razor

3. **No console output**
* Use ILogger instead of Console.WriteLine
* Check debug output window in Visual Studio

## Performance Considerations
* Maximum file size: Consider server memory limits
* Concurrent uploads: Implement throttling for multiple large files
* Storage: Implement cleanup routines for old files
* Security: Validate file types and scan for malware

## Security Recommendations
* Always validate file types (not just by extension)
* Implement virus scanning for production environments
* Store files outside wwwroot for sensitive data
* Use GUID filenames to prevent overwrites
* Implement authentication/authorization


## Testing
**Run the tests:**
```bash
dotnet test
```

## License
This project is licensed under the MIT License - see the LICENSE file for details.

## Contributing
* Fork the repository
* Create your feature branch (git checkout -b feature/AmazingFeature)
* Commit your changes (git commit -m 'Add some AmazingFeature')
* Push to the branch (git push origin feature/AmazingFeature)
* Open a Pull Request

## Contact
Htet Oo Naing (R Rex) - htetoonaing.ycc@gmail.com

Project Link: https://github.com/HtetOoNaing/c-sharp-blazor-file-upload

## Acknowledgments
[Blazor Documentation](https://learn.microsoft.com/en-us/aspnet/core/blazor)

[InputFile Component Documentation](https://learn.microsoft.com/en-us/aspnet/core/blazor/file-uploads)