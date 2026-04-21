using Microsoft.EntityFrameworkCore;

using ResumeTracker.Application.Abstractions.Persistence;
using ResumeTracker.Domain;
using ResumeTracker.Domain.Common;

namespace ResumeTracker.Persistence.Repositories;

public class GenericRepository<TEntity, TId> : IGenericRepository<TEntity, TId>
    where TEntity : AggregateRoot<TId>
{
    protected readonly ResumeTrackerDbContext Context;
    protected readonly DbSet<TEntity> DbSet;

    public GenericRepository(ResumeTrackerDbContext context)
    {
        Context = context;
        DbSet = context.Set<TEntity>();
    }

    public async Task<TEntity?> GetByIdAsync(
        TId id,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync([id!], cancellationToken);
    }

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
    }

    public void Update(TEntity entity)
    {
        Context.Entry(entity).State = EntityState.Modified;
    }

    public void Remove(TEntity entity)
    {
        DbSet.Remove(entity);
    }
}