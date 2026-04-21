
namespace ResumeTracker.Domain.Events;

public sealed record ResumeCreatedDomainEvent(Guid ResumeId, Guid UserId) : DomainEvent;