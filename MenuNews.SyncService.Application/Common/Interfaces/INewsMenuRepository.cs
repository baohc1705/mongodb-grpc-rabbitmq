using MenuNews.SyncService.Domain.Entities;

namespace MenuNews.SyncService.Application.Common.Interfaces;

public interface INewsMenuRepository : IBaseRepository<NewsMenu>
{
    Task SoftDeleteByMenuIdAsync(Guid menuId, CancellationToken ct = default);
    Task SoftDeleteByNewsIdAsync(Guid newsId, CancellationToken ct = default);
}
