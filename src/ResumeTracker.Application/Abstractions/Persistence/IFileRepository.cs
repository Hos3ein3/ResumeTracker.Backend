using ResumeTracker.Domain.Documents;

namespace ResumeTracker.Application.Abstractions.Persistence;

public interface IFileRepository
{
    Task<ResumeFile?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ResumeFile>> GetByResumeIdAsync(Guid resumeId, CancellationToken cancellationToken = default);
    Task<string> AddAsync(ResumeFile file, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task DeleteByResumeIdAsync(Guid resumeId, CancellationToken cancellationToken = default);
}