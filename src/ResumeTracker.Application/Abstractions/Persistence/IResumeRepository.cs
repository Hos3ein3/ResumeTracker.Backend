

using ResumeTracker.Application.Abstractions.Persistence;
using ResumeTracker.Domain.Entities;

namespace ResumeTracker.Application.Repositories;

public interface IResumeRepository : IGenericRepository<Resume, Guid>
{
    Task<IReadOnlyList<Resume>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}