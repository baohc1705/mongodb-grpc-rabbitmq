using MenuNews.SyncService.Domain.Events;
using MenuNews.SyncService.Infrastructure.Postgres.Persistence.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MenuNews.SyncService.Infrastructure.Messaging.Consumer;

public class PostgresOutboxProcessor : OutboxProcessor<OutboxMessage, PostgresDbContext>
{
    public PostgresOutboxProcessor(IServiceScopeFactory serviceScopeFactory, ILogger<OutboxProcessor<OutboxMessage, PostgresDbContext>> logger) : base(serviceScopeFactory, logger)
    {
    }
}
