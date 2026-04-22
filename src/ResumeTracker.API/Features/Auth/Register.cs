using Microsoft.AspNetCore.Mvc;

using ResumeTracker.API.Extensions;
using ResumeTracker.Application.DTOs.Auth;
using ResumeTracker.Application.Features.Auth;
using ResumeTracker.Application.Features.UserPreferences;

namespace ResumeTracker.API.Features.Auth;

public static class Register
{
    public static async Task<IResult> Handle(
        RegisterRequest request,
        [FromServices] IAuthService authService,
        [FromServices] IUserPreferencesService userPreferencesService,
        HttpContext httpContext,
        CancellationToken ct)
    {
        var ip = GetIpAddress(httpContext);
        var result = await authService.RegisterAsync(request, ip, ct);



        return result.ToHttpResult();
    }

    private static string GetIpAddress(HttpContext ctx)
        => ctx.Request.Headers["X-Forwarded-For"].FirstOrDefault()
           ?? ctx.Connection.RemoteIpAddress?.ToString()
           ?? "unknown";
}