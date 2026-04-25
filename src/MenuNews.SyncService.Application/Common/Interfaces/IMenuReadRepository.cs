using MenuNews.SyncService.Application.DTOs;
using MenuNews.SyncService.Domain.Entities;

namespace MenuNews.SyncService.Application.Common.Interfaces;

public interface IMenuReadRepository
{
    Task<IReadOnlyCollection<MenuDetailDto>> GetAllAsync(CancellationToken ct = default);
    Task<Menu> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
