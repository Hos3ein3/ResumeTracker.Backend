
using MongoDB.Driver;

using ResumeTracker.Application.Abstractions.Events;
using ResumeTracker.Application.Abstractions.Persistence;
using ResumeTracker.Domain.Documents;
using ResumeTracker.Infrastructure.Persistence;

namespace ResumeTracker.Infrastructure.Repositories;

public class ResumeLogRepository : MongoRepository<ResumeLog>, IResumeLogRepository
{
    private readonly IMongoCollection<ResumeLog> _collection;

    public ResumeLogRepository(MongoDbContext context, IDomainEventDispatcher dispatcher) : base(context.ResumeLogs, dispatcher)
    {
        _collection = context.ResumeLogs;
    }

    public async Task<IReadOnlyList<ResumeLog>> GetAllByResumeIdAsync(Guid resumeId, CancellationToken ct = default)
    => await _collection
            .Find(x => x.ResumeId == resumeId)
            .SortByDescending(x => x.CreatedAtUtc)
            .ToListAsync(ct);
}
