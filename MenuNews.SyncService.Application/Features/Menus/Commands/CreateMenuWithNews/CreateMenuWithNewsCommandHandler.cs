using AutoMapper;
using MediatR;
using MenuNews.SyncService.Application.Common.Exceptions;
using MenuNews.SyncService.Application.Common.Interfaces;
using MenuNews.SyncService.Application.Constants;
using MenuNews.SyncService.Application.DTOs;
using MenuNews.SyncService.Domain.Entities;
using MenuNews.SyncService.Domain.Events;

namespace MenuNews.SyncService.Application.Features.Menus.Commands.CreateMenuWithNews;

public class CreateMenuWithNewsCommandHandler : IRequestHandler<CreateMenuWithNewsCommand, MenuDto>
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IRabbitMqPublisher publisher;
    private readonly IMapper mapper;
    public CreateMenuWithNewsCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IRabbitMqPublisher publisher)
    {
        this.unitOfWork = unitOfWork;
        this.mapper = mapper;
        this.publisher = publisher;
    }

    public async Task<MenuDto> Handle(CreateMenuWithNewsCommand request, CancellationToken cancellationToken)
    {
        if (await unitOfWork.MenuRepository.ExistsAsync(m => m.Slug.Equals(request.Slug), cancellationToken)) {
            throw new BusinessException($"Menu with slug {request.Slug} already exists.");
        }

        // Create menu
        var menu = Menu.Create(request.Name, request.Slug, request.DisplayOrder);
        await unitOfWork.MenuRepository.AddAsync(menu, cancellationToken);

        var newsSyncItems = new List<NewsSyncItem>();
        var newsSyncEvents = new List<NewsSyncEvent>();

        // Create news and news-menu relationship
        var requestSlugs = request.NewsItems.Select(x => x.Slug).ToList();
        if (requestSlugs.Count != requestSlugs.Distinct().Count())
        {
            throw new BusinessException("Duplicate slugs found in the news items list.");
        }

        foreach (var item in request.NewsItems)
        {
            if (await unitOfWork.NewsRepository.ExistsAsync(n => n.Slug.Equals(item.Slug), cancellationToken)) {
                throw new BusinessException($"News with slug {item.Slug} already exists.");
            }

            var news = Domain.Entities.News.Create(
                item.Title,
                item.Slug,
                item.Summary,
                item.Content,
                item.Thumbnail,
                item.PublishedAt
            );

            await unitOfWork.NewsRepository.AddAsync(news, cancellationToken);

            var junction = NewsMenu.Create(menu.Id, news.Id, item.DisplayOrder);
            await unitOfWork.NewsMenuRepository.AddAsync(junction, cancellationToken);

            newsSyncItems.Add(BuildNewsAsyncItem(news, item.DisplayOrder));

            // NewsSyncEvent: News document embeds the parent Menu

            //newsSyncEvents.Add(new NewsSyncEvent
            //{
            //    EventType = Domain.Enums.SyncEventType.UPSERT,
            //    NewsId = news.Id,
            //    Title = news.Title,
            //    Slug = news.Slug,
            //    Summary = news.Summary,
            //    Content = news.Content,
            //    Thumbnail = news.Thumbnail,
            //    Status = news.Status,
            //    PublishedAt = news.PublishedAt,
            //    ViewCount = news.ViewCount,
            //    IsActive = news.IsActive,
            //    DisplayOrder = item.DisplayOrder,
            //    CreatedAt = news.CreatedAt,
            //    Menus = new List<MenuSyncItem>
            //    {
            //        new MenuSyncItem
            //        {
            //            MenuId = menu.Id,
            //            Name = menu.Name,
            //            Slug = menu.Slug,
            //            DisplayOrder = menu.DisplayOrder,
            //            NmDisplayOrder = item.DisplayOrder
            //        }
            //    }
            //});
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var menuSyncEvent = new MenuSyncEvent
        {
            EventType = Domain.Enums.SyncEventType.UPSERT,
            MenuId = menu.Id,
            Name = menu.Name,
            Slug = menu.Slug,
            DisplayOrder = menu.DisplayOrder,
            IsActive = menu.IsActive,
            CreatedAt = menu.CreatedAt,
            News = newsSyncItems
        };

        await publisher.PublishAsync(RabbitMqConstants.MenuExchange, RabbitMqConstants.MenuSyncRoutingKey, menuSyncEvent, cancellationToken);

        //foreach (var newsEvent in newsSyncEvents)
        //{
        //    await publisher.PublishAsync(RabbitMqConstants.NewsExchange, RabbitMqConstants.NewsSyncRoutingKey, newsEvent, cancellationToken);
        //}

        return mapper.Map<MenuDto>(menu);
    }

    private NewsSyncItem BuildNewsAsyncItem(Domain.Entities.News news, int displayOrder)
    {
        return new NewsSyncItem
        {
            NewsId = news.Id,
            Title = news.Title,
            Slug = news.Slug,
            Summary = news.Summary,
            Thumbnail = news.Thumbnail,
            Status = news.Status,
            PublishedAt = news.PublishedAt,
            ViewCount = news.ViewCount,
            CreatedAt = news.CreatedAt,
            DisplayOrder = displayOrder
        };
    }
}
