using System.Threading.RateLimiting;
using BlazorDemo.Components;
using BlazorDemo.Data;
using BlazorDemo.Models;
using BlazorDemo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddHubOptions(options =>
    {
        options.MaximumReceiveMessageSize = 10 * 1024 * 1024; // 10 MB
    });

// Bind UploadOptions from appsettings.json
builder.Services.Configure<UploadOptions>(
    builder.Configuration.GetSection(UploadOptions.SectionName));

// Register application services
builder.Services.AddScoped<IFileValidationService, FileValidationService>();
builder.Services.AddScoped<IFilePreviewService, FilePreviewService>();
builder.Services.AddScoped<IFileUploadService, FileUploadService>();

// Database for Identity
builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=BlazorDemo.db"));

// ASP.NET Core Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<IdentityDbContext>()
.AddDefaultTokenProviders();

// Authentication & Authorization
builder.Services.AddAuthentication();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanUploadFiles", policy =>
        policy.RequireClaim("Permission", "CanUploadFiles"));
});

// Rate limiting: 10 requests per 10 seconds per IP on the file-serving endpoint
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fileServing", limiter =>
    {
        limiter.PermitLimit = 10;
        limiter.Window = TimeSpan.FromSeconds(10);
        limiter.QueueLimit = 0;
    });
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Serve uploaded files from secure storage (outside wwwroot) - requires CanUploadFiles claim
app.MapGet("/uploads/{fileName}", [Authorize(Policy = "CanUploadFiles")] (string fileName, IWebHostEnvironment env, IOptions<UploadOptions> uploadOptions) =>
{
    // Prevent directory traversal
    if (fileName.Contains("..") || fileName.Contains('/') || fileName.Contains('\\'))
        return Results.BadRequest();

    var filePath = Path.Combine(env.ContentRootPath, uploadOptions.Value.UploadFolder, fileName);

    if (!File.Exists(filePath))
        return Results.NotFound();

    var extension = Path.GetExtension(fileName).ToLowerInvariant();
    var contentType = extension switch
    {
        ".jpg" or ".jpeg" => "image/jpeg",
        ".png" => "image/png",
        ".gif" => "image/gif",
        ".bmp" => "image/bmp",
        ".webp" => "image/webp",
        _ => "application/octet-stream"
    };

    return Results.File(filePath, contentType);
}).RequireRateLimiting("fileServing");

// Logout endpoint
app.MapPost("/logout", async (SignInManager<IdentityUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Redirect("/");
});

app.Run();

// Make Program class visible to integration tests
public partial class Program { }
