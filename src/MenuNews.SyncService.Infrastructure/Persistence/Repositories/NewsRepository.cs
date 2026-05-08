using MenuNews.SyncService.Application.Common.Interfaces.SqlRepository;
using MenuNews.SyncService.Domain.Entities;

namespace MenuNews.SyncService.Infrastructure.Persistence.Repositories;

public sealed class NewsRepository : BaseRepository<News, AppDbContext>, INewsRepository
{
    public NewsRepository(AppDbContext context) : base(context)
    {
    }
}
