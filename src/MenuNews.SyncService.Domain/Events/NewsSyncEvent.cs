using MenuNews.SyncService.Domain.Enums;

namespace MenuNews.SyncService.Domain.Events;

public sealed class NewsSyncEvent
{
    public SyncEventType EventType { get; init; }
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
    public string Summary { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public string Thumbnail { get; init; } = string.Empty;
    public NewsStatus Status { get; init; }
    public DateTime? PublishedAt { get; init; }
    public int ViewCount { get; init; }
    public bool IsActive { get; init; }
    public int DisplayOrder { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public List<MenuSyncItem> Menus { get; init; } = [];
}
public sealed class MenuSyncItem
{
    public Guid MenuId { get; init; }
    public string Name { get; init; } = default!;
    public string Slug { get; init; } = default!;
    public int DisplayOrder { get; init; }
    public int NmDisplayOrder { get; init; }
}