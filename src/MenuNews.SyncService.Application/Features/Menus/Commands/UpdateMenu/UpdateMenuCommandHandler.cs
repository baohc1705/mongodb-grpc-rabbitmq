using AutoMapper;
using MediatR;
using MenuNews.SyncService.Application.Common.Exceptions;
using MenuNews.SyncService.Application.Common.Interfaces;
using MenuNews.SyncService.Application.Constants;
using MenuNews.SyncService.Application.DTOs;
using MenuNews.SyncService.Domain.Entities;
using MenuNews.SyncService.Domain.Enums;
using MenuNews.SyncService.Domain.Events;

namespace MenuNews.SyncService.Application.Features.Menus.Commands.UpdateMenu;

public class UpdateMenuCommandHandler : IRequestHandler<UpdateMenuCommand, MenuDto>
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IMapper mapper;
    private readonly IRabbitMqPublisher publisher;
    private readonly IMenuReadRepository menuReadRepository;

    public UpdateMenuCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IRabbitMqPublisher publisher, IMenuReadRepository menuReadRepository)
    {
        this.unitOfWork = unitOfWork;
        this.mapper = mapper;
        this.publisher = publisher;
        this.menuReadRepository = menuReadRepository;
    }

    public async Task<MenuDto> Handle(UpdateMenuCommand request, CancellationToken cancellationToken)
    {
        var menu = await menuReadRepository.GetByIdAsync(request.Id, cancellationToken)
           ?? throw new NotFoundException(nameof(UpdateMenuCommand), request.Id);

        if (await unitOfWork.MenuRepository.ExistsAsync(m => m.Slug == request.Slug, cancellationToken))
            throw new BusinessException($"Menu slug {request.Slug} is existed!");

        menu.Name = request.Name;
        menu.Slug = request.Slug;
        menu.DisplayOrder = request.DisplayOrder;

        unitOfWork.MenuRepository.Update(menu);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var menuUpdatedMessage = BuidlerMessage(menu);

        await publisher.PublishAsync(menuUpdatedMessage, MenuRoutingKey.Updated, cancellationToken);

        return mapper.Map<MenuDto>(menu);
    }

    private MenuSyncEvent BuidlerMessage(Menu menu)
    {
        return new MenuSyncEvent
        {
            EventType = SyncEventType.UPSERT,
            MenuId = menu.Id,
            Name = menu.Name,
            Slug = menu.Slug,
            DisplayOrder = menu.DisplayOrder,
            UpdatedAt = menu.UpdatedAt,
        };
    }

}
