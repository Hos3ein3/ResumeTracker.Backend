using ResumeTracker.Domain.Common;

namespace ResumeTracker.Application.Abstractions.Persistence;

public interface IMongoRepository<T> where T : IHasDomainEvents
{
    Task<T?> GetByIdAsync(string id, CancellationToken ct = default);
    Task InsertAsync(T entity, CancellationToken ct = default);
    Task UpdateAsync(T entity, CancellationToken ct = default);
    Task DeleteAsync(string id, CancellationToken ct = default);
}