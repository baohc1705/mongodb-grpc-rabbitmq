using MediatR;

namespace MenuNews.SyncService.Application.Features.Menus.Commands.CreateMenu;

public class CreateMenuCommand : IRequest<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public IReadOnlyList<CreateNewsItemRequest> NewsItems { get; set; } = new List<CreateNewsItemRequest>();
}

public class CreateNewsItemRequest
{
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Thumbnail { get; set; } = string.Empty;
    public DateTime? PublishedAt { get; set; }
    public int DisplayOrder { get; set; }
}
