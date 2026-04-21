using System.Text.Json;

using HealthChecks.UI.Client;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ResumeTracker.Infrastructure.Configurations;

public static class HealthCheckConfiguration
{
    public static IServiceCollection AddHealthChecksInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
    {
        var pgConnection = configuration.GetConnectionString("DefaultConnection")!;
        var redisConnection = configuration["Redis:ConnectionString"]!;

        // ❌ DO NOT register IMongoClient here anymore — already registered in DependencyInjection.cs

        services
            .AddHealthChecks()
            .AddCheck(
                name: "self",
                check: () => HealthCheckResult.Healthy("Application is running."),
                tags: ["live"])
            .AddNpgSql(
                connectionString: pgConnection,
                name: "postgresql",
                failureStatus: HealthStatus.Unhealthy,
                tags: ["ready", "db", "postgresql"])
            .AddRedis(
                redisConnectionString: redisConnection,
                name: "redis",
                failureStatus: HealthStatus.Degraded,
                tags: ["ready", "cache", "redis"])
            .AddMongoDb(   // ✅ resolves IMongoClient from DI — already registered above
                name: "mongodb",
                failureStatus: HealthStatus.Degraded,
                tags: ["ready", "files", "mongodb"]);

        services
            .AddHealthChecksUI(options =>
            {
                options.SetEvaluationTimeInSeconds(30);
                options.MaximumHistoryEntriesPerEndpoint(50);
                options.AddHealthCheckEndpoint("ResumeTracker API", "/health");
            })
            .AddInMemoryStorage();

        return services;
    }

    public static IApplicationBuilder UseHealthCheckEndpoints(this IApplicationBuilder app)
    {
        // Liveness — process alive? (no external deps)
        app.UseHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("live"),
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        // Readiness — all dependencies reachable?
        app.UseHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready"),
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        // Full report — consumed by Health Checks UI dashboard
        app.UseHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        // Health Checks UI dashboard — available at /health-ui
        app.UseHealthChecksUI(options =>
        {
            options.UIPath = "/health-ui";
            options.ApiPath = "/health-ui-api";
        });

        return app;
    }
    private static async Task WriteJsonResponse(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";

        var response = new
        {
            status = report.Status.ToString(),
            duration = report.TotalDuration.TotalMilliseconds,
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                duration = e.Value.Duration.TotalMilliseconds,
                tags = e.Value.Tags,
                error = e.Value.Exception?.Message
            })
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                WriteIndented = true
            }));
    }
}