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
    private readonly IMenuRepository menuRepository;
    private readonly INewsMenuRepository newsMenuRepository;
    private readonly INewsRepository newsRepository;
    private readonly IRabbitMqPublisher publisher;
    private readonly IMapper mapper;

    public CreateMenuWithNewsCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IRabbitMqPublisher publisher,
        IMenuRepository menuRepository,
        INewsMenuRepository newsMenuRepository,
        INewsRepository newsRepository)
    {
        this.unitOfWork = unitOfWork;
        this.mapper = mapper;
        this.publisher = publisher;
        this.menuRepository = menuRepository;
        this.newsMenuRepository = newsMenuRepository;
        this.newsRepository = newsRepository;
    }

    public async Task<MenuDto> Handle(CreateMenuWithNewsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var requestSlugs = request.NewsItems.Select(x => x.Slug).ToList();
            ValidateRequestSlugs(requestSlugs);
            await ValidateNewsSlugsAsync(requestSlugs, cancellationToken);

            await unitOfWork.BeginTransactionAsync(cancellationToken);


            var menu = await CreateAndValidateMenuAsync(request.Name, request.Slug, request.DisplayOrder, cancellationToken);

            var (newsEntities, junctions, newsSyncItems, newsSyncEvents) = ProcessNewsItems(request.NewsItems, menu);

            await newsRepository.AddRangeAsync(newsEntities, cancellationToken);
            await newsMenuRepository.AddRangeAsync(junctions, cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            await unitOfWork.CommitTransactionAsync(cancellationToken);

            await PublishSyncEventsAsync(menu, newsSyncItems, newsSyncEvents, cancellationToken);

            return mapper.Map<MenuDto>(menu);
        }
        catch (BusinessException)
        {

            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw new Exception("Failed to create menu with news. Transaction rolled back.", ex);
        }
    }

    private void ValidateRequestSlugs(List<string> requestSlugs)
    {
        if (requestSlugs.Count != requestSlugs.Distinct().Count())
        {
            throw new BusinessException("Duplicate slugs found in the news items list.");
        }
    }

    private async Task ValidateNewsSlugsAsync(List<string> requestSlugs, CancellationToken cancellationToken)
    {
        var existingNews = await newsRepository.GetAllAsync(n => requestSlugs.Contains(n.Slug), cancellationToken);
        if (existingNews.Any())
        {
            var duplicateSlugs = string.Join(", ", existingNews.Select(n => n.Slug));
            throw new BusinessException($"News with the following slugs already exist: {duplicateSlugs}");
        }
    }

    private async Task<Domain.Entities.Menu> CreateAndValidateMenuAsync(string name, string slug, int displayOrder, CancellationToken cancellationToken)
    {
        if (await menuRepository.ExistsAsync(m => m.Slug.Equals(slug), cancellationToken))
        {
            throw new BusinessException($"Menu with slug {slug} already exists.");
        }

        var menu = Domain.Entities.Menu.Create(name, slug, displayOrder);
        await menuRepository.AddAsync(menu, cancellationToken);
        return menu;
    }

    private (List<Domain.Entities.News>, List<NewsMenu>, List<NewsSyncItem>, List<NewsSyncEvent>) ProcessNewsItems(
        List<CreateNewsItemsRequest> requestNewsItems,
        Domain.Entities.Menu menu)
    {
        var newsEntities = new List<Domain.Entities.News>();
        var junctions = new List<NewsMenu>();
        var newsSyncItems = new List<NewsSyncItem>();
        var newsSyncEvents = new List<NewsSyncEvent>();

        foreach (var item in requestNewsItems)
        {
            var news = Domain.Entities.News.Create(
                item.Title,
                item.Slug,
                item.Summary,
                item.Content,
                item.Thumbnail,
                item.PublishedAt
            );

            var junction = NewsMenu.Create(menu.Id, news.Id, item.DisplayOrder);

            newsEntities.Add(news);
            junctions.Add(junction);

            newsSyncItems.Add(BuildNewsSyncItem(news, item.DisplayOrder));
            newsSyncEvents.Add(BuildNewsSyncEvent(news, menu, item.DisplayOrder));
        }

        return (newsEntities, junctions, newsSyncItems, newsSyncEvents);
    }

    private NewsSyncItem BuildNewsSyncItem(Domain.Entities.News news, int displayOrder)
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

    private NewsSyncEvent BuildNewsSyncEvent(Domain.Entities.News news, Domain.Entities.Menu menu, int displayOrder)
    {
        return new NewsSyncEvent
        {
            EventType = Domain.Enums.SyncEventType.UPSERT,
            Id = news.Id,
            Title = news.Title,
            Slug = news.Slug,
            Summary = news.Summary,
            Content = news.Content,
            Thumbnail = news.Thumbnail,
            Status = news.Status,
            PublishedAt = news.PublishedAt,
            ViewCount = news.ViewCount,
            IsActive = news.IsActive,
            DisplayOrder = displayOrder,
            CreatedAt = news.CreatedAt,
            Menus = new List<MenuSyncItem>
            {
                new MenuSyncItem
                {
                    MenuId = menu.Id,
                    Name = menu.Name,
                    Slug = menu.Slug,
                    DisplayOrder = menu.DisplayOrder,
                    NmDisplayOrder = displayOrder
                }
            }
        };
    }

    private async Task PublishSyncEventsAsync(
        Domain.Entities.Menu menu,
        List<NewsSyncItem> newsSyncItems,
        List<NewsSyncEvent> newsSyncEvents,
        CancellationToken cancellationToken)
    {
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

        await publisher.PublishAsync(menuSyncEvent, MenuRoutingKey.Inserted, cancellationToken);

        foreach (var newsEvent in newsSyncEvents)
        {
            await publisher.PublishAsync(newsEvent, NewsRountingKey.Inserted, cancellationToken);
        }
    }
}
