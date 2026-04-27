using MediatR;
using MenuNews.SyncService.Application.Common.Interfaces;
using MenuNews.SyncService.Application.Constants;
using MenuNews.SyncService.Domain.Events;
using System.Text.Json;

namespace MenuNews.SyncService.Application.Features.News.Commands.DeleteNews;

public class DeleteNewsCommandHandler : IRequestHandler<DeleteNewsCommand>
{
    private readonly IUnitOfWork unitOfWork;

    public DeleteNewsCommandHandler(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteNewsCommand request, CancellationToken cancellationToken)
    {
        var news = await unitOfWork.NewsRepository.GetAsync(n => n.Id.Equals(request.Id), cancellationToken)
            ?? throw new Exception($"Not found with id=[{request.Id}]");
        var junctions = await unitOfWork.NewsMenuRepository.GetAllAsync(nm => nm.NewsId.Equals(news.Id), cancellationToken);
        if (junctions.Any())
        {
            unitOfWork.NewsMenuRepository.RemoveRange(junctions);
        }

        unitOfWork.NewsRepository.Remove(news);

        var payload = JsonSerializer.Serialize(BuilderEvent(news.Id));
        var outbox = OutboxMessage.Create(NewsRountingKey.Deleted, payload);

        await unitOfWork.OutboxMessageRepository.AddAsync(outbox, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private NewsSyncEvent BuilderEvent(Guid id) => new NewsSyncEvent { Id = id };
}
