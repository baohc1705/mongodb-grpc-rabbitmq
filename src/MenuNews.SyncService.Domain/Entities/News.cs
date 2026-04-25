using MenuNews.SyncService.Domain.Common;
using MenuNews.SyncService.Domain.Enums;

namespace MenuNews.SyncService.Domain.Entities;

public sealed class News : BaseEntity
{
    public string Title { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string Summary { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;
    public string Thumbnail { get; private set; } = string.Empty;
    public NewsStatus Status { get; private set; }
    public DateTime? PublishedAt { get; private set; }
    public int ViewCount{ get; private set; }

   private readonly List<NewsMenu> newsMenus = new();
    public IReadOnlyCollection<NewsMenu> NewsMenus => newsMenus.AsReadOnly();
    private News() { }
    public static News Create(string title, string slug, string summary, string content, string thumbnail, DateTime? publishedAt)
    {
        return new News
        {
            Id = Guid.NewGuid(),
            Title = title,
            Slug = slug,
            Summary = summary,
            Content = content,
            Thumbnail = thumbnail,
            Status = NewsStatus.DRAF,
            PublishedAt = publishedAt,
            ViewCount = 0,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }
    public void Update(string title, string slug, string summary, string content, string thumbnail, NewsStatus status, DateTime? publishedAt)
    {
        Title = title;
        Slug = slug;
        Summary = summary;
        Content = content;
        Thumbnail = thumbnail;
        Status = status;
        PublishedAt = publishedAt;
        UpdatedAt = DateTime.UtcNow;
    }
    public void SoftDelete()
    {
        IsActive = false;
        DeletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void IncrementViewCount() => ViewCount++;
    
}
