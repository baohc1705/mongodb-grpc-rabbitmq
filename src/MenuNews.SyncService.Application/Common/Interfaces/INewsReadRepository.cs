using MenuNews.SyncService.Application.DTOs;

namespace MenuNews.SyncService.Application.Common.Interfaces;

public interface INewsReadRepository
{
    Task<NewsDetailDto?> GetByIdAsync(Guid newsId, CancellationToken ct = default);
    Task<(List<NewsDetailDto> Items, long TotalCount)> GetAllAsync(
        bool activeOnly, int skip, int take, Guid? menuId, CancellationToken ct = default);
}
