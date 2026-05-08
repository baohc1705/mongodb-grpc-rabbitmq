using AutoMapper;
using MediatR;
using MenuNews.SyncService.Application.Common.Exceptions;
using MenuNews.SyncService.Application.Common.Interfaces.PostgresRepository;
using MenuNews.SyncService.Application.Common.Interfaces.UnitOfWork;
using MenuNews.SyncService.Application.Constants;
using MenuNews.SyncService.Application.DTOs;
using MenuNews.SyncService.Domain.Entities;
using MenuNews.SyncService.Domain.Events;
using System.Text.Json;
using System.Xml.Linq;

namespace MenuNews.SyncService.Application.Features.Menus.Commands.CreateMenuWithNewsPostgres;

public class CreateMenuWithNewsPostgresCommandHandler : IRequestHandler<CreateMenuWithNewsPostgresCommand, MenuDto>
{
    private readonly IMenuPostgresRepository menuRepository;
    private readonly INewsPostgresRepository newsRepository;
    private readonly INewsMenuPostgresRepository newsMenuRepository;
    private readonly IOutboxMessagePostgresRepository outboxMessageRepository;
    private readonly IPostgresUnitOfWork unitOfWork;
    private readonly IMapper mapper;

    public CreateMenuWithNewsPostgresCommandHandler(
        IMenuPostgresRepository menuRepository,
        INewsPostgresRepository newsRepository,
        INewsMenuPostgresRepository newsMenuRepository,
        IOutboxMessagePostgresRepository outboxMessageRepository,
        IPostgresUnitOfWork unitOfWork,
        IMapper mapper)
    {
        this.menuRepository = menuRepository;
        this.newsRepository = newsRepository;
        this.newsMenuRepository = newsMenuRepository;
        this.outboxMessageRepository = outboxMessageRepository;
        this.unitOfWork = unitOfWork;
        this.mapper = mapper;
    }

    public async Task<MenuDto> Handle(CreateMenuWithNewsPostgresCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var requestSlugs = request.NewsItems.Select(x => x.Slug).ToList();
            ValidateRequestSlugs(requestSlugs);
            await ValidateNewsSlugsAsync(requestSlugs, cancellationToken);

            if (await menuRepository.ExistsAsync(m => m.Slug.Equals(request.Slug), cancellationToken))
            {
                throw new BusinessException($"Menu with slug {request.Slug} already exists.");
            }

            var menuEntity = Domain.Entities.Menu.Create(request.Name, request.Slug, request.DisplayOrder);

            await unitOfWork.BeginTransactionAsync(cancellationToken);

            await menuRepository.AddAsync(menuEntity, cancellationToken);

            var (newsEntities, junctions) = ProcessNewsItems(request.NewsItems, menuEntity);

            await newsRepository.AddRangeAsync(newsEntities, cancellationToken);
            await newsMenuRepository.AddRangeAsync(junctions, cancellationToken);

            var menuEvent = BuildMenuSyncEvent(menuEntity, newsEntities, junctions); 
            var payload = JsonSerializer.Serialize(menuEvent);
            var outbox = OutboxMessage.Create(MenuRoutingKey.Inserted, payload);

            await outboxMessageRepository.AddAsync(outbox);

            await unitOfWork.SaveChangesAsync(cancellationToken);
            await unitOfWork.CommitTransactionAsync(cancellationToken);

            return mapper.Map<MenuDto>(menuEntity);
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    private (List<Domain.Entities.News> , List<NewsMenu>) ProcessNewsItems(List<CreateNewsItemsPostgresRequest> newsItems, Menu menuEntity)
    {
        var newsEntities = new List<Domain.Entities.News>();
        var junctions = new List<NewsMenu>();

        foreach(var item in newsItems)
        {
            var news = Domain.Entities.News.Create(
                item.Title, 
                item.Slug, 
                item.Summary, 
                item.Content, 
                item.Thumbnail, 
                item.PublishedAt
            );
            
            newsEntities.Add(news);

            var junction = NewsMenu.Create(menuEntity.Id, news.Id, item.DisplayOrder);
            junctions.Add(junction);
        }

        return (newsEntities, junctions);
    }

    private MenuSyncEvent BuildMenuSyncEvent(
        Menu menu, 
        List<Domain.Entities.News> newsEntities, 
        List<NewsMenu> junctions)
    {
        var newsList = newsEntities.Select(news =>
        {
            var junction = junctions.FirstOrDefault(j => j.NewsId == news.Id);
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
                DisplayOrder = junction?.DisplayOrder ?? 0
            };
        }).ToList();

        return new MenuSyncEvent
        {
            EventType = MenuRoutingKey.Inserted,
            Id = menu.Id,
            Name = menu.Name,
            Slug = menu.Slug,
            DisplayOrder = menu.DisplayOrder,
            IsActive = menu.IsActive,
            CreatedAt = menu.CreatedAt,
            UpdatedAt = menu.UpdatedAt,
            News = newsList
        };
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
}
