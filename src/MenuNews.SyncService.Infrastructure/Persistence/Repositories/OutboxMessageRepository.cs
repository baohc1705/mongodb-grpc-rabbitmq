using MenuNews.SyncService.Application.Common.Interfaces;
using MenuNews.SyncService.Domain.Events;

namespace MenuNews.SyncService.Infrastructure.Persistence.Repositories;

public class OutboxMessageRepository : BaseRepository<OutboxMessage>, IOutboxMessageRepository
{
    public OutboxMessageRepository(AppDbContext context) : base(context)
    {
    }
}
