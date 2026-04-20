# AGENTS.md — BlazorDemo Project Report

> Auto-generated project scan report.
> Last scanned: 2026-04-20

---

## 1. Solution Overview

| Key               | Value                                          |
| ----------------- | ---------------------------------------------- |
| **Solution**      | `BlazorDemo.sln` / `BlazorDemo.slnx`          |
| **Project**       | `BlazorDemo/BlazorDemo.csproj`                 |
| **Framework**     | .NET 10.0 (`net10.0`)                          |
| **App Type**      | Blazor Web App (Interactive Server)            |
| **Render Mode**   | Per-page `@rendermode InteractiveServer`       |
| **License**       | MIT — Htet Oo Naing (2026)                     |
| **Launch URLs**   | `https://localhost:7250` / `http://localhost:5151` |

---

## 2. Project Structure

```
BlazorDemo/                              ← solution root (open this in IDE)
├── BlazorDemo.sln                       ← primary solution file
├── BlazorDemo.slnx                      ← XML solution file
├── AGENTS.md                            ← this report
└── BlazorDemo/                          ← project folder
    ├── BlazorDemo.csproj
    ├── BlazorDemo.sln                   ← ⚠️ duplicate .sln (see issue #1)
    ├── Program.cs                       ← app entry point & middleware
    ├── Components/
    │   ├── App.razor                    ← root HTML document
    │   ├── Routes.razor                 ← router config
    │   ├── _Imports.razor               ← global usings
    │   ├── Layout/
    │   │   ├── MainLayout.razor         ← shell layout (sidebar + content)
    │   │   ├── MainLayout.razor.css
    │   │   ├── NavMenu.razor            ← sidebar navigation
    │   │   ├── NavMenu.razor.css
    │   │   ├── ReconnectModal.razor     ← SignalR reconnect UI
    │   │   ├── ReconnectModal.razor.css
    │   │   └── ReconnectModal.razor.js
    │   └── Pages/
    │       ├── Home.razor               ← ★ file upload page (interactive)
    │       ├── Counter.razor            ← template counter (interactive)
    │       ├── Weather.razor            ← template weather (stream rendering)
    │       ├── Error.razor              ← error page (static)
    │       └── NotFound.razor           ← 404 page (static)
    ├── Models/
    │   └── CustomerModel.cs             ← customer + file names model
    ├── Properties/
    │   └── launchSettings.json
    ├── wwwroot/
    │   ├── app.css                      ← global styles
    │   ├── css/site.css                 ← custom styles
    │   ├── favicon.png
    │   └── lib/bootstrap/               ← Bootstrap CSS
    ├── appsettings.json
    ├── appsettings.Development.json
    ├── LICENSE
    └── README.md
```

---

## 3. Build Status

```
✅  dotnet build → succeeded (net10.0)
✅  No compile errors or warnings
```

---

## 4. Page-by-Page Analysis

### 4.1 Home.razor (`/`) — File Upload

| Aspect              | Status |
| ------------------- | ------ |
| Render mode         | ✅ `@rendermode InteractiveServer` |
| InputFile binding   | ✅ `OnChange` wired to `OnInputFileChange` |
| Multi-file support  | ✅ up to 3 files |
| Size validation     | ✅ 3 MB per file |
| Duplicate detection | ✅ by name + size |
| Image preview       | ✅ base64 data URI |
| Upload to disk      | ✅ `wwwroot/uploads/{GUID}_{name}` |
| Error display       | ✅ alert list |
| Success display     | ✅ count message |

### 4.2 Counter.razor (`/counter`)

- ✅ `@rendermode InteractiveServer`
- Standard template — works correctly.

### 4.3 Weather.razor (`/weather`)

- ✅ `[StreamRendering]` attribute
- Standard template — static data, no interactivity needed.

### 4.4 Error.razor (`/Error`) & NotFound.razor (`/not-found`)

- ✅ Static rendering — correct for error/404 pages.

---

## 5. Program.cs Pipeline

```
Services:
  ✅ AddRazorComponents().AddInteractiveServerComponents()

Middleware (in order):
  ✅ UseExceptionHandler (non-dev)
  ✅ UseHsts (non-dev)
  ✅ UseStatusCodePagesWithReExecute("/not-found")
  ✅ UseHttpsRedirection
  ✅ UseAntiforgery
  ✅ MapStaticAssets
  ✅ MapRazorComponents<App>().AddInteractiveServerRenderMode()
```

---

## 6. Model

**`CustomerModel.cs`**

| Property    | Type           | Default   |
| ----------- | -------------- | --------- |
| `Id`        | `int`          | 0         |
| `UserName`  | `string`       | `"admin"` |
| `FirstName` | `string?`      | null      |
| `LastName`  | `string?`      | null      |
| `FileNames` | `List<string>?`| `[]`      |

