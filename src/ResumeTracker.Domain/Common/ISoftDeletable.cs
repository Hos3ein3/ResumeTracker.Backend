namespace ResumeTracker.Domain.Common;

public interface ISoftDeletable
{
    bool IsDeleted { get; }
    DateTime? DeletedAtUtc { get; }
    string? DeletedBy { get; }
}