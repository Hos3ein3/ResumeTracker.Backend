

using ResumeTracker.Application.Abstractions.Events;
using ResumeTracker.Domain.Events;

namespace ResumeTracker.Application.Features.Resumes.Events;

public sealed class ResumeCreatedDomainEventHandler : IDomainEventHandler<ResumeCreatedDomainEvent>
{
    public Task HandleAsync(ResumeCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        // Example side effect:
        // audit log, default tags, statistics, integration event preparation, etc.
        return Task.CompletedTask;
    }
}