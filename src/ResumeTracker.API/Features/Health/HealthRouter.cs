using ResumeTracker.API.Extensions;



namespace ResumeTracker.API.Features.Health;

public sealed class HealthRouter 
{
    public void RegisterRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/health")
            .WithTags("Health")
            .WithOpenApi();

        group.MapGet("/ping", Ping)
            .WithName("Ping")
            .WithSummary("Ping the API")
            .WithDescription("Returns a simple pong response to verify the API is reachable.")
            .AllowAnonymous();

        group.MapGet("/info", Info)
            .WithName("AppInfo")
            .WithSummary("Application info")
            .WithDescription("Returns application name, version, and current server UTC time.")
            .AllowAnonymous();
    }

    private static IResult Ping() =>
        Results.Ok(new { message = "pong", timestamp = DateTime.UtcNow });

    private static IResult Info() =>
        Results.Ok(new
        {
            application = "ResumeTracker API",
            version = "1.0.0",
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
            serverTime = DateTime.UtcNow
        });
}