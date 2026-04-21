using MenuNews.SyncService.Domain.Entities;
using MenuNews.SyncService.Domain.Interfaces;

namespace MenuNews.SyncService.Infrastructure.Persistence.WriteDb.Repositories;

public class NewsMenuRepository : BaseRepository<NewsMenu>, INewsMenuRepository
{
    public NewsMenuRepository(AppDbContext context) : base(context)
    {
    }
}
