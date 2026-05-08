using MediatR;
using MenuNews.SyncService.Application.DTOs;

namespace MenuNews.SyncService.Application.Features.Menus.Commands.CreateMenuWithNewsPostgres;

public record CreateNewsItemsPostgresRequest(
    string Title,
    string Slug,
    string Summary,
    string Content,
    string? Thumbnail,
    DateTime? PublishedAt,
    int DisplayOrder
);

public record CreateMenuWithNewsPostgresCommand(
    string Name,
    string Slug,
    int DisplayOrder,
    List<CreateNewsItemsPostgresRequest> NewsItems
) : IRequest<MenuDto>;
