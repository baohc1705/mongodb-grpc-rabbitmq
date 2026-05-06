using AutoMapper;
using MediatR;
using MenuNews.SyncService.Application.Common.Interfaces;
using MenuNews.SyncService.Application.DTOs;

namespace MenuNews.SyncService.Application.Features.Menus.Queries.GetMenusPostgres;

public class GetMenusPostgresQueryHandler : IRequestHandler<GetMenusPostgresQuery, IEnumerable<MenuDto>>
{
    private readonly IMenuRepository menuRepository;
    private readonly IMapper mapper;

    public GetMenusPostgresQueryHandler(IMenuRepository menuRepository, IMapper mapper)
    {
        this.menuRepository = menuRepository;
        this.mapper = mapper;
    }

    public async Task<IEnumerable<MenuDto>> Handle(GetMenusPostgresQuery request, CancellationToken cancellationToken)
    {
        var menus = await menuRepository.GetAllAsync(cancellationToken);
        return mapper.Map<IEnumerable<MenuDto>>(menus);
    }
}
