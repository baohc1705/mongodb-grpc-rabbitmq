using MediatR;
using MenuNews.SyncService.Application.DTOs;

namespace MenuNews.SyncService.Application.Features.Menus.Commands.UpdateMenu;

public sealed record UpdateMenuCommand (
    Guid Id,
    string Name,
    string Slug,
    int DisplayOrder
) : IRequest<MenuDto>;
