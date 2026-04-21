
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

using Serilog;
using Serilog.Events;

namespace ResumeTracker.Infrastructure.Configurations;

public static class SerilogConfiguration
{
    /// <summary>
    /// Stage 1 — Bootstrap logger.
    /// Captures fatal startup errors BEFORE appsettings.json is loaded.
    /// Writes only to Console so nothing is lost during host construction.
    /// </summary>
    public static void CreateBootstrapLogger()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateBootstrapLogger();   // ← ReloadableLogger — replaced at stage 2
    }

    /// <summary>
    /// Stage 2 — Full logger.
    /// Called inside AddSerilog() — has access to IConfiguration and DI services.
    /// Reads full config from appsettings.json Serilog section.
    /// Only the Seq sink is active in all environments.
    /// Console and File sinks are declared in config but gated per environment.
    /// </summary>
    public static void ConfigureLogger(
        HostBuilderContext context,
        IServiceProvider services,
        LoggerConfiguration loggerConfig)
    {
        loggerConfig
            // ── Read everything from appsettings.json ──────────
            .ReadFrom.Configuration(context.Configuration)

            // ── Inject DI-aware enrichers (ILogEventEnricher) ──
            .ReadFrom.Services(services)

            // ── Enrichers ──────────────────────────────────────
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .Enrich.WithThreadId()
            .Enrich.WithProcessId()
              .Enrich.WithProcessName()
            .Enrich.WithProperty("Application", "ResumeTracker")
            .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName);
    }

    /// <summary>
    /// Adds UseSerilogRequestLogging with a customized message template.
    /// Call this in the middleware pipeline.
    /// </summary>
    public static IApplicationBuilder UseStructuredRequestLogging(this IApplicationBuilder app)
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate =
                "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

            // Enrich each request log with useful context
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.ToString());
                diagnosticContext.Set("UserId", httpContext.User.FindFirst("sub")?.Value ?? "anonymous");
                diagnosticContext.Set("RemoteIp", httpContext.Connection.RemoteIpAddress?.ToString());
            };
        });

        return app;
    }
}