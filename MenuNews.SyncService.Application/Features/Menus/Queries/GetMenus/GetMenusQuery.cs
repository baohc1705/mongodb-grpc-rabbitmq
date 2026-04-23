using MediatR;
using MenuNews.SyncService.Application.DTOs;

namespace MenuNews.SyncService.Application.Features.Menus.Queries.GetMenus;

public class GetMenusQuery : IRequest<List<MenuDetailDto>>
{
}
