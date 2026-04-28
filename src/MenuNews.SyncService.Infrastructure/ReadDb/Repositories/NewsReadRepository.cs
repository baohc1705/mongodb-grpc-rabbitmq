using MenuNews.SyncService.Application.Common.Interfaces;
using MenuNews.SyncService.Application.Common.Models.ReadModels;
using MongoDB.Driver;

namespace MenuNews.SyncService.Infrastructure.ReadDb.Repositories;

public class NewsReadRepository : BaseReadRepository<NewsReadModel>, INewsReadRepository
{
    public NewsReadRepository(MongoDbContext context) : base(context.NewsReadModel)
    {
    }
}
