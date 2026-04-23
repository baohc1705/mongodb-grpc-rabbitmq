using MenuNews.SyncService.Application.DTOs;

namespace MenuNews.SyncService.Application.Common.Interfaces;

public interface IMenuReadRepository
{
    Task<IReadOnlyCollection<MenuDetailDto>> GetAllAsync(CancellationToken ct = default);
}
