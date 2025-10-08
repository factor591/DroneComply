using DroneComply.Core.Interfaces;
using DroneComply.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DroneComply.Data.Repositories;

public class EfRepository<T> : IAsyncRepository<T> where T : class
{
    protected DroneComplyDbContext DbContext { get; }

    public EfRepository(DroneComplyDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await DbContext.Set<T>().AddAsync(entity, cancellationToken);
        await DbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return;
        }

        DbContext.Set<T>().Remove(entity);
        await DbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<T>().FindAsync(new object?[] { id }, cancellationToken);
    }

    public virtual async Task<IReadOnlyList<T>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<T>().ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        var entry = DbContext.Entry(entity);

        if (entry.State == EntityState.Detached)
        {
            DbContext.Set<T>().Attach(entity);
            entry.State = EntityState.Modified;
        }

        await DbContext.SaveChangesAsync(cancellationToken);
    }
}
