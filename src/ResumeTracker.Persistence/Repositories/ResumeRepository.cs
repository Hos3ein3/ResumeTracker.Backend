using Microsoft.EntityFrameworkCore;

using ResumeTracker.Application.Common.Extensions;
using ResumeTracker.Application.Common.Models;
using ResumeTracker.Application.Repositories;
using ResumeTracker.Domain.Common;
using ResumeTracker.Domain.Entities;

namespace ResumeTracker.Persistence.Repositories;

public sealed class ResumeRepository : GenericRepository<Resume, Guid>, IResumeRepository
{
    private readonly ResumeTrackerDbContext _context;
    public ResumeRepository(ResumeTrackerDbContext context) : base(context)
    {
        _context = context;
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

    public async Task<PagedList<Resume>> GetAllByUserAsync(
       Guid userId,
       PagedQuery query,
       CancellationToken ct = default)
       => await _context.Resumes
           .Where(r => r.UserId == userId)
           .OrderByDescending(r => r.ApplyDate)
           .ToPagedListAsync(query, ct);
}