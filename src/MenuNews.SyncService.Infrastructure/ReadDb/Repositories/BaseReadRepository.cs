using MenuNews.SyncService.Application.Common.Interfaces;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace MenuNews.SyncService.Infrastructure.ReadDb.Repositories;

public abstract class BaseReadRepository<T> : IBaseReadRepository<T> where T : class
{
    protected readonly IMongoCollection<T> collection;

    protected BaseReadRepository(IMongoCollection<T> collection)
    {
        this.collection = collection;
    }

    public virtual async Task<T?> GetAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default)
    {
        return await collection.Find(expression).FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await collection.Find(_ => true).ToListAsync(cancellationToken);
    }

    public virtual async Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default)
    {
        return await collection.Find(expression).ToListAsync(cancellationToken);
    }

    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> where, CancellationToken cancellationToken = default)
    {
        return await collection.Find(where).AnyAsync(cancellationToken);
    }
}
