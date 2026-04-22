

using ResumeTracker.Application.Abstractions.Events;
using ResumeTracker.Application.Features.ResumeLog;
using ResumeTracker.Domain.Events.Resume;

namespace ResumeTracker.Application.EventsHandler.ResumeLog;

public sealed class LogResumeStatusChangeEventHandler : IDomainEventHandler<ResumeStatusChangedEvent>
{
    private readonly IResumeLogService _resumeLogService;
    public LogResumeStatusChangeEventHandler(IResumeLogService resumeLogService)
    {
        _resumeLogService = resumeLogService;
    }

    public async Task HandleAsync(ResumeStatusChangedEvent @event, CancellationToken cancellationToken = default)
    {
        await _resumeLogService.LogStatusChangeAsync(@event.ResumeId, @event.PreviousStatus, @event.CurrentStatus, cancellationToken);
    }
}
