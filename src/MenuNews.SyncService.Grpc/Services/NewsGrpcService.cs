using Grpc.Core;
using MediatR;
using MenuNews.SyncService.Application.Features.News.Commands.CreateNewsWithMenusOutbox;
using MenuNews.SyncService.Grpc.Protos;

namespace MenuNews.SyncService.Grpc.Services;

public class NewsGrpcService : NewsService.NewsServiceBase
{
    private readonly ISender mediator;

    public NewsGrpcService(ISender mediator)
    {
        this.mediator = mediator;
    }

    public override async Task<NewsProto> CreateNewsWithMenu(CreateNewsWithMenuRequest request, ServerCallContext context)
    {
        var newsRequest = new CreateNewsWithMenusOutboxCommand(
            Title: request.Title,
            Slug: request.Slug,
            Summary: request.Summary,
            Content: request.Content,
            Thumbnail: request.Thumbnail,
            PublishedAt: DateTime.Parse(request.PublishedAt),
            MenuItems: request.MenusItems.Select(m => new MenusItemRequest(
                Name: m.Name,
                Slug: m.Slug,
                DisplayOrder: m.DisplayOrder,
                NewsMenuDisplayOrder: m.NmDisplayOrder
            )).ToList()
        );

        var res = await mediator.Send(newsRequest);

        return new NewsProto
        {
            Id = res.Id.ToString(),
            Title = res.Title,
            Slug = res.Slug,
            Status = res.Status,
            IsActive = res.IsActive,
            CreatedAt = res.CreatedAt.ToString("G")
          
        };
    }
}
