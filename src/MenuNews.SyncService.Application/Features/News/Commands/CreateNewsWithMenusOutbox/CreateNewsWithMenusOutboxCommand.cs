using MediatR;
using MenuNews.SyncService.Application.DTOs;

namespace MenuNews.SyncService.Application.Features.News.Commands.CreateNewsWithMenusOutbox;

public record CreateNewsWithMenusOutboxCommand (
    string Title,
    string Slug,
    string Summary,
    string Content,
    string Thumbnail,
    DateTime PublishedAt,
    List<MenusItemRequest> MenuItems
): IRequest<NewsDto>;

public record MenusItemRequest (
    string Name,
    string Slug,
    int DisplayOrder,
    int NewsMenuDisplayOrder
);
