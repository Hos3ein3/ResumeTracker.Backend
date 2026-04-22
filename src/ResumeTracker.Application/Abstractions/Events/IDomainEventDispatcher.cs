
using ResumeTracker.Domain;

namespace ResumeTracker.Application.Abstractions.Events;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IDomainEvent domainEvent, CancellationToken ct = default);
    Task DispatchAllAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken ct = default);

}
