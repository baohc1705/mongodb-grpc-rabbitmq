using MenuNews.SyncService.Application.Common.Interfaces;
using MenuNews.SyncService.Infrastructure.Postgres.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MenuNews.SyncService.Infrastructure.Postgres.Persistence.Repositories;

public class BasePostgresRepository<T> : IBaseRepository<T> where T : class
{
    private readonly DbSet<T> dbSet;

    public BasePostgresRepository(PostgresDbContext context)
    {
        dbSet = context.Set<T>();
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await dbSet.AddAsync(entity, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        await dbSet.AddRangeAsync(entities, cancellationToken); 
    }

    public Task<bool> ExistsAsync(Expression<Func<T, bool>> where, CancellationToken cancellationToken = default)
    {
        return dbSet.AnyAsync(where, cancellationToken);
    }

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbSet.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default)
    {
        return await dbSet
            .Where(expression)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<T?> GetAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default)
    {
        return await dbSet.AsNoTracking().FirstOrDefaultAsync(expression, cancellationToken);
    }

    public void Remove(T entity)
    {
        dbSet.Remove(entity);
    }

    public void RemoveRange(IEnumerable<T> entities)
    {
        dbSet.RemoveRange(entities);
    }

    public void Update(T entity)
    {
        dbSet.Update(entity);
    }
}
