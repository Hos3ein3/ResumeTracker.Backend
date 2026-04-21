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

    public async Task DispatchAsync(
        IEnumerable<IDomainEvent> domainEvents,
        CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in domainEvents)
        {
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
            var handlers = _serviceProvider.GetServices(handlerType);

            foreach (var handler in handlers)
            {
                var method = handlerType.GetMethod(nameof(IDomainEventHandler<IDomainEvent>.HandleAsync));
                if (method is null)
                    continue;

                var task = (Task?)method.Invoke(handler, [domainEvent, cancellationToken]);
                if (task is not null)
                    await task;
            }
        }
    }
}