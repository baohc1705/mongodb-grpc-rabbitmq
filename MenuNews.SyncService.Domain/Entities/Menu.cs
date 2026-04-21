using MenuNews.SyncService.Domain.Common;

namespace MenuNews.SyncService.Domain.Entities;

public class Menu : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public ICollection<NewsMenu> NewsMenus { get; set; } = new List<NewsMenu>();
}
