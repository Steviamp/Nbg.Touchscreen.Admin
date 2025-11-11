using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Nbg.Touchscreen.Admin.Data;

namespace Nbg.Touchscreen.Admin.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        // -------- LOGIN --------
        app.MapPost("/auth/login", async (
            [FromForm] LoginDto dto,
            AppDbContext db,
            HttpContext http,
            IPasswordHasher<User> hasher) =>
        {
            var user = await db.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email && u.IsActive);

            if (user is null)
                return Results.Redirect("/login?error=invalid");

            var vr = PasswordVerificationResult.Failed;
            try
            {
                vr = hasher.VerifyHashedPassword(user, user.Password, dto.Password);
            }
            catch
            {
                // αν κάτι πάει στραβά, το αφήνουμε ως Failed
            }

            // Legacy υποστήριξη: αν το password είναι plain-text και ταιριάζει
            if (vr == PasswordVerificationResult.Failed && user.Password == dto.Password)
            {
                user.Password = hasher.HashPassword(user, dto.Password); // migrate σε hash
                await db.SaveChangesAsync();
                vr = PasswordVerificationResult.Success;
            }

            if (vr == PasswordVerificationResult.Failed)
                return Results.Redirect("/login?error=invalid");

            // 3) Claims + cookie sign-in
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name,
                    string.IsNullOrWhiteSpace(user.Name) ? user.Email : user.Name),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, user.Role ?? "Viewer")
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            await http.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            // 4) Redirect μετά το login
            var returnUrl = string.IsNullOrWhiteSpace(dto.ReturnUrl)
                ? "/"
                : dto.ReturnUrl!;

            return Results.Redirect(returnUrl);
        });

        // -------- LOGOUT --------
        app.MapGet("/auth/logout", async (HttpContext http) =>
        {
            await http.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Results.Redirect("/login");
        })
        .DisableAntiforgery();

        // -------- FORGOT PASSWORD --------
        app.MapPost("/auth/forgot-password", async (
            [FromForm] ForgotPasswordDto dto,
            AppDbContext db) =>
        {
            var email = dto.Email?.Trim().ToLowerInvariant();

            if (!string.IsNullOrWhiteSpace(email))
            {
                var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email && u.IsActive);
                if (user != null)
                {
                    // TODO: εδώ αργότερα θα βάλουμε token + email send
                    Serilog.Log.Information("Password reset requested for {Email}", email);
                }
            }

            return Results.Redirect("/forgot-password?status=ok");
        });
    }

    public record LoginDto(string Email, string Password, string? ReturnUrl);
    public record ForgotPasswordDto(string Email);
}

