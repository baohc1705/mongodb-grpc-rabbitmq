using MenuNews.SyncService.Application.Common.Interfaces.SqlRepository;
using MenuNews.SyncService.Domain.Entities;

namespace MenuNews.SyncService.Infrastructure.Persistence.Repositories;

public sealed class MenuRepository : BaseRepository<Menu, AppDbContext>, IMenuRepository
{
    public MenuRepository(AppDbContext context) : base(context)
    {
    }
}
