namespace MenuNews.SyncService.Application.Common.Models.ReadModels;

public sealed class MenuReadModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = default!;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<NewsEmbedded> News { get; set; } = [];
}

public sealed class NewsEmbedded
{
    public Guid NewsId { get; set; }
    public string Title { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public string Summary { get; set; } = default!;
    public string? Thumbnail { get; set; }
    public string Status { get; set; } = default!;
    public DateTime? PublishedAt { get; set; }
    public int ViewCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public int DisplayOrder { get; set; }
}
