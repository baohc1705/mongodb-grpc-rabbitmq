using MenuNews.SyncService.Domain.Common;

namespace MenuNews.SyncService.Domain.Entities;

public sealed class Menu : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public int DisplayOrder { get; private set; }
    private readonly List<NewsMenu> newsMenus = new();
    public IReadOnlyCollection<NewsMenu> NewsMenus => newsMenus.AsReadOnly();

    private Menu() { }
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
    }

    public void SoftDelete()
    {
        IsActive = false;
        DeletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
