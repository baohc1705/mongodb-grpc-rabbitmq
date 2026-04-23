using MongoDB.Bson.Serialization.Attributes;

namespace MenuNews.SyncService.Infrastructure.ReadDb.ReadModels;

public sealed class MenuReadModel
{
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public Guid Id { get; set; }

    [BsonElement("menu_name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("menu_slug")]
    public string Slug { get; set; } = default!;

    [BsonElement("menu_display_order")]
    public int DisplayOrder { get; set; }

    [BsonElement("menu_is_active")]
    public bool IsActive { get; set; }

    [BsonElement("menu_created_at")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("menu_updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [BsonElement("menu_news_items")]
    public List<NewsEmbedded> News { get; set; } = [];
}

public sealed class NewsEmbedded
{
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public Guid NewsId { get; set; }
    [BsonElement("news_title")]
    public string Title { get; set; } = default!;
    [BsonElement("news_slug")]
    public string Slug { get; set; } = default!;
    [BsonElement("news_summary")]
    public string Summary { get; set; } = default!;
    [BsonElement("news_thumbnail")]
    public string? Thumbnail { get; set; }
    [BsonElement("news_status")]
    public string Status { get; set; } = default!;
    [BsonElement("news_published_at")]
    public DateTime? PublishedAt { get; set; }
    [BsonElement("news_view_count")]
    public int ViewCount { get; set; }
    [BsonElement("news_created_at")]
    public DateTime CreatedAt { get; set; }
    [BsonElement("news_display_order")]
    public int DisplayOrder { get; set; }
}
