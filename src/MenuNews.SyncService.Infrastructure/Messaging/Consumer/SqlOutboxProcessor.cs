using MenuNews.SyncService.Domain.Events;
using MenuNews.SyncService.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MenuNews.SyncService.Infrastructure.Messaging.Consumer;

public class SqlOutboxProcessor : OutboxProcessor<OutboxMessage, AppDbContext>
{
    public SqlOutboxProcessor(IServiceScopeFactory serviceScopeFactory, ILogger<OutboxProcessor<OutboxMessage, AppDbContext>> logger) : base(serviceScopeFactory, logger)
    {
    }
}
