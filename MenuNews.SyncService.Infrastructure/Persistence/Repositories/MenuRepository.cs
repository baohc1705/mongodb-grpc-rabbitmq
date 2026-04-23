using MenuNews.SyncService.Application.Common.Interfaces;
using MenuNews.SyncService.Domain.Entities;

namespace MenuNews.SyncService.Infrastructure.Persistence.Repositories;

public sealed class MenuRepository : BaseRepository<Menu>, IMenuRepository
{
    public MenuRepository(AppDbContext context) : base(context)
    {
    }
}
