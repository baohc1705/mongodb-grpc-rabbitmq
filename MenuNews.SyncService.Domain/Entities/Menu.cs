using MenuNews.SyncService.Domain.Common;

namespace MenuNews.SyncService.Domain.Entities;

public sealed class Menu : BaseEntity
{
    public string Name { get;set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public IReadOnlyCollection<NewsMenu> NewsMenus { get; set; } = new List<NewsMenu>();
    public static Menu Create(string name, string slug, int displayOrder)
    {
        return new Menu
        {
            Id = Guid.NewGuid(),
            Name = name,
            Slug = slug,
            DisplayOrder = displayOrder,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(string name, string slug, int displayOrder)
    {
        Name = name;
        Slug = slug;
        DisplayOrder = displayOrder;
        UpdatedAt = DateTime.UtcNow;
        CreatedAt = CreatedAt;
    }

    public void SoftDelete()
    {
        IsActive = false;
        DeletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
