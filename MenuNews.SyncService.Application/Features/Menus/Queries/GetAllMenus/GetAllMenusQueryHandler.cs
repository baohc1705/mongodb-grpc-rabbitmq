using MediatR;
using MenuNews.SyncService.Application.Features.Menus.DTOs;
using MenuNews.SyncService.Application.Features.News.DTOs;
using MenuNews.SyncService.Domain.Interfaces;

namespace MenuNews.SyncService.Application.Features.Menus.Queries.GetAllMenus;

public class GetAllMenusQueryHandler : IRequestHandler<GetAllMenusQuery, IReadOnlyList<MenuDto>>
{
    private readonly IUnitOfWork unitOfWork;

    public GetAllMenusQueryHandler(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<MenuDto>> Handle(GetAllMenusQuery request, CancellationToken cancellationToken)
    {
        var menus = await unitOfWork.MenuRepository.GetAllAsync(cancellationToken);
        return menus.Select(m => new MenuDto
        {
            Id = m.Id,
            Name = m.Name,
            Slug = m.Slug,
            DisplayOrder = m.DisplayOrder,
            CreatedAt = m.CreatedAt,
            News = m.NewsMenus
            .Where(nm => nm.News is not null)
            .Select(nm => new NewsDto
            {
                Id = nm.NewsId,
                Title = nm.News?.Title ?? string.Empty
            })
            .ToList()
        }).ToList();
    }
}
