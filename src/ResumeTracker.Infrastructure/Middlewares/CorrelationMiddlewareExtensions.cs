using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ResumeTracker.Infrastructure.Middlewares;

public static class CorrelationMiddlewareExtensions
{
    /// <summary>Registers scoped services needed by the middleware.</summary>
    public static IServiceCollection AddCorrelationContext(this IServiceCollection services)
    {
        services.AddScoped<CorrelationContextAccessor>();
        return services;
    }

    /// <summary>
    /// Adds CorrelationMiddleware to the pipeline.
    /// Must be placed BEFORE UseSerilogRequestLogging and UseAuthentication
    /// so that UserId is available from JWT claims.
    /// 
    /// IMPORTANT: Place AFTER UseAuthentication if you want UserId populated from JWT.
    /// If you need CorrelationId/RequestId on unauthenticated requests too, keep it before.
    /// Best order: UseCorrelationContext → UseAuthentication → UseSerilogRequestLogging
    /// </summary>
    public static IApplicationBuilder UseCorrelationContext(this IApplicationBuilder app)
        => app.UseMiddleware<CorrelationMiddleware>();
}