using MongoDB.Bson;
using MongoDB.Driver;

using ResumeTracker.Application.Abstractions.Events;
using ResumeTracker.Application.Abstractions.Persistence;
using ResumeTracker.Domain.Documents;
using ResumeTracker.Infrastructure.Persistence;

namespace ResumeTracker.Infrastructure.Repositories;

public sealed class UserPreferencesRepository : MongoRepository<UserPreferences>, IUserPreferencesRepository
{
    private readonly IMongoCollection<UserPreferences> _collection;

    public UserPreferencesRepository(MongoDbContext context, IDomainEventDispatcher dispatcher) : base(context.UserPreferences, dispatcher)
    {
        _collection = context.UserPreferences;
    }

    public async Task<UserPreferences?> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(x => x.UserId == userId)
            .FirstOrDefaultAsync(cancellationToken);
    }




    public async Task DeleteByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        await _collection.DeleteOneAsync(
            x => x.UserId == userId,
            cancellationToken);
    }
}