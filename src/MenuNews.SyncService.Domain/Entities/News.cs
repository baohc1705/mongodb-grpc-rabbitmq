using MenuNews.SyncService.Domain.Common;
using MenuNews.SyncService.Domain.Enums;

namespace MenuNews.SyncService.Domain.Entities;

public sealed class News : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Slug { get;  set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Content { get;  set; } = string.Empty;
    public string Thumbnail { get; set; } = string.Empty;
    public NewsStatus Status { get; set; }
    public DateTime? PublishedAt { get; set; }
    public int ViewCount{ get; set; }
    public IReadOnlyCollection<NewsMenu> NewsMenus { get; set; } = new List<NewsMenu>();
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

    public void SoftDelete()
    {
        IsActive = false;
        DeletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void IncrementViewCount() => ViewCount++;
    
}
