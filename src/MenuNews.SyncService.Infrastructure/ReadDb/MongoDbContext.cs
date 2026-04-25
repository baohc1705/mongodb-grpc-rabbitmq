using MenuNews.SyncService.Infrastructure.ReadDb.ReadModels;
using MenuNews.SyncService.Infrastructure.ReadDb.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace MenuNews.SyncService.Infrastructure.ReadDb;

public sealed class MongoDbContext
{
    private readonly IMongoDatabase database;
    private readonly MongoDbSettings settings;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        this.settings = settings.Value;
        var client = new MongoClient(this.settings.ConnectionString);
        database = client.GetDatabase(this.settings.DatabaseName);
    }

    public IMongoCollection<MenuReadModel> MenuReadModel
         => database.GetCollection<MenuReadModel>(this.settings.MenusCollectionName);

    public IMongoCollection<NewsReadModel> NewsReadModel
        => database.GetCollection<NewsReadModel>(this.settings.NewsCollectionName);

}
