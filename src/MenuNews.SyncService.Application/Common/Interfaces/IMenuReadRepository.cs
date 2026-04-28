using MenuNews.SyncService.Application.Common.Models.ReadModels;
using MenuNews.SyncService.Application.DTOs;
using MenuNews.SyncService.Domain.Entities;

namespace MenuNews.SyncService.Application.Common.Interfaces;

public interface IMenuReadRepository : IBaseReadRepository<MenuReadModel>
{
}
