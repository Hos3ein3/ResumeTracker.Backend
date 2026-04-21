using MongoDB.Bson;
using MongoDB.Driver;

using ResumeTracker.Application.Abstractions.Persistence;
using ResumeTracker.Domain.Documents;
using ResumeTracker.Infrastructure.Persistence;

namespace ResumeTracker.Infrastructure.Repositories;

public sealed class UserPreferencesRepository : IUserPreferencesRepository
{
    private readonly IMongoCollection<UserPreferences> _collection;

    public UserPreferencesRepository(MongoDbContext context)
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

    public async Task<string> CreateAsync(
        UserPreferences preferences,
        CancellationToken cancellationToken = default)
    {
        preferences.Id = ObjectId.GenerateNewId().ToString();
        preferences.CreatedAtUtc = DateTime.UtcNow;
        preferences.UpdatedAtUtc = DateTime.UtcNow;

        await _collection.InsertOneAsync(preferences, cancellationToken: cancellationToken);
        return preferences.Id;
    }

    public async Task UpdateAsync(
        UserPreferences preferences,
        CancellationToken cancellationToken = default)
    {
        preferences.UpdatedAtUtc = DateTime.UtcNow;

        await _collection.ReplaceOneAsync(
            x => x.UserId == preferences.UserId,
            preferences,
            new ReplaceOptions { IsUpsert = true },
            cancellationToken);
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