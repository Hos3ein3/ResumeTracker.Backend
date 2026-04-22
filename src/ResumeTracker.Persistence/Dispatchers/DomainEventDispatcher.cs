using Microsoft.Extensions.DependencyInjection; // ← GetServices lives here

using ResumeTracker.Application.Abstractions.Events;
using ResumeTracker.Domain;
using ResumeTracker.Domain.Common;

namespace ResumeTracker.Persistence.Dispatchers;

public sealed class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public DomainEventDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken ct = default)
    {
        var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
        var handlers = _serviceProvider.GetServices(handlerType);

        foreach (var handler in handlers)
        {
            // call HandleAsync via reflection — keeps it strongly typed at registration
            var method = handlerType.GetMethod(nameof(IDomainEventHandler<IDomainEvent>.HandleAsync))!;
            await (Task)method.Invoke(handler, [domainEvent, ct])!;
        }
    }
    public async Task DispatchAllAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken ct = default)
    {
        foreach (var domainEvent in domainEvents)
            await DispatchAsync(domainEvent, ct);
    }
}