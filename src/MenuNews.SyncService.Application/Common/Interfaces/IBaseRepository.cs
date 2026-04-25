using System.Linq.Expressions;

namespace MenuNews.SyncService.Application.Common.Interfaces;

public interface IBaseRepository<T> where T : class
{
    Task<T?> GetAsync(Expression<Func<T , bool>> expression, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default);
    //Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<IQueryable<T>, IQueryable<T>>>? querybuilder, CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);

    Task<bool> ExistsAsync(Expression<Func<T, bool>> where, CancellationToken cancellationToken = default);
}
