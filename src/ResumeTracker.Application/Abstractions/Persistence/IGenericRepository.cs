using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ResumeTracker.Domain;

namespace ResumeTracker.Application.Abstractions.Persistence;

public interface IGenericRepository<TEntity, TId>
 where TEntity : AggregateRoot<TId>
{
    Task<TEntity?> GetByIdWithNoTrackingAsync(TId id, CancellationToken cancellationToken = default);
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    void Update(TEntity entity);
    void Remove(TEntity entity);
}
