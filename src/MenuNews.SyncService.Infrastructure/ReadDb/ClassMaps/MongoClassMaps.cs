using MenuNews.SyncService.Infrastructure.ReadDb.ReadModels;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace MenuNews.SyncService.Infrastructure.ReadDb.ClassMaps;

public static class MongoClassMaps
{
    private static bool registered = false;
    private static readonly object lockObj = new();

    public static void Register()
    {
        lock (lockObj)
        {
            if (registered) return;

            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

            BsonClassMap.RegisterClassMap<MenuReadModel>(menu =>
            {
                menu.AutoMap();
                menu.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<NewsEmbedded>(news =>
            {
                news.AutoMap();
                news.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<NewsReadModel>(news =>
            {
                news.AutoMap();
                news.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<MenuEmbedded>(menu =>
            {
                menu.AutoMap();
                menu.SetIgnoreExtraElements(true);
            });

            registered = true;
        }
    }
}