---

## 7. Issues & Warnings

### Issue #1 — ⚠️ Duplicate `.sln` inside project folder

There are **two** `.sln` files:
- `BlazorDemo/BlazorDemo.sln` (root — correct)
- `BlazorDemo/BlazorDemo/BlazorDemo.sln` (inside project folder — **should not be here**)

The inner one has a different `SolutionGuid` and project GUID. This can confuse IDEs and tooling.

**Fix:** Delete `BlazorDemo/BlazorDemo/BlazorDemo.sln`.

---

### Issue #2 — ⚠️ Upload path uses `Directory.GetCurrentDirectory()`

```csharp
// Home.razor line 201
string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
```

`Directory.GetCurrentDirectory()` varies depending on how the app is launched (terminal vs IDE vs service). This can cause uploads to silently write to wrong locations.

**Fix:** Inject `IWebHostEnvironment` and use `WebRootPath`:

```csharp
@inject IWebHostEnvironment Environment

// then in UploadFiles():
string uploadFolder = Path.Combine(Environment.WebRootPath, "uploads");
```

---

### Issue #3 — ⚠️ SignalR MaximumReceiveMessageSize not configured

The default SignalR message size is **32 KB**. File uploads transfer data over SignalR in Blazor Server mode. Uploading files close to 3 MB may fail silently or throw.

**Fix:** Add to `Program.cs`:

```csharp
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddHubOptions(options =>
    {
        options.MaximumReceiveMessageSize = 10 * 1024 * 1024; // 10 MB
    });
```

---

### Issue #4 — ⚠️ No `.vscode/launch.json` or `tasks.json`

VS Code has no debug/launch configuration. This causes the "exited with code 0" behavior when trying to run/debug from VS Code.

**Fix:** Create `.vscode/launch.json` and `.vscode/tasks.json` in the solution root targeting the inner project.

---

### Issue #5 — ℹ️ Nullable warning in Home.razor

```csharp
// Line 160
string previewUrl = null;  // ← CS8600: nullable not annotated
```

Project has `<Nullable>enable</Nullable>`. Should be `string? previewUrl = null;`.

---

### Issue #6 — ℹ️ Redundant `@using` directives in Home.razor

```razor
@using Microsoft.AspNetCore.Components.Forms   ← already in _Imports.razor
@using System.IO                                ← already via ImplicitUsings
```

Not harmful, but unnecessary.

---

### Issue #7 — ℹ️ README references `dotnet test` but no test project exists

The README has a "Testing" section with `dotnet test`, but there is no test project in the solution.

---

### Issue #8 — ℹ️ External CDN dependency

`App.razor` loads Font Awesome from `cdnjs.cloudflare.com`. This means the icons won't render offline.

---

### Issue #9 — ℹ️ `CustomerModel.UserName` hardcoded to `"admin"`

The model defaults `UserName = "admin"` but there is no authentication or user context. This is fine for a demo but should be noted.

---

## 8. How to Run

```bash
# From solution root:
dotnet run --project BlazorDemo/BlazorDemo.csproj

# Or from inside the project folder:
cd BlazorDemo/BlazorDemo
dotnet run

# Then open: https://localhost:7250 or http://localhost:5151
```

**Important:** Do NOT run `dotnet run` from the solution root without `--project` — there is no executable at the solution level.

---

## 9. Priority Fix Summary

| Priority | Issue                              | Effort |
| -------- | ---------------------------------- | ------ |
| 🔴 High  | #3 SignalR message size limit      | 1 min  |
| 🔴 High  | #4 Missing VS Code launch config  | 2 min  |
| 🟡 Med   | #2 Upload path fragility           | 2 min  |
| 🟡 Med   | #1 Duplicate .sln file             | 1 min  |
| 🟢 Low   | #5 Nullable warning                | 1 min  |
| 🟢 Low   | #6 Redundant usings                | 1 min  |
| 🟢 Low   | #7 README test section             | 1 min  |
| 🟢 Low   | #8 CDN dependency                  | N/A    |
| 🟢 Low   | #9 Hardcoded UserName              | N/A    |

---

## 10. Tech Stack

- **Runtime:** .NET 10.0 (Preview)
- **Framework:** ASP.NET Core Blazor Web App
- **Rendering:** Interactive Server (SignalR)
- **CSS:** Bootstrap 5 + custom CSS
- **Icons:** Font Awesome 6 (CDN)
- **IDE:** VS Code / Windsurf

---

## 11. Coding Conventions

- **Nullable reference types:** enabled
- **Implicit usings:** enabled
- **File-scoped namespaces:** used in models
- **Component structure:** `@page` → `@rendermode` → `@using` → markup → `@code`
- **Naming:** PascalCase for public members, camelCase for private fields
- **Inner classes:** `FilePreview` defined inside `Home.razor` `@code` block
