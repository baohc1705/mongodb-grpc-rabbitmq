using MenuNews.SyncService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace MenuNews.SyncService.Infrastructure.Persistence.WriteDb.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext context;
    private readonly IMenuRepository menuRepository;
    private readonly INewsRepository newsRepository;
    private readonly INewsMenuRepository newsMenuRepository;

    private IDbContextTransaction? transaction;
    public UnitOfWork(
        AppDbContext context,
        IMenuRepository menuRepository,
        INewsRepository newsRepository,
        INewsMenuRepository newsMenuRepository)
    {
        this.context = context;
        this.menuRepository = menuRepository;
        this.newsRepository = newsRepository;
        this.newsMenuRepository = newsMenuRepository;
    }

    public IMenuRepository MenuRepository => menuRepository;

    public INewsRepository NewsRepository => newsRepository;

    public INewsMenuRepository NewsMenuRepository => newsMenuRepository;

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        transaction = await context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (transaction is null)
            throw new InvalidOperationException("Transaction chưa được bắt đầu.");

        await transaction.CommitAsync(cancellationToken);
        await transaction.DisposeAsync();

        transaction = null;
    }

    public async ValueTask DisposeAsync()
    {
        if (transaction is not null)
            await transaction.DisposeAsync();
        await context.DisposeAsync();
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (transaction is null)
            throw new InvalidOperationException("Transaction chưa được bắt đầu.");
        await transaction.RollbackAsync(cancellationToken);
        await transaction.DisposeAsync();
        transaction = null;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }
}
