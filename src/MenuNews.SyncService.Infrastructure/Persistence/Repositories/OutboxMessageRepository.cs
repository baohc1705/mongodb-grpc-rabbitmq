using MenuNews.SyncService.Application.Common.Interfaces.SqlRepository;
using MenuNews.SyncService.Domain.Events;

namespace MenuNews.SyncService.Infrastructure.Persistence.Repositories;

public class OutboxMessageRepository : BaseRepository<OutboxMessage, AppDbContext>, IOutboxMessageRepository
{
    public OutboxMessageRepository(AppDbContext context) : base(context)
    {
    }
}
