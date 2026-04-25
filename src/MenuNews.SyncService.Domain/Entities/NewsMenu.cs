using MenuNews.SyncService.Domain.Common;

namespace MenuNews.SyncService.Domain.Entities;

public sealed class NewsMenu : BaseEntity
{
    public Guid MenuId { get; private set; }
    public Guid NewsId { get; private set; }
    public int DisplayOrder { get; private set; }
    public Menu? Menu { get; private set; }
    public News? News { get; private set; }

    private NewsMenu() { }
    public static NewsMenu Create(Guid menuId, Guid newsId, int displayOrder)
    {
        return new NewsMenu
        {
            Id = Guid.NewGuid(),
            MenuId = menuId,
            NewsId = newsId,
            DisplayOrder = displayOrder,
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
}
