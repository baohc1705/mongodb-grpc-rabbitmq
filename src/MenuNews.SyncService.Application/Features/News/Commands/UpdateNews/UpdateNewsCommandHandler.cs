using AutoMapper;
using MediatR;
using MenuNews.SyncService.Application.Common.Interfaces;
using MenuNews.SyncService.Application.Constants;
using MenuNews.SyncService.Application.DTOs;
using MenuNews.SyncService.Domain.Events;
using System.Text.Json;

namespace MenuNews.SyncService.Application.Features.News.Commands.UpdateNews;

public class UpdateNewsCommandHandler : IRequestHandler<UpdateNewsCommand, NewsDto>
{
    private readonly IUnitOfWork unitOfWork;
    private readonly INewsReadRepository newsReadRepository;
    private readonly INewsRepository newsRepository;
    private readonly INewsMenuRepository newsMenuRepository;
    private readonly IOutboxMessageRepository outboxMessageRepository;
    private readonly IMapper mapper;

    public UpdateNewsCommandHandler(
        IUnitOfWork unitOfWork,
        INewsReadRepository newsReadRepository,
        IMapper mapper,
        INewsRepository newsRepository,
        INewsMenuRepository newsMenuRepository,
        IOutboxMessageRepository outboxMessageRepository)
    {
        this.unitOfWork = unitOfWork;
        this.newsReadRepository = newsReadRepository;
        this.mapper = mapper;
        this.newsRepository = newsRepository;
        this.newsMenuRepository = newsMenuRepository;
        this.outboxMessageRepository = outboxMessageRepository;
    }

    public async Task<NewsDto> Handle(UpdateNewsCommand request, CancellationToken cancellationToken)
    {
        var news = await newsReadRepository.GetByIdAsync(request.Id)
             ?? throw new Exception("Not found");
        if (await newsRepository.ExistsAsync(n=>n.Slug.Equals(request.Slug)))
        {
            throw new Exception("Slug existed");
        }

        var newsEntityUpdate = BuilderNewsEntity(request, news);

        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            newsRepository.Update(newsEntityUpdate);

            var newsMessage = BuildMessage(newsEntityUpdate);

            var newsPayload = JsonSerializer.Serialize(newsEntityUpdate);
            var outboxMessage = OutboxMessage.Create(NewsRountingKey.Updated, newsPayload);
            await outboxMessageRepository.AddAsync(outboxMessage, cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);
            await unitOfWork.CommitTransactionAsync(cancellationToken);
            return mapper.Map<NewsDto>(newsEntityUpdate);
        }
        catch (Exception)
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }


    private NewsSyncEvent BuildMessage(Domain.Entities.News newsEntityUpdate)
    {
        return new NewsSyncEvent
        {
            EventType = Domain.Enums.SyncEventType.UPDATE,
            Id = newsEntityUpdate.Id,
            Title = newsEntityUpdate.Title,
            Content = newsEntityUpdate.Content,
            Thumbnail = newsEntityUpdate.Thumbnail,
            PublishedAt = newsEntityUpdate.PublishedAt,
            UpdatedAt = newsEntityUpdate.UpdatedAt,
        };
    }

    private Domain.Entities.News BuilderNewsEntity(UpdateNewsCommand request, Domain.Entities.News news)
    {

        news.Title = request.Title;
        news.Slug = request.Slug;
        news.Content = request.Content;
        news.Thumbnail = request.Thumbnail;
        news.PublishedAt = request.PublishedAt;
        news.UpdatedAt = DateTime.UtcNow;
        return news;
    }
}
