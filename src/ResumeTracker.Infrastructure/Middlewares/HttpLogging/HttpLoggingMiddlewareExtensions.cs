using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ResumeTracker.Infrastructure.Middlewares.HttpLogging;

public static class HttpLoggingMiddlewareExtensions
{
    public static IServiceCollection AddHttpRequestResponseLogging(
        this IServiceCollection services,
        Action<HttpLoggingOptions>? configure = null)
    {
        var options = new HttpLoggingOptions();
        configure?.Invoke(options);

        // Register as singleton — options don't change per request
        services.AddSingleton(options);
        return services;
    }

    public static IApplicationBuilder UseHttpRequestResponseLogging(
        this IApplicationBuilder app)
    {
        var env = app.ApplicationServices
            .GetRequiredService<IHostEnvironment>();

        // In Production: log only errors to reduce noise and avoid leaking PII
        // In Development: log everything
        if (env.IsProduction())
        {
            app.UseMiddleware<HttpLoggingMiddleware>(
                new HttpLoggingOptions
                {
                    LogRequestBody = true,
                    LogResponseBody = true,
                    LogRequestHeaders = true,
                    LogResponseHeaders = false,
                    LogOnlyErrors = true,   // ← only 4xx/5xx in production
                    MaxBodySizeBytes = 8 * 1024
                });
        }
        else
        {
            var options = app.ApplicationServices
                .GetRequiredService<HttpLoggingOptions>();
            app.UseMiddleware<HttpLoggingMiddleware>(options);
        }

        return app;
    }
}