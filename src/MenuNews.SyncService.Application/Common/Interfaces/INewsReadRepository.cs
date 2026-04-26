using MenuNews.SyncService.Application.DTOs;
using MenuNews.SyncService.Domain.Entities;

namespace MenuNews.SyncService.Application.Common.Interfaces;

public interface INewsReadRepository
{
    Task<News?> GetByIdAsync(Guid newsId, CancellationToken ct = default);
    Task<IReadOnlyList<NewsDetailDto>> GetAllAsync(CancellationToken ct = default);
}
