using Grpc.Core;
using MediatR;
using MenuNews.SyncService.Application.Features.Menus.Commands.CreateMenuWithNews;
using MenuNews.SyncService.Application.Features.Menus.Queries.GetMenus;
using MenuNews.SyncService.Grpc.Protos;

namespace MenuNews.SyncService.Grpc.Services;

public class MenuGrpcService : MenuService.MenuServiceBase
{
    private readonly ISender mediator;

    public MenuGrpcService(ISender mediator)
    {
        this.mediator = mediator;
    }

    public override async Task<MenuResponse> CreateMenuWithNews(CreateMenuWithNewsRequest request, ServerCallContext context)
    {
        var command = new CreateMenuWithNewsCommand(
            request.Name,
            Slug: request.Slug,
            DisplayOrder: request.DisplayOrder,
            NewsItems: request.NewsItems.Select(n => new CreateNewsItemsRequest(
                Title: n.Title,
                Slug: n.Slug,
                Summary: n.Summary,
                Content: n.Content,
                Thumbnail: n.Thumbnail,
                PublishedAt: DateTime.Parse(n.PublishedAt),
                DisplayOrder: n.DisplayOrder
            )).ToList()
        );

        var result = await mediator.Send(command, context.CancellationToken);

        return new MenuResponse
        {
            Id = result.Id.ToString(),
            Name = result.Name,
            Slug = result.Slug,
            DisplayOrder = result.DisplayOrder,
            IsActive = result.IsActive,
            CreatedAt = result.CreatedAt.ToString("o")
        };
    }

    public override async Task<GetMenuWithNewsResponse> GetMenuWithNews(GetMenuWithNewsRequest request, ServerCallContext context)
    {
        var menus = await mediator.Send(new GetMenusQuery());
        var response = new GetMenuWithNewsResponse();

        response.Menus.AddRange(menus.Select(m =>
        {
            var detail = new MenuDetailResponse
            {
                Id = m.Id.ToString(),
                Name = m.Name,
                Slug = m.Slug,
                DisplayOrder = m.DisplayOrder,
                IsActive = m.IsActive,
                CreatedAt = m.CreatedAt.ToString("o")
            };

            detail.News.AddRange(m.News.Select(n => new NewsEmbeddedProto
            {
                Id = n.Id.ToString(),
                Title = n.Title,
                Slug = n.Slug,
                Summary = n.Summary,
                Thumbnail = n.Thumbnail ?? string.Empty,
                Status = n.Status,
                PublishedAt = n.PublishedAt?.ToString("o") ?? string.Empty,
                ViewCount = n.ViewCount,
                CreatedAt = n.CreatedAt.ToString("o"),
                DisplayOrder = n.DisplayOrder
            }));
            return detail;
        }));

        return response;
    }
}
