using MenuNews.SyncService.Application.Common.Models.ReadModels;
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

            // Root Menu Mapping
            BsonClassMap.RegisterClassMap<MenuReadModel>(menu =>
            {
                menu.MapIdProperty(m => m.Id).SetSerializer(new GuidSerializer(BsonType.String));
                menu.AutoMap();
                menu.SetIgnoreExtraElements(true);
                menu.MapProperty(m => m.Name).SetElementName("menu_name");
                menu.MapProperty(m => m.Slug).SetElementName("menu_slug");
                menu.MapProperty(m => m.DisplayOrder).SetElementName("menu_display_order");
                menu.MapProperty(m => m.IsActive).SetElementName("menu_is_active");
                menu.MapProperty(m => m.CreatedAt).SetElementName("menu_created_at");
                menu.MapProperty(m => m.UpdatedAt).SetElementName("menu_updated_at");
                menu.MapProperty(m => m.News).SetElementName("menu_news_items");
            });

            // Embedded News Mapping
            BsonClassMap.RegisterClassMap<NewsEmbedded>(news =>
            {
                news.MapIdProperty(n => n.NewsId).SetSerializer(new GuidSerializer(BsonType.String));
                news.AutoMap();
                news.SetIgnoreExtraElements(true);
                news.MapProperty(n => n.Title).SetElementName("news_title");
                news.MapProperty(n => n.Slug).SetElementName("news_slug");
                news.MapProperty(n => n.Summary).SetElementName("news_summary");
                news.MapProperty(n => n.Thumbnail).SetElementName("news_thumbnail");
                news.MapProperty(n => n.Status).SetElementName("news_status");
                news.MapProperty(n => n.PublishedAt).SetElementName("news_published_at");
                news.MapProperty(n => n.ViewCount).SetElementName("news_view_count");
                news.MapProperty(n => n.CreatedAt).SetElementName("news_created_at");
                news.MapProperty(n => n.DisplayOrder).SetElementName("news_display_order");
            });

            // Root News Mapping
            BsonClassMap.RegisterClassMap<NewsReadModel>(news =>
            {
                news.MapIdProperty(n => n.Id).SetSerializer(new GuidSerializer(BsonType.String));
                news.AutoMap();
                news.SetIgnoreExtraElements(true);
                news.MapProperty(n => n.Title).SetElementName("news_title");
                news.MapProperty(n => n.Slug).SetElementName("news_slug");
                news.MapProperty(n => n.Summary).SetElementName("news_summary");
                news.MapProperty(n => n.Content).SetElementName("news_content");
                news.MapProperty(n => n.Thumbnail).SetElementName("news_thumbnail");
                news.MapProperty(n => n.Status).SetElementName("news_status");
                news.MapProperty(n => n.PublishedAt).SetElementName("news_published_at");
                news.MapProperty(n => n.ViewCount).SetElementName("news_view_count");
                news.MapProperty(n => n.IsActive).SetElementName("news_is_active");
                news.MapProperty(n => n.CreatedAt).SetElementName("news_created_at");
                news.MapProperty(n => n.UpdatedAt).SetElementName("news_updated_at");
                news.MapProperty(n => n.Menus).SetElementName("news_menus");
            });

            // Embedded Menu Mapping
            BsonClassMap.RegisterClassMap<MenuEmbedded>(menu =>
            {
                menu.MapIdProperty(m => m.MenuId).SetSerializer(new GuidSerializer(BsonType.String));
                menu.AutoMap();
                menu.SetIgnoreExtraElements(true);
                menu.MapProperty(m => m.Name).SetElementName("menu_name");
                menu.MapProperty(m => m.Slug).SetElementName("menu_slug");
                menu.MapProperty(m => m.DisplayOrder).SetElementName("menu_display_order");
                menu.MapProperty(m => m.NmDisplayOrder).SetElementName("menu_nm_display_order");
            });

            registered = true;
        }
    }
}
