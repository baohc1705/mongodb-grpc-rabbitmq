using MediatR;
using MenuNews.SyncService.Application.DTOs;

namespace MenuNews.SyncService.Application.Features.Menus.Queries.GetMenusPostgres;

public class GetMenusPostgresQuery() : IRequest<IEnumerable<MenuDto>>;
