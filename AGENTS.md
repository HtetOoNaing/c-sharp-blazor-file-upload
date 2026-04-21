# AGENTS.md

Rules and constraints for AI agents working on this project.

---

## Pinned Versions — Do NOT Upgrade Without Asking

| Dependency          | Version   |
| ------------------- | --------- |
| .NET SDK            | 10.0      |
| ASP.NET Core Blazor | 10.0      |
| Bootstrap           | 5.3.3     |
| Font Awesome        | 6.0.0     |
| Entity Framework Core | 8.0.4   |

- Do NOT add NuGet packages without explicit approval.
- Do NOT modify the authentication flow without explicit approval.

---

## Rules

### General
- This is a **learning/demo project**. Keep code simple and readable.
- The owner is not deeply familiar with C# / .NET / Blazor — explain non-obvious changes.
- Do NOT delete or weaken existing functionality without asking.
- Do NOT refactor working code unless asked.
- Prefer minimal, targeted fixes over large rewrites.

### Framework & Rendering
- Use **`@rendermode InteractiveServer`** on any page that needs interactivity.
- Pages that are display-only (e.g., Error, NotFound) stay **static**.
- Weather page uses **`[StreamRendering]`** — keep it that way.

### File Structure
- Entry point: `Program.cs`
- Pages: `Components/Pages/` with `@page` directive
- Layouts: `Components/Layout/`
- Models and config classes: `Models/`
- Services: `Services/`
- Custom exceptions: `Exceptions/`
- Static files: `wwwroot/`
- Global usings: `Components/_Imports.razor`

### Coding Conventions
- **Nullable reference types**: enabled — use `string?` not `string` for nullable
- **Implicit usings**: enabled — do NOT add `@using System.IO` or other implicit ones
- **File-scoped namespaces**: use `namespace Foo;` not `namespace Foo { }`
- **Component order**: `@page` → `@rendermode` → `@using` → `@inject` → markup → `@code`
- **Naming**: PascalCase for public members, camelCase for private fields
- **No `Console.WriteLine`** in components — use `ILogger<T>` for logging.
- **Error handling**: Use custom exceptions (`FileUploadException`, `FileValidationException`) in services; catch them in components to show user-friendly messages.

### Authentication
- Home page requires `[Authorize(Policy = "CanUploadFiles")]` claim.
- Login/Register pages are accessible anonymously.
- File serving endpoint requires `CanUploadFiles` claim.
- Users receive `CanUploadFiles` claim on registration.

### Data Protection
- File metadata (original file names) encrypted using `IDataProtector`.
- CORS policy "StrictPolicy" configured for API endpoints.
- Anti-forgery tokens enabled via `UseAntiforgery()`.

### File Uploads
- Upload destination: `App_Data/uploads/` (outside wwwroot, served via `/uploads/{fileName}` endpoint)
- Filename format: `{Guid}_{originalName}`
- Max file size: 3 MB per file
- Max file count: 3
- Allowed image extensions: `.jpg`, `.jpeg`, `.png`, `.gif`, `.bmp`, `.webp`

### Commands
- **Run**: `dotnet run` from `BlazorDemo/`
- **Build**: `dotnet build` from `BlazorDemo/`
- **Test**: `dotnet test` from `BlazorDemo.Tests/`
