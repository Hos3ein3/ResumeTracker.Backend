

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ResumeTracker.Infrastructure.Extensions;

public static class ClaimsPrincipalExtensions
{
    // ── Identity ──────────────────────────────────────────────────

    public static Guid? GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(JwtRegisteredClaimNames.Sub)
                    ?? user.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(value, out var id) ? id : null;
    }

    /// <summary>Returns UserId or throws if missing — use in endpoints that RequireAuthorization.</summary>
    public static Guid GetRequiredUserId(this ClaimsPrincipal user)
        => user.GetUserId()
           ?? throw new UnauthorizedAccessException(
               "User identity could not be resolved from the token.");

    public static string? GetEmail(this ClaimsPrincipal user)
        => user.FindFirstValue(JwtRegisteredClaimNames.Email)
           ?? user.FindFirstValue(ClaimTypes.Email);

    public static string? GetFirstName(this ClaimsPrincipal user)
        => user.FindFirstValue("firstName");

    public static string? GetLastName(this ClaimsPrincipal user)
        => user.FindFirstValue("lastName");

    public static string? GetFullName(this ClaimsPrincipal user)
    {
        var first = user.GetFirstName();
        var last = user.GetLastName();

        return (first, last) switch
        {
            (null, null) => null,
            (null, _) => last,
            (_, null) => first,
            _ => $"{first} {last}"
        };
    }

    public static string? GetJwtId(this ClaimsPrincipal user)
        => user.FindFirstValue(JwtRegisteredClaimNames.Jti);

    // ── Roles ─────────────────────────────────────────────────────

    public static IReadOnlyList<string> GetRoles(this ClaimsPrincipal user)
        => user.FindAll(ClaimTypes.Role)
               .Select(c => c.Value)
               .ToList();

    public static bool IsInAnyRole(this ClaimsPrincipal user, params string[] roles)
        => roles.Any(user.IsInRole);

    public static bool IsAdmin(this ClaimsPrincipal user)
        => user.IsInRole("Admin");

    public static bool IsUser(this ClaimsPrincipal user)
        => user.IsInRole("User");

    // ── Auth state ────────────────────────────────────────────────

    public static bool IsAuthenticated(this ClaimsPrincipal user)
        => user.Identity?.IsAuthenticated is true;

    public static bool HasClaim(this ClaimsPrincipal user, string type, string value)
        => user.HasClaim(type, value);

    // ── Token metadata ────────────────────────────────────────────

    public static DateTime? GetTokenIssuedAt(this ClaimsPrincipal user)
    {
        var iat = user.FindFirstValue(JwtRegisteredClaimNames.Iat);
        if (iat is null || !long.TryParse(iat, out var seconds))
            return null;

        return DateTimeOffset.FromUnixTimeSeconds(seconds).UtcDateTime;
    }

    public static DateTime? GetTokenExpiry(this ClaimsPrincipal user)
    {
        var exp = user.FindFirstValue(JwtRegisteredClaimNames.Exp);
        if (exp is null || !long.TryParse(exp, out var seconds))
            return null;

        return DateTimeOffset.FromUnixTimeSeconds(seconds).UtcDateTime;
    }

    public static bool IsTokenExpired(this ClaimsPrincipal user)
    {
        var expiry = user.GetTokenExpiry();
        return expiry is not null && DateTime.UtcNow >= expiry;
    }
}