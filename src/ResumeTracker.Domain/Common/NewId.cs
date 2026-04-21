namespace ResumeTracker.Domain.Common;

/// <summary>
/// Generates time-sortable UUID v7 identifiers.
/// Uses .NET 9+ native Guid.CreateVersion7() — no external packages needed.
/// </summary>
public static class NewId
{
    public static Guid Next() => Guid.CreateVersion7();
    public static Guid New(DateTimeOffset timestamp) => Guid.CreateVersion7(timestamp);
}