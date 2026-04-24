
using ResumeTracker.Domain.Common;

namespace ResumeTracker.Domain;

public abstract class AggregateRoot<TId> : BaseEntity<TId>, IHasDomainEvents
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    => _domainEvents.Add(domainEvent);


    public void ClearDomainEvents()
    => _domainEvents.Clear();

}