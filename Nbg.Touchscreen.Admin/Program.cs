using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using Nbg.Touchscreen.Admin.Components;
using Nbg.Touchscreen.Admin.Data;
using System.Security.Claims;
using Serilog;
using Serilog.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddCircuitOptions(options =>
    {
        options.DetailedErrors = true;
    });

builder.Services.AddMudServices();

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, o =>
    {
        o.LoginPath = "/login";
        o.AccessDeniedPath = "/forbidden";
        o.ExpireTimeSpan = TimeSpan.FromMinutes(30); // αυτόματο logout μετά από 30 λεπτά
        o.SlidingExpiration = false;
    });

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

var logsPath = Path.Combine(AppContext.BaseDirectory, "App_Data", "logs");
Directory.CreateDirectory(logsPath);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .WriteTo.Console()
    .WriteTo.File(
        Path.Combine(logsPath, "log-.txt"),
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 14,
        fileSizeLimitBytes: 10_000_000,
        rollOnFileSizeLimit: true,
        shared: true,
        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}"
    )
    .CreateLogger();

builder.Host.UseSerilog();
var app = builder.Build();

var logsRoot = Path.Combine(AppContext.BaseDirectory, "App_Data", "logs");

// Λίστα αρχείων log
app.MapGet("/admin/api/logs", [Authorize] () =>
{
    if (!Directory.Exists(logsRoot))
        return Results.Ok(Array.Empty<object>());

    var files = Directory.GetFiles(logsRoot, "log-*.txt", SearchOption.TopDirectoryOnly)
        .OrderByDescending(f => f)
        .Select(f => new {
            Name = Path.GetFileName(f),
            SizeBytes = new FileInfo(f).Length,
            LastWriteUtc = File.GetLastWriteTimeUtc(f)
        });

    return Results.Ok(files);
});

// Download συγκεκριμένου log
app.MapGet("/admin/api/logs/{name}", [Authorize] (string name) =>
{
    // basic sanitization
    if (string.IsNullOrWhiteSpace(name) || name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
        return Results.BadRequest();

    var full = Path.Combine(logsRoot, name);
    if (!full.StartsWith(logsRoot)) return Results.BadRequest(); // ασφάλεια path traversal
    if (!System.IO.File.Exists(full)) return Results.NotFound();

    return Results.File(full, "text/plain", fileDownloadName: name);
});
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapPost("/auth/login", async ([FromForm] LoginDto dto, AppDbContext db,
                                  HttpContext http, IPasswordHasher<User> hasher) =>
{
    var user = await db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email && u.IsActive);
    if (user is null) return Results.Redirect("/login?error=invalid");

    // 1) Δοκίμασε κανονικό verify σε hashed password
    var vr = PasswordVerificationResult.Failed;
    try { vr = hasher.VerifyHashedPassword(user, user.Password, dto.Password); } catch { }

    // 2) Αν αποτύχει, υποστήριξε legacy: plain match => κάνε άμεσο upgrade σε hash
    if (vr == PasswordVerificationResult.Failed && user.Password == dto.Password)
    {
        user.Password = hasher.HashPassword(user, dto.Password); // migrate σε hash
        await db.SaveChangesAsync();
        vr = PasswordVerificationResult.Success;
    }

    if (vr == PasswordVerificationResult.Failed) return Results.Redirect("/login?error=invalid");

    // 3) Claims + Cookie sign-in
    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, string.IsNullOrWhiteSpace(user.Name) ? user.Email : user.Name),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role ?? "Viewer")
    };
    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    await http.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

    // 4) Redirect μετά το login
    var returnUrl = string.IsNullOrWhiteSpace(dto.ReturnUrl) ? "/" : dto.ReturnUrl!;
    return Results.Redirect(returnUrl);
});

app.MapGet("/auth/logout", async (HttpContext http) =>
{
    await http.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/login");   // browser redirect
})
.DisableAntiforgery();

app.Run();

public record LoginDto(string Email, string Password, string? ReturnUrl);