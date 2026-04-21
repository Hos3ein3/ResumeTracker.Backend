using Microsoft.EntityFrameworkCore;

using ResumeTracker.Application.Abstractions.Persistence;
using ResumeTracker.Application.Repositories;
using ResumeTracker.Domain.Entities;

namespace ResumeTracker.Persistence.Repositories;

public sealed class ResumeRepository : GenericRepository<Resume, Guid>, IResumeRepository
{
    public ResumeRepository(ResumeTrackerDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Resume>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Guid resumeId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .AnyAsync(r => r.Id == resumeId, cancellationToken);
    }
}