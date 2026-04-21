
using Microsoft.AspNetCore.Mvc;

using ResumeTracker.Application.DTOs.Auth;
using ResumeTracker.Application.Features.Auth;

namespace ResumeTracker.API.Features.Auth;

public static class Login
{
    public static async Task<IResult> Handle(
        LoginRequest request,
        [FromServices] IAuthService authService,
        HttpContext httpContext,
        CancellationToken ct)
    {
        var ip = GetIpAddress(httpContext);
        var result = await authService.LoginAsync(request, ip, ct);
        return result.ToHttpResult();
    }

    private static string GetIpAddress(HttpContext ctx)
        => ctx.Request.Headers["X-Forwarded-For"].FirstOrDefault()
           ?? ctx.Connection.RemoteIpAddress?.ToString()
           ?? "unknown";
}