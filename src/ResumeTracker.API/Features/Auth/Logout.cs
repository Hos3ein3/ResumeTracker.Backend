using Microsoft.AspNetCore.Mvc;

using ResumeTracker.API.Extensions;
using ResumeTracker.Application.DTOs.Auth;
using ResumeTracker.Application.Features.Auth;

namespace ResumeTracker.API.Features.Auth;

public static class Logout
{
    public static async Task<IResult> Handle(
        LogoutRequest request,
        [FromServices] IAuthService authService,
        HttpContext httpContext,
        CancellationToken ct)
    {
        var ip = GetIpAddress(httpContext);
        var result = await authService.LogoutAsync(request, ip, ct);
        return result.ToHttpResult();
    }

    private static string GetIpAddress(HttpContext ctx)
        => ctx.Request.Headers["X-Forwarded-For"].FirstOrDefault()
           ?? ctx.Connection.RemoteIpAddress?.ToString()
           ?? "unknown";
}