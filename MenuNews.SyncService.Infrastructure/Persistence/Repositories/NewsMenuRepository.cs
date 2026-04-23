using MenuNews.SyncService.Application.Common.Interfaces;
using MenuNews.SyncService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MenuNews.SyncService.Infrastructure.Persistence.Repositories;

public sealed class NewsMenuRepository : BaseRepository<NewsMenu>, INewsMenuRepository
{
    public NewsMenuRepository(AppDbContext context) : base(context)
    {
    }

    public async Task SoftDeleteByMenuIdAsync(Guid menuId, CancellationToken ct = default)
    {
        var items = await dbSet
            .Where(x => x.MenuId.Equals(menuId) && x.IsActive)
            .ToListAsync(ct);
        foreach (var item in items) item.SoftDelete();
    }

    public async Task SoftDeleteByNewsIdAsync(Guid newsId, CancellationToken ct = default)
    {
        var items = await dbSet
            .Where(x => x.NewsId.Equals(newsId) && x.IsActive)
            .ToListAsync(ct);
        foreach (var item in items) item.SoftDelete();
    }
}
