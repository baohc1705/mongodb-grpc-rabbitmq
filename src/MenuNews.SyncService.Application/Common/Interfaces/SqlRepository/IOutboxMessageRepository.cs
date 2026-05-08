using MenuNews.SyncService.Domain.Events;

namespace MenuNews.SyncService.Application.Common.Interfaces.SqlRepository;

public interface IOutboxMessageRepository : IBaseRepository<OutboxMessage>
{
}
