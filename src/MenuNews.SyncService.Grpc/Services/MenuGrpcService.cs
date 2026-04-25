using Google.Protobuf.WellKnownTypes;
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
                PublishedAt: n.PublishedAt?.ToDateTime(),
                DisplayOrder: n.DisplayOrder
            )).ToList()
        );

        Console.WriteLine($"[gRPC] CreateMenuWithNews received {request.NewsItems.Count} news items in request.");

        var result = await mediator.Send(command, context.CancellationToken);

        return new MenuResponse
        {
            Id = result.Id.ToString(),
            Name = result.Name,
            Slug = result.Slug,
            DisplayOrder = result.DisplayOrder,
            IsActive = result.IsActive,
            CreatedAt = Timestamp.FromDateTime(DateTime.SpecifyKind(result.CreatedAt, DateTimeKind.Utc)),
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
                CreatedAt = Timestamp.FromDateTime(DateTime.SpecifyKind(m.CreatedAt, DateTimeKind.Utc))
            };

            detail.News.AddRange(m.News.Select(n => new NewsEmbeddedProto
            {
                Id = n.Id.ToString(),
                Title = n.Title,
                Slug = n.Slug,
                Summary = n.Summary,
                Thumbnail = n.Thumbnail ?? string.Empty,
                Status = n.Status,
                PublishedAt = n.PublishedAt.HasValue ? Timestamp.FromDateTime(DateTime.SpecifyKind(n.PublishedAt.Value, DateTimeKind.Utc)) : null,
                ViewCount = n.ViewCount,
                CreatedAt = Timestamp.FromDateTime(DateTime.SpecifyKind(n.CreatedAt, DateTimeKind.Utc)),
                DisplayOrder = n.DisplayOrder
            }));
            return detail;
        }));

        return response;
    }
}
