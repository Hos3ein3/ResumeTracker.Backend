using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

using Serilog.Context;

namespace ResumeTracker.Infrastructure.Middlewares;

public sealed class CorrelationMiddleware
{
    // ── Header name constants ──────────────────────────────
    public const string CorrelationIdHeader = "X-Correlation-Id";
    public const string ClientIdHeader = "X-Client-Id";
    public const string GatewayIdHeader = "X-Gateway-Id";
    public const string RequestIdHeader = "X-Request-Id";

    private readonly RequestDelegate _next;

    public CorrelationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        // ── 1. Extract or generate each identifier ─────────
        var requestId = httpContext.Request.Headers[RequestIdHeader].FirstOrDefault()
                        ?? httpContext.TraceIdentifier
                        ?? Guid.CreateVersion7().ToString();

        var correlationId = httpContext.Request.Headers[CorrelationIdHeader].FirstOrDefault()
                            ?? Guid.CreateVersion7().ToString();

        var clientId = httpContext.Request.Headers[ClientIdHeader].FirstOrDefault();
        var gatewayId = httpContext.Request.Headers[GatewayIdHeader].FirstOrDefault();
        var userId = httpContext.User.FindFirst("sub")?.Value
                        ?? httpContext.User.FindFirst(
                               System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        // ── 2. Build the scoped context object ─────────────
        var context = new CorrelationContext
        {
            RequestId = requestId,
            CorrelationId = correlationId,
            ClientId = clientId,
            GatewayId = gatewayId,
            UserId = userId
        };

        // ── 3. Register in DI so handlers/services can inject it ──
        httpContext.RequestServices
            .GetRequiredService<CorrelationContextAccessor>()
            .Current = context;

        // ── 4. Echo correlation headers back to the caller ─
        httpContext.Response.OnStarting(() =>
        {
            httpContext.Response.Headers[CorrelationIdHeader] = correlationId;
            httpContext.Response.Headers[RequestIdHeader] = requestId;
            return Task.CompletedTask;
        });

        // ── 5. Push all identifiers into Serilog LogContext ─
        //    Every log line emitted during this request will carry these properties in Seq
        using (LogContext.PushProperty("RequestId", requestId))
        using (LogContext.PushProperty("CorrelationId", correlationId))
        using (LogContext.PushProperty("ClientId", clientId ?? "unknown"))
        using (LogContext.PushProperty("GatewayId", gatewayId ?? "unknown"))
        using (LogContext.PushProperty("UserId", userId ?? "anonymous"))
        {
            await _next(httpContext);
        }
    }
}