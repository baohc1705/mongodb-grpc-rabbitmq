using MediatR;
using MenuNews.SyncService.Application.Common.Interfaces;
using MenuNews.SyncService.Application.Common.Exceptions;
using MenuNews.SyncService.Application.Constants;
using MenuNews.SyncService.Domain.Entities;
using MenuNews.SyncService.Domain.Events;

namespace MenuNews.SyncService.Application.Features.Menus.Commands.DeleteMenu;

public sealed class DeleteMenuCommandHandler : IRequestHandler<DeleteMenuCommand>
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IRabbitMqPublisher publisher;

    public DeleteMenuCommandHandler(IUnitOfWork unitOfWork, IRabbitMqPublisher publisher)
    {
        this.unitOfWork = unitOfWork;
        this.publisher = publisher;
    }

    public async Task Handle(DeleteMenuCommand request, CancellationToken cancellationToken)
    {
        var menu = await unitOfWork.MenuRepository.GetAsync(m => m.Id.Equals(request.Id))
            ?? throw new NotFoundException(nameof(DeleteMenuCommand), request.Id);

        var junctionMenus = await unitOfWork.NewsMenuRepository.GetAllAsync(nm => nm.MenuId.Equals(menu.Id));

        unitOfWork.MenuRepository.RemoveRange(junctionMenus.Select(nm => new Domain.Entities.Menu { Id = nm.MenuId }).ToList());

        await unitOfWork.SaveChangesAsync();

        var menuMessage = BuildingMenuMessage(menu);

        await publisher.PublishAsync(menuMessage, MenuRoutingKey.Deleted, cancellationToken);
    }

    private MenuSyncEvent BuildingMenuMessage(Menu menu) => new MenuSyncEvent { MenuId = menu.Id };

}
