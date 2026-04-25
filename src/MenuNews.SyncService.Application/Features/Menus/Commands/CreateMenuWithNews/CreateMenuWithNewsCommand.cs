using MediatR;
using MenuNews.SyncService.Application.DTOs;

namespace MenuNews.SyncService.Application.Features.Menus.Commands.CreateMenuWithNews;
public sealed record CreateNewsItemsRequest(
    string Title,
    string Slug,
    string Summary,
    string Content,
    string? Thumbnail,
    DateTime? PublishedAt,
    int DisplayOrder
);

public sealed record CreateMenuWithNewsCommand(
    string Name,
    string Slug,
    int DisplayOrder,
    List<CreateNewsItemsRequest> NewsItems
) : IRequest<MenuDto>;
