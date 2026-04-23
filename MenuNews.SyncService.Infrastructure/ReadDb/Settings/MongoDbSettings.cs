namespace MenuNews.SyncService.Infrastructure.ReadDb.Settings;

public sealed class MongoDbSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string MenusCollectionName { get; set; } = string.Empty;
    public string NewsCollectionName { get; set; } = string.Empty;
}
