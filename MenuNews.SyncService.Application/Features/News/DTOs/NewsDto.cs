using MenuNews.SyncService.Domain.Enums;

namespace MenuNews.SyncService.Application.Features.News.DTOs;

public class NewsDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Thumbnail { get; set; } = string.Empty;
    public DateTime? PublishedAt { get; set; }
}
