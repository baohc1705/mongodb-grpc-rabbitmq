using MenuNews.SyncService.Domain.Entities;
using MenuNews.SyncService.Domain.Interfaces;

namespace MenuNews.SyncService.Infrastructure.Persistence.WriteDb.Repositories;

public class NewsRepository : BaseRepository<News>, INewsRepository
{
    public NewsRepository(AppDbContext context) : base(context)
    {
    }
}
