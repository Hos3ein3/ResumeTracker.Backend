

using Microsoft.Extensions.Logging;

using ResumeTracker.Application.Abstractions.Localization;
using ResumeTracker.Application.Abstractions.Persistence;
using ResumeTracker.Application.Features.ResumeLog;
using ResumeTracker.Domain.Enums;

namespace ResumeTracker.Infrastructure.Services.ResumeLog;

public class ResumeLogService : IResumeLogService
{
    private readonly IResumeLogRepository _repository;
    private readonly IMessageLocalizer _localizer;
    private readonly ILogger<ResumeLogService> _logger;

    public ResumeLogService(IResumeLogRepository repository, IMessageLocalizer localizer, ILogger<ResumeLogService> logger)
    {
        _repository = repository;
        _localizer = localizer;
        _logger = logger;
    }

    public async Task<IReadOnlyList<Domain.Documents.ResumeLog>> GetLogsAsync(Guid resumeId, CancellationToken ct = default)
    => await _repository.GetAllByResumeIdAsync(resumeId, ct);

    public async Task LogStatusChangeAsync(Guid resumeId, ResumeStatus previousStatus, ResumeStatus currentStatus, CancellationToken ct = default)
    {
        var log = Domain.Documents.ResumeLog.Create(resumeId, previousStatus, currentStatus);
        await _repository.InsertAsync(log, ct);
    }
}
