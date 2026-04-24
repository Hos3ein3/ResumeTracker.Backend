
using ResumeTracker.Domain.Common;

namespace ResumeTracker.Domain;

public abstract record DomainEvent : IDomainEvent
{
    public Guid EventId { get; } = NewId.Next();
    public DateTime OccurredOnUtc { get; init; } = DateTime.UtcNow;

}