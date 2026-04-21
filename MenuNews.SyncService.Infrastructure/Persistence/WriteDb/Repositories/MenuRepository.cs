using MenuNews.SyncService.Domain.Entities;
using MenuNews.SyncService.Domain.Interfaces;

namespace MenuNews.SyncService.Infrastructure.Persistence.WriteDb.Repositories;

public class MenuRepository : BaseRepository<Menu>, IMenuRepository
{
    public MenuRepository(AppDbContext context) : base(context)
    {
    }
}
