

using MongoDB.Driver;

using ResumeTracker.Application.Abstractions.Events;
using ResumeTracker.Application.Abstractions.Persistence;
using ResumeTracker.Domain;

namespace ResumeTracker.Infrastructure.Persistence;

public class MongoRepository<T>(
    IMongoCollection<T> collection,
    IDomainEventDispatcher dispatcher
) : IMongoRepository<T> where T : MongoAggregateRoot
{
    public async Task<T?> GetByIdAsync(string id, CancellationToken ct = default)
        => await collection
            .Find(Builders<T>.Filter.Eq(e => e.Id, id))
            .FirstOrDefaultAsync(ct);

    public async Task InsertAsync(T entity, CancellationToken ct = default)
    {
        await collection.InsertOneAsync(entity, cancellationToken: ct);
        await DispatchAndClearAsync(entity, ct);
    }

    public async Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        await collection.ReplaceOneAsync(
            Builders<T>.Filter.Eq(e => e.Id, entity.Id),
            entity,
            cancellationToken: ct);

        await DispatchAndClearAsync(entity, ct);
    }

    public async Task DeleteAsync(string id, CancellationToken ct = default)
        => await collection.DeleteOneAsync(
            Builders<T>.Filter.Eq(e => e.Id, id), ct);

    // ─── Dispatch after successful DB operation ───────────────────────
    private async Task DispatchAndClearAsync(T entity, CancellationToken ct)
    {
        if (entity.DomainEvents.Count == 0) return;

        await dispatcher.DispatchAllAsync(entity.DomainEvents, ct);
        entity.ClearDomainEvents();
    }
}
