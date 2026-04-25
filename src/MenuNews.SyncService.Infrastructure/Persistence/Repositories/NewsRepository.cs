using MenuNews.SyncService.Application.Common.Interfaces;
using MenuNews.SyncService.Domain.Entities;

namespace MenuNews.SyncService.Infrastructure.Persistence.Repositories;

public sealed class NewsRepository : BaseRepository<News>, INewsRepository
{
    public NewsRepository(AppDbContext context) : base(context)
    {
    }
}
