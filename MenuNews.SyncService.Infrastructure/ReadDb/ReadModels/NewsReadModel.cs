using MongoDB.Bson.Serialization.Attributes;

namespace MenuNews.SyncService.Infrastructure.ReadDb.ReadModels;

public sealed class NewsReadModel
{
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public Guid NewsId { get; set; }
    [BsonElement("news_title")]
    public string Title { get; set; } = string.Empty;
    [BsonElement("news_slug")]
    public string Slug { get; set; } = string.Empty;
    [BsonElement("news_summary")]
    public string Summary { get; set; } = string.Empty;
    [BsonElement("news_content")]
    public string Content { get; set; } = string.Empty;
    [BsonElement("news_thumbnail")]
    public string? Thumbnail { get; set; }
    [BsonElement("news_status")]
    public string Status { get; set; } = string.Empty;
    [BsonElement("news_published_at")]
    public DateTime? PublishedAt { get; set; }
    [BsonElement("news_view_count")]
    public int ViewCount { get; set; }
    [BsonElement("news_is_active")]
    public bool IsActive { get; set; }
    [BsonElement("news_created_at")]
    public DateTime CreatedAt { get; set; }
    [BsonElement("news_updated_at")]
    public DateTime? UpdatedAt { get; set; }
    [BsonElement("news_menus")]
    public List<MenuEmbedded> Menus { get; set; } = [];
}

public sealed class MenuEmbedded
{
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public Guid MenuId { get; set; }
    [BsonElement("menu_name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("menu_slug")]
    public string Slug { get; set; } = string.Empty;

    [BsonElement("menu_display_order")]
    public int DisplayOrder { get; set; }
    [BsonElement("menu_nm_display_order")]
    public int NmDisplayOrder { get; set; }
}
