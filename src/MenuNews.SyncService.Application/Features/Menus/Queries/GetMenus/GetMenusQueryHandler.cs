using AutoMapper;
using MediatR;
using MenuNews.SyncService.Application.Common.Interfaces;
using MenuNews.SyncService.Application.DTOs;

namespace MenuNews.SyncService.Application.Features.Menus.Queries.GetMenus;

public class GetMenusQueryHandler : IRequestHandler<GetMenusQuery, List<MenuDetailDto>>
{
    private readonly IMenuReadRepository menuReadRepository;
    private readonly IMapper mapper;

    public GetMenusQueryHandler(IMenuReadRepository menuReadRepository, IMapper mapper)
    {
        this.menuReadRepository = menuReadRepository;
        this.mapper = mapper;
    }

    public async Task<List<MenuDetailDto>> Handle(GetMenusQuery request, CancellationToken cancellationToken)
    {
        var menus = await menuReadRepository.GetAllAsync(cancellationToken);
        return mapper.Map<List<MenuDetailDto>>(menus);
    }
}
