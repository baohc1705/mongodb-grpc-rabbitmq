using MenuNews.SyncService.Application.Common.Interfaces;

namespace MenuNews.SyncService.Infrastructure.Persistence.UnitOfWork;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext context;
    private IMenuRepository menuRepository;
    private INewsRepository newsRepository;
    private INewsMenuRepository newsMenuRepository;
    private IOutboxMessageRepository outboxMessageRepository;

    public UnitOfWork(
        AppDbContext context,
        IMenuRepository menuRepository,
        INewsRepository newsRepository,
        INewsMenuRepository newsMenuRepository,
        IOutboxMessageRepository outboxMessageRepository)
    {
        this.context = context;
        this.menuRepository = menuRepository;
        this.newsRepository = newsRepository;
        this.newsMenuRepository = newsMenuRepository;
        this.outboxMessageRepository = outboxMessageRepository;
    }

    public IMenuRepository MenuRepository => menuRepository;

    public INewsRepository NewsRepository => newsRepository;

    public INewsMenuRepository NewsMenuRepository => newsMenuRepository;

    public IOutboxMessageRepository OutboxMessageRepository => outboxMessageRepository;

    public async ValueTask DisposeAsync()
    {
        await context.DisposeAsync();
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }
}
