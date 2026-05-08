using MenuNews.SyncService.Domain.Events;

namespace MenuNews.SyncService.Application.Common.Interfaces.PostgresRepository;

public interface IOutboxMessagePostgresRepository : IBaseRepository<OutboxMessage>
{
}
