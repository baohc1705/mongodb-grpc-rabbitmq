using MenuNews.SyncService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MenuNews.SyncService.Infrastructure.Persistence.WriteDb.Repositories;

public class BaseRepository<T> : IBaseRepository<T> where T : class
{
    private readonly AppDbContext context;
    private readonly DbSet<T> dbSet;
    public BaseRepository(AppDbContext context)
    {
        this.context = context;
        dbSet = context.Set<T>();
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await dbSet.AddAsync(entity, cancellationToken);
    }

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbSet
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default)
    {
        return await dbSet.Where(expression)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<T?> GetAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default)
    {
        return await dbSet
            .Where(expression)
            .AsNoTracking()
            .FirstOrDefaultAsync();
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
