namespace ResumeTracker.Infrastructure.Settings;

public sealed class RedisSettings
{
    public string ConnectionString { get; init; } = default!;
    public string InstanceName { get; init; } = "ResumeTracker:";
}