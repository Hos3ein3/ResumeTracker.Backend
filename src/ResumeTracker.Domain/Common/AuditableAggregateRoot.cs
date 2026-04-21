namespace ResumeTracker.Domain.Common;

public abstract class AuditableAggregateRoot<TId> : AggregateRoot<TId>, ISoftDeletable
{
    public DateTime CreatedAtUtc { get; protected set; } = DateTime.UtcNow;
    public DateTime? ModifiedAtUtc { get; protected set; }
    public string? CreatedBy { get; set; }
    public string? ModifiedBy { get; set; }

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }
    public string? DeletedBy { get; private set; }


    public void SetModified(string? modifiedBy)
    {
        ModifiedAtUtc = DateTime.UtcNow;
        ModifiedBy = modifiedBy;

    }
    public void SoftDelete(string? deletedBy = null)
    {
        IsDeleted = true;
        DeletedAtUtc = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }
    public void Restore()
    {
        IsDeleted = false;
        DeletedAtUtc = null;
        DeletedBy = null;
    }
}