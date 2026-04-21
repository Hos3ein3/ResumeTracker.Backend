namespace ResumeTracker.Infrastructure.Middlewares;

/// <summary>
/// Immutable bag of tracing identifiers attached to the current request.
/// Registered as Scoped — one instance per HTTP request.
/// </summary>
public sealed class CorrelationContext
{
    /// <summary>Unique ID for this specific HTTP request. Generated server-side if not provided.</summary>
    public string RequestId { get; init; } = default!;

    /// <summary>
    /// Shared ID that links multiple requests belonging to the same logical operation.
    /// Passed by the caller via X-Correlation-Id header; generated if absent.
    /// </summary>
    public string CorrelationId { get; init; } = default!;

    /// <summary>Identifies the client application (mobile app, web, etc.). From X-Client-Id header.</summary>
    public string? ClientId { get; init; }

    /// <summary>Identifies which API gateway or reverse proxy forwarded the request. From X-Gateway-Id header.</summary>
    public string? GatewayId { get; init; }

    /// <summary>Authenticated user's subject claim. Null for anonymous requests.</summary>
    public string? UserId { get; init; }
}