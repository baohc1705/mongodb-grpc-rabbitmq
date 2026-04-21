using MediatR;
using MenuNews.SyncService.Application.Features.Menus.DTOs;

namespace MenuNews.SyncService.Application.Features.Menus.Queries.GetAllMenus;

public class GetAllMenusQuery : IRequest<IReadOnlyList<MenuDto>>
{
}
