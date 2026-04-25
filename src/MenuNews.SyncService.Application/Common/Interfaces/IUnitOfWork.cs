namespace MenuNews.SyncService.Application.Common.Interfaces;

public interface IUnitOfWork : IAsyncDisposable
{
    IMenuRepository MenuRepository { get; }
    INewsRepository NewsRepository { get; }
    INewsMenuRepository NewsMenuRepository { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
