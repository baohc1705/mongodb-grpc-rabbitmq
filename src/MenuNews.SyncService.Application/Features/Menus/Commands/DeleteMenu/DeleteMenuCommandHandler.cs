using MediatR;
using MenuNews.SyncService.Application.Common.Exceptions;
using MenuNews.SyncService.Application.Common.Interfaces;
using MenuNews.SyncService.Application.Constants;
using MenuNews.SyncService.Domain.Entities;
using MenuNews.SyncService.Domain.Events;

namespace MenuNews.SyncService.Application.Features.Menus.Commands.DeleteMenu;

public sealed class DeleteMenuCommandHandler : IRequestHandler<DeleteMenuCommand>
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IMenuRepository menuRepository;
    private readonly INewsMenuRepository newsMenuRepository;
    private readonly IRabbitMqPublisher publisher;

    public DeleteMenuCommandHandler(
        IUnitOfWork unitOfWork,
        IRabbitMqPublisher publisher,
        IMenuRepository menuRepository,
        INewsMenuRepository newsMenuRepository)
    {
        this.unitOfWork = unitOfWork;
        this.publisher = publisher;
        this.menuRepository = menuRepository;
        this.newsMenuRepository = newsMenuRepository;
    }

    public async Task Handle(DeleteMenuCommand request, CancellationToken cancellationToken)
    {

        var menu = await menuRepository.GetAsync(m => m.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(DeleteMenuCommand), request.Id);

        var junctions = await newsMenuRepository.GetAllAsync(nm => nm.MenuId == menu.Id, cancellationToken);

        await unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            newsMenuRepository.RemoveRange(junctions);
            menuRepository.Remove(menu);

            await unitOfWork.SaveChangesAsync(cancellationToken);
            await unitOfWork.CommitTransactionAsync(cancellationToken);


            await publisher.PublishAsync(BuildingMenuMessage(menu), MenuRoutingKey.Deleted, cancellationToken);
        }
        catch (Exception)
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    private MenuSyncEvent BuildingMenuMessage(Menu menu) => new MenuSyncEvent { MenuId = menu.Id };

}
