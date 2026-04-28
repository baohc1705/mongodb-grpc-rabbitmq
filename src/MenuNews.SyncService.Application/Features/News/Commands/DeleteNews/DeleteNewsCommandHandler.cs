using MediatR;
using MenuNews.SyncService.Application.Common.Interfaces;
using MenuNews.SyncService.Application.Constants;
using MenuNews.SyncService.Domain.Events;
using System.Text.Json;

namespace MenuNews.SyncService.Application.Features.News.Commands.DeleteNews;

public class DeleteNewsCommandHandler : IRequestHandler<DeleteNewsCommand>
{
    private readonly IUnitOfWork unitOfWork;
    private readonly INewsRepository newsRepository;
    private readonly INewsMenuRepository newsMenuRepository;
    private readonly IOutboxMessageRepository outboxMessageRepository;

    public DeleteNewsCommandHandler(
        IUnitOfWork unitOfWork, 
        INewsRepository newsRepository, 
        INewsMenuRepository newsMenuRepository, 
        IOutboxMessageRepository outboxMessageRepository)
    {
        this.unitOfWork = unitOfWork;
        this.newsRepository = newsRepository;
        this.newsMenuRepository = newsMenuRepository;
        this.outboxMessageRepository = outboxMessageRepository;
    }

    public async Task Handle(DeleteNewsCommand request, CancellationToken cancellationToken)
    {
        var news = await newsRepository.GetAsync(n => n.Id.Equals(request.Id), cancellationToken)
            ?? throw new Exception($"Not found with id=[{request.Id}]");
        var junctions = await newsMenuRepository.GetAllAsync(nm => nm.NewsId.Equals(news.Id), cancellationToken);
        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            if (junctions.Any())
            {
                newsMenuRepository.RemoveRange(junctions);
            }

            newsRepository.Remove(news);

            var payload = JsonSerializer.Serialize(BuilderEvent(news.Id));
            var outbox = OutboxMessage.Create(NewsRountingKey.Deleted, payload);

            await outboxMessageRepository.AddAsync(outbox, cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);
            await unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch (Exception)
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    private NewsSyncEvent BuilderEvent(Guid id) => new NewsSyncEvent { Id = id };
}
