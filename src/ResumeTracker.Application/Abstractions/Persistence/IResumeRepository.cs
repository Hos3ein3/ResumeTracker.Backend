

using ResumeTracker.Application.Abstractions.Persistence;
using ResumeTracker.Application.Common.Models;
using ResumeTracker.Domain.Common;
using ResumeTracker.Domain.Entities;

namespace ResumeTracker.Application.Repositories;

public interface IResumeRepository : IGenericRepository<Resume, Guid>
{
    Task<PagedList<Resume>> GetAllByUserAsync(
        Guid userId,
        PagedQuery query,
        CancellationToken ct = default);
    Task<IReadOnlyList<Resume>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}