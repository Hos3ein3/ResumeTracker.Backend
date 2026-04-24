using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ResumeTracker.Domain.Common;

namespace ResumeTracker.Domain;

public abstract class MongoAggregateRoot : IHasDomainEvents
{

    public string Id { get; protected set; } = NewId.Next().ToString();
    public DateTime CreatedAtUtc { get; protected set; } = DateTime.UtcNow;
    public DateTime? ModifiedAtUtc { get; protected set; } = DateTime.UtcNow;
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

    private readonly List<IDomainEvent> _domainEvents = [];
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    public void ClearDomainEvents() => _domainEvents.Clear();
    protected void RaiseDomainEvent(IDomainEvent e) => _domainEvents.Add(e);
}