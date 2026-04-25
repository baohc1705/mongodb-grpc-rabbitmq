using MenuNews.SyncService.Domain.Enums;

namespace MenuNews.SyncService.Domain.Events;

public sealed class MenuSyncEvent
{
    public SyncEventType EventType { get; init; }
    public Guid MenuId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
    public int DisplayOrder { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<NewsSyncItem> News { get; init; } = new();
}

public sealed class NewsSyncItem
{
    public Guid NewsId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
    public string Summary { get; init; } = string.Empty;
    public string Thumbnail { get; init; } = string.Empty;
    public NewsStatus Status { get; init; }
    public DateTime? PublishedAt { get; init; }
    public int ViewCount { get; init; }
    public DateTime CreatedAt { get; init; }
    public int DisplayOrder { get; init; }
}
