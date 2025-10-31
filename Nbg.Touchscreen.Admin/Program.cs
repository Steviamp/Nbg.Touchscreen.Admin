using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using Nbg.Touchscreen.Admin.Components;
using Nbg.Touchscreen.Admin.Data;
using System.Security.Claims;

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
        o.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

var app = builder.Build();

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
    if (user is null) return Results.Unauthorized();

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

    if (vr == PasswordVerificationResult.Failed) return Results.Unauthorized(); ;

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