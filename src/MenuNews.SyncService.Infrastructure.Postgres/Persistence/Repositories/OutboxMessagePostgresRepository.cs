using MenuNews.SyncService.Application.Common.Interfaces.PostgresRepository;
using MenuNews.SyncService.Domain.Events;
using MenuNews.SyncService.Infrastructure.Persistence.Repositories;
using MenuNews.SyncService.Infrastructure.Postgres.Persistence.Data;

namespace MenuNews.SyncService.Infrastructure.Postgres.Persistence.Repositories;

public class OutboxMessagePostgresRepository : BaseRepository<OutboxMessage, PostgresDbContext>, IOutboxMessagePostgresRepository
{
    public OutboxMessagePostgresRepository(PostgresDbContext context) : base(context)
    {
    }
}
