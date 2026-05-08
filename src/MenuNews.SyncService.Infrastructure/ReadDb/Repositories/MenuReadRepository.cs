using MenuNews.SyncService.Application.Common.Interfaces.MongoRepository;
using MenuNews.SyncService.Application.Common.Models.ReadModels;
using MongoDB.Driver;

namespace MenuNews.SyncService.Infrastructure.ReadDb.Repositories;

public sealed class MenuReadRepository : BaseReadRepository<MenuReadModel>, IMenuReadRepository
{
    public MenuReadRepository(MongoDbContext context) : base(context.MenuReadModel)
    {
    }
}
