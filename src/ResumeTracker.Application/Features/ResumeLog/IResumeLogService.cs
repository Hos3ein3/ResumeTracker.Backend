using ResumeTracker.Domain.Documents;
using ResumeTracker.Domain.Enums;

namespace ResumeTracker.Application.Features.ResumeLog;

public interface IResumeLogService
{
    Task LogStatusChangeAsync(Guid resumeId,
        ResumeStatus previousStatus,
        ResumeStatus currentStatus,
        CancellationToken ct = default);

    Task<IReadOnlyList<Domain.Documents.ResumeLog>> GetLogsAsync(
    Guid resumeId,
    CancellationToken ct = default);
}
