using MenuNews.SyncService.Application.Features.News.DTOs;

namespace MenuNews.SyncService.Application.Features.Menus.DTOs;

public class MenuDto
{
    public Guid Id { get; set; }
    public bool Active { get; set; } = true;
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<NewsDto> News { get; set; } = new ();
}
