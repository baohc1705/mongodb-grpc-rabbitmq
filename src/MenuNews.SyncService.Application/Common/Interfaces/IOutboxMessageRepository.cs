using MenuNews.SyncService.Domain.Events;

namespace MenuNews.SyncService.Application.Common.Interfaces;

public interface IOutboxMessageRepository : IBaseRepository<OutboxMessage>
{
}
