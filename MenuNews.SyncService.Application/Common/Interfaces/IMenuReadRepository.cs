using MenuNews.SyncService.Application.DTOs;

namespace MenuNews.SyncService.Application.Common.Interfaces;

public interface IMenuReadRepository
{
    Task<MenuDetailDto?> GetByIdAsync(Guid menuId, CancellationToken ct = default);
    Task<(List<MenuDetailDto> Items, long TotalCount)> GetAllAsync(
        bool activeOnly, int skip, int take, CancellationToken ct = default);
    Task<IReadOnlyCollection<MenuDetailDto>> GetAllAsync(CancellationToken ct = default);
}
