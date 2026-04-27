using Grpc.Core;
using MediatR;
using MenuNews.SyncService.Application.DTOs;
using MenuNews.SyncService.Application.Features.News.Commands.CreateNewsWithMenusOutbox;
using MenuNews.SyncService.Application.Features.News.Commands.DeleteNews;
using MenuNews.SyncService.Application.Features.News.Commands.UpdateNews;
using MenuNews.SyncService.Application.Features.News.Queries.GetAllNews;
using MenuNews.SyncService.Application.Features.News.Queries.GetNewsById;
using MenuNews.SyncService.Grpc.Protos;

namespace MenuNews.SyncService.Grpc.Services;

public class NewsGrpcService : NewsService.NewsServiceBase
{
    private readonly ISender mediator;

    public NewsGrpcService(ISender mediator)
    {
        this.mediator = mediator;
    }

    public override async Task<GetAllNewsResponse> GetAllNews(GetAllNewsRquest request, ServerCallContext context)
    {
        var result = await mediator.Send(new GetAllNewsQuery());

        var response = new GetAllNewsResponse();
        response.Items.AddRange(result.Select(MapToNewsDetailProto));

        return response;
    }

    public override async Task<NewsDetaiProto> GetNewsById(GetNewsByIdRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var newsId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid News Id format."));
        }

        var res = await mediator.Send(new GetNewsByIdQuery(newsId));

        return MapToNewsDetailProto(res);
    }

    public override async Task<NewsProto> CreateNewsWithMenu(CreateNewsWithMenuRequest request, ServerCallContext context)
    {
        var newsRequest = new CreateNewsWithMenusOutboxCommand(
            Title: request.Title,
            Slug: request.Slug,
            Summary: request.Summary,
            Content: request.Content,
            Thumbnail: request.Thumbnail,
            PublishedAt: DateTime.TryParse(request.PublishedAt, out var pubAt) ? pubAt : DateTime.UtcNow,
            MenuItems: request.MenusItems.Select(m => new MenusItemRequest(
                Name: m.Name,
                Slug: m.Slug,
                DisplayOrder: m.DisplayOrder,
                NewsMenuDisplayOrder: m.NmDisplayOrder
            )).ToList()
        );

        var res = await mediator.Send(newsRequest);

        return MapToNewsProto(res);
    }

    public override async Task<NewsProto> UpdateNews(UpdateNewsRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var newsId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid News Id format."));
        }

        var updateCommand = new UpdateNewsCommand(
            Id: newsId,
            Title: request.Title,
            Slug: request.Slug,
            Summary: request.Summary,
            Content: request.Content,
            Thumbnail: request.Thumbnail,
            PublishedAt: DateTime.TryParse(request.PublishedAt, out var pubAt) ? pubAt : DateTime.UtcNow
        );

        var res = await mediator.Send(updateCommand);

        return MapToNewsProto(res);
    }

    public override async Task<DeleteNewsByIdResponse> DeleteNewsById(DeleteNewsByIdRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var newsId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid News Id format."));
        }

        await mediator.Send(new DeleteNewsCommand(newsId));

        return new DeleteNewsByIdResponse
        {
            Result = "Success"
        };
    }

    private NewsDetaiProto MapToNewsDetailProto(NewsDetailDto res)
    {
        var proto = new NewsDetaiProto
        {
            Id = res.Id.ToString(),
            Title = res.Title,
            Slug = res.Slug,
            Summary = res.Summary,
            Thumbnail = res.Thumbnail ?? string.Empty,
            Status = res.Status,
            PublishedAt = res.PublishedAt?.ToString("O") ?? string.Empty,
            ViewCount = res.ViewCount,
            IsActive = res.IsActive,
            CreatedAt = res.CreatedAt.ToString("O")
        };

        if (res.Menus != null)
        {
            proto.Menus.AddRange(res.Menus.Select(MapToMenuEmbeddedProto));
        }

        return proto;
    }

    private NewsProto MapToNewsProto(NewsDto res)
    {
        return new NewsProto
        {
            Id = res.Id.ToString(),
            Title = res.Title,
            Slug = res.Slug,
            Status = res.Status,
            IsActive = res.IsActive,
            CreatedAt = res.CreatedAt.ToString("O")
        };
    }

    private MenuEmbeddedProto MapToMenuEmbeddedProto(MenuEmbeddedDto m)
    {
        return new MenuEmbeddedProto
        {
            Id = m.Id.ToString(),
            Name = m.Name,
            Slug = m.Slug,
            DisplayOrder = m.DisplayOrder,
            NmDisplayOrder = m.NmDisplayOrder
        };
    }
}
