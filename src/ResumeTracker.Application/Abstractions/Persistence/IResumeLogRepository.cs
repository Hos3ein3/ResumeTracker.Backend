using System.Collections.Generic;

using ResumeTracker.Domain.Documents;

namespace ResumeTracker.Application.Abstractions.Persistence;

public interface IResumeLogRepository : IMongoRepository<ResumeLog>
{
    Task<IReadOnlyList<ResumeLog>> GetAllByResumeIdAsync(Guid resumeId, CancellationToken ct = default);
}
