using AutoMapper;
using MediatR;
using MenuNews.SyncService.Application.Common.Exceptions;
using MenuNews.SyncService.Application.Common.Interfaces;
using MenuNews.SyncService.Application.Constants;
using MenuNews.SyncService.Application.DTOs;
using MenuNews.SyncService.Domain.Entities;
using MenuNews.SyncService.Domain.Events;

namespace MenuNews.SyncService.Application.Features.Menus.Commands.UpdateMenu;

public class UpdateMenuCommandHandler : IRequestHandler<UpdateMenuCommand, MenuDto>
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IRabbitMqPublisher publisher;
    private readonly IMapper mapper;

    public UpdateMenuCommandHandler(IUnitOfWork unitOfWork, IRabbitMqPublisher publisher, IMapper mapper)
    {
        this.unitOfWork = unitOfWork;
        this.publisher = publisher;
        this.mapper = mapper;
    }

    public async Task<MenuDto> Handle(UpdateMenuCommand request, CancellationToken cancellationToken)
    {
        var menu = await unitOfWork.MenuRepository.GetAsync(m => m.Id.Equals(request.Id), cancellationToken)
            ?? throw new NotFoundException(nameof(Menu), request.Id);
        if (await unitOfWork.MenuRepository.ExistsAsync(m => m.Slug.Equals(request.Slug), cancellationToken))
        {
            throw new BusinessException($"Menu with slug {request.Slug} already exists.");
        }

        menu.Update(request.Name, request.Slug, request.DisplayOrder);
        unitOfWork.MenuRepository.Update(menu);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var newsSyncItems = menu.NewsMenus
            .Where(nm => nm.IsActive && nm.News is not null)
            .Select (nm => new NewsSyncItem
            {
                NewsId = nm.News!.Id,
                Title = nm.News.Title,
                Slug = nm.News.Slug,
                Summary = nm.News.Summary,
                Thumbnail = nm.News.Thumbnail,
                Status = nm.News.Status,
                PublishedAt = nm.News.PublishedAt,
                ViewCount = nm.News.ViewCount,
                CreatedAt = nm.News.CreatedAt,
                DisplayOrder = nm.DisplayOrder
            }).ToList();

        var menuSyncEvent = new MenuSyncEvent
        {
            EventType = Domain.Enums.SyncEventType.UPSERT,
            MenuId = menu.Id,
            Name = menu.Name,
            Slug = menu.Slug,
            DisplayOrder = menu.DisplayOrder,
            IsActive = menu.IsActive,
            CreatedAt = menu.CreatedAt,
            UpdatedAt = menu.UpdatedAt,
            News = newsSyncItems
        };

        await publisher.PublishAsync(RabbitMqConstants.MenuExchange, RabbitMqConstants.MenuSyncRoutingKey,menuSyncEvent, cancellationToken);

        return mapper.Map<MenuDto>(menu);
    }
}
