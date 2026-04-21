# AGENTS.md

Rules and constraints for AI agents working on this project.

---

## Project Identity

- **Name**: BlazorDemo
- **Type**: Blazor Web App (Interactive Server)
- **Git root**: This folder (`BlazorDemo/`) — there is no outer solution folder in the repo

---

## Pinned Versions — Do NOT Upgrade Without Asking

| Dependency          | Version   | Location                          |
| ------------------- | --------- | --------------------------------- |
| .NET SDK            | 10.0      | `BlazorDemo.csproj` (`net10.0`)   |
| ASP.NET Core Blazor | 10.0      | Ships with .NET SDK               |
| Bootstrap           | 5.3.3     | `wwwroot/lib/bootstrap/`          |
| Font Awesome        | 6.0.0     | `wwwroot/lib/font-awesome/css/all.min.css` (local, was CDN)|

- No additional NuGet packages. Everything is built-in to the .NET SDK.
- Do NOT add NuGet packages without explicit approval.

---

## Rules

### General
- This is a **learning/demo project**. Keep code simple and readable.
- The owner is not deeply familiar with C# / .NET / Blazor — explain non-obvious changes.
- Do NOT delete or weaken existing functionality without asking.
- Do NOT refactor working code unless asked.
- Prefer minimal, targeted fixes over large rewrites.

### Framework & Rendering
- Use **`@rendermode InteractiveServer`** on any page that needs interactivity (event handlers, forms, etc.).
- Pages that are display-only (e.g., Error, NotFound) stay **static** — no render mode directive.
- Weather page uses **`[StreamRendering]`** — keep it that way.

### File Structure
- Entry point: `Program.cs`
- All UI components go in `Components/`
- Pages go in `Components/Pages/` with `@page` directive
- Layouts go in `Components/Layout/`
- Data models and config classes go in `Models/`
- Service interfaces and implementations go in `Services/`
- Custom exception types go in `Exceptions/`
- Static files (CSS, images, uploads) go in `wwwroot/`
- Global `@using` statements go in `Components/_Imports.razor`

### Coding Conventions
- **Nullable reference types**: enabled — use `string?` not `string` for nullable
- **Implicit usings**: enabled — do NOT add `@using System.IO` or other implicit ones
- **File-scoped namespaces**: use `namespace Foo;` not `namespace Foo { }`
- **Component order**: `@page` → `@rendermode` → `@using` → `@inject` → markup → `@code`
- **Naming**: PascalCase for public members, camelCase for private fields
- **No `Console.WriteLine`** in components — it goes to server console, not browser. Use `ILogger<T>` for logging.
- **Error handling**: Use custom exceptions (`FileUploadException`, `FileValidationException`) in services; catch them in components to show user-friendly messages.

### File Uploads
- Upload destination: `wwwroot/uploads/`
- Filename format: `{Guid}_{originalName}`
- Max file size: 3 MB per file
- Max file count: 3
- Allowed image extensions: `.jpg`, `.jpeg`, `.png`, `.gif`, `.bmp`, `.webp`

### How to Run
```bash
# From this project folder:
dotnet run

# Then open: https://localhost:7250 or http://localhost:5151
```

### How to Build
```bash
dotnet build
```

### How to Test
```bash
cd BlazorDemo.Tests  # or: dotnet test BlazorDemo.Tests/BlazorDemo.Tests.csproj
dotnet test
```

**Test Structure:**
- **UnitTests/** - Fast, isolated tests for models, services, and validation logic
  - `CustomerModelTests.cs` - Tests for CustomerModel properties
  - `FileValidationTests.cs` - Tests for image validation, file size, file naming
  - `FileValidationServiceTests.cs` - Tests for FileValidationService via IOptions
  - `ExceptionTests.cs` - Tests for custom exception types
- **IntegrationTests/** - Tests that run the actual Blazor app
  - `BlazorAppIntegrationTests.cs` - HTTP endpoint tests using WebApplicationFactory

**Current Test Count:** 79 tests (all passing)

---

## Known Issues (Do NOT Re-Report)

These are known and tracked. Fix only if explicitly asked.

1. ~~Upload path uses `Directory.GetCurrentDirectory()` — should use `IWebHostEnvironment.WebRootPath`~~ ✅ **DONE**
2. ~~SignalR `MaximumReceiveMessageSize` not configured — default 32KB may limit large uploads~~ ✅ **DONE** (set to 10MB)
3. ~~`string previewUrl = null;` should be `string? previewUrl = null;`~~ ✅ **DONE**
4. ~~Redundant `@using` directives in `Home.razor` (already in `_Imports.razor`)~~ ✅ **DONE**
5. ~~Font Awesome loaded from CDN — icons won't work offline~~ ✅ **DONE** (now local at `wwwroot/lib/font-awesome/`)
6. ~~`CustomerModel.UserName` hardcoded to `"admin"` — no auth system yet~~ ✅ **DONE** (now `string? UserName` with no default)
7. ~~README references `dotnet test` but no test project exists~~ ✅ **DONE** (BlazorDemo.Tests project created with 42 tests)
