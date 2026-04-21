namespace ResumeTracker.Infrastructure.Middlewares;

/// <summary>
/// Scoped accessor — inject this anywhere to read the current request's correlation context.
/// </summary>
public sealed class CorrelationContextAccessor
{
    public CorrelationContext? Current { get; internal set; }
}