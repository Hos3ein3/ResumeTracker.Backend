namespace ResumeTracker.Domain.Common;

public abstract class AuditableAggregateRoot<TId> : AggregateRoot<TId>
{
    public DateTime CreatedAtUtc { get; protected set; } = DateTime.UtcNow;
    public DateTime? ModifiedAtUtc { get; protected set; }

    public bool IsDelete { get; set; } = false;

    public DateTime? DeletedAtUtc { get; set; }

    public void SetModified()
    {
        ModifiedAtUtc = DateTime.UtcNow;
    }
    public void SetDeleted()
    {
        IsDelete = true;
        DeletedAtUtc = DateTime.UtcNow;
    }
}