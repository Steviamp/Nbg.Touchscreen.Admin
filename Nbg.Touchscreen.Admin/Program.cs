using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using Nbg.Touchscreen.Admin.Components;
using Nbg.Touchscreen.Admin.Data;
using Nbg.Touchscreen.Admin.Data.helpers;
using Nbg.Touchscreen.Admin.Endpoints;
using Serilog;
using Serilog.Events;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddCircuitOptions(options => { options.DetailedErrors = true; });

builder.Services.AddMudServices();

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, o =>
    {
        o.LoginPath = "/login";
        o.AccessDeniedPath = "/forbidden";
        o.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        o.SlidingExpiration = false;
    });

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<HolidayService>();

builder.Services.AddScoped<ISoapService, SoapService>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


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

app.MapGet("/admin/api/logs", [Authorize] () =>
{
    if (!Directory.Exists(logsRoot))
        return Results.Ok(Array.Empty<object>());

    var files = Directory.GetFiles(logsRoot, "log-*.txt", SearchOption.TopDirectoryOnly)
        .OrderByDescending(f => f)
        .Select(f => new
        {
            Name = Path.GetFileName(f),
            SizeBytes = new FileInfo(f).Length,
            LastWriteUtc = File.GetLastWriteTimeUtc(f)
        });

    return Results.Ok(files);
});

app.MapGet("/admin/api/logs/{name}", [Authorize] (string name) =>
{
    if (string.IsNullOrWhiteSpace(name) || name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
        return Results.BadRequest();

    var full = Path.Combine(logsRoot, name);
    if (!full.StartsWith(logsRoot)) return Results.BadRequest();
    if (!File.Exists(full)) return Results.NotFound();

    return Results.File(full, "text/plain", fileDownloadName: name);
});


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapControllers();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapAuthEndpoints();

app.Run();
