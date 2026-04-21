using MenuNews.SyncService.Domain.Common;

namespace MenuNews.SyncService.Domain.Entities;

public class NewsMenu : BaseEntity
{
    public Guid MenuId { get; set; }
    public Guid NewsId { get; set; }
    public int DisplayOrder { get; set; }
    public Menu Menu { get; set; } = null!;
    public News News { get; set; } = null!;
}
