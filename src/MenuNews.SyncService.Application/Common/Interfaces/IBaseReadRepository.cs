using System.Linq.Expressions;

namespace MenuNews.SyncService.Application.Common.Interfaces;

public interface IBaseReadRepository<T> where T : class
{
    Task<T?> GetAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> where, CancellationToken cancellationToken = default);
}
