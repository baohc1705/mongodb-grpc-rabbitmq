using AutoMapper;
using MediatR;
using MenuNews.SyncService.Application.Common.Exceptions;
using MenuNews.SyncService.Application.Common.Interfaces;
using MenuNews.SyncService.Application.Constants;
using MenuNews.SyncService.Application.DTOs;
using MenuNews.SyncService.Domain.Entities;
using MenuNews.SyncService.Domain.Enums;
using MenuNews.SyncService.Domain.Events;
using System.Text.Json;

namespace MenuNews.SyncService.Application.Features.News.Commands.CreateNewsWithMenusOutbox;

public class CreateNewsWithMenusOutboxCommandHandler : IRequestHandler<CreateNewsWithMenusOutboxCommand, NewsDto>
{
    private readonly IUnitOfWork unitOfWork;
    private readonly INewsRepository newsRepository;
    private readonly INewsMenuRepository newsMenuRepository;
    private readonly IMenuRepository menuRepository;
    private readonly IOutboxMessageRepository outboxMessageRepository;
    private readonly IMapper mapper;

    public CreateNewsWithMenusOutboxCommandHandler(
        IMapper mapper,
        IUnitOfWork unitOfWork,
        INewsRepository newsRepository,
        INewsMenuRepository newsMenuRepository,
        IMenuRepository menuRepository,
        IOutboxMessageRepository outboxMessageRepository)
    {
        this.mapper = mapper;
        this.unitOfWork = unitOfWork;
        this.newsRepository = newsRepository;
        this.newsMenuRepository = newsMenuRepository;
        this.menuRepository = menuRepository;
        this.outboxMessageRepository = outboxMessageRepository;
    }

    public async Task<NewsDto> Handle(CreateNewsWithMenusOutboxCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var menuSlugs = request.MenuItems.Select(m => m.Slug).ToList();

            await ValidateDuplicateMenuSlugs(menuSlugs);

            if (await newsRepository.ExistsAsync(n => n.Slug.Equals(request.Slug)))
                throw new BusinessException($"News has slug [{request.Slug}] existed");

            var newsEntity = Domain.Entities.News.Create(
                request.Title,
                request.Slug,
                request.Summary,
                request.Content,
                request.Thumbnail,
                request.PublishedAt
            );

            await unitOfWork.BeginTransactionAsync(cancellationToken);

            await newsRepository.AddAsync(newsEntity);

            var (menuEntities, juctions) = SaveMenuAndJunctions(request.MenuItems, newsEntity);

            await menuRepository.AddRangeAsync(menuEntities);
            await newsMenuRepository.AddRangeAsync(juctions);

            var newsEvent = BuildNewsSyncEvent(newsEntity, menuEntities, juctions);
            var payload = JsonSerializer.Serialize(newsEvent);

            var outboxMessage = OutboxMessage.Create(NewsRountingKey.Inserted, payload);

            await outboxMessageRepository.AddAsync(outboxMessage);

            await unitOfWork.SaveChangesAsync(cancellationToken);
            await unitOfWork.CommitTransactionAsync(cancellationToken);

            return mapper.Map<NewsDto>(newsEntity);
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

    private NewsSyncEvent BuildNewsSyncEvent(
        Domain.Entities.News news,
        List<Menu> menuEntities,
        List<NewsMenu> junctions)
    {
        var menus = menuEntities.Select(menu =>
        {
            var junction = junctions.First(j => j.MenuId == menu.Id);
            return new MenuSyncItem
            {
                MenuId = menu.Id,
                Name = menu.Name,
                Slug = menu.Slug,
                DisplayOrder = menu.DisplayOrder,
                NmDisplayOrder = junction.DisplayOrder
            };
        }).ToList();

        return new NewsSyncEvent
        {
            EventType = SyncEventType.UPSERT,
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
            CreatedAt = news.CreatedAt,
            UpdatedAt = news.UpdatedAt,
            Menus = menus
        };
    }

    private (List<Menu>, List<NewsMenu>) SaveMenuAndJunctions(List<MenusItemRequest> menuItems, Domain.Entities.News newsEntity)
    {
        var menuEntities = new List<Menu>();
        var junctions = new List<NewsMenu>();

        foreach (var item in menuItems)
        {
            var menu = Menu.Create(item.Name, item.Slug, item.DisplayOrder);
            menuEntities.Add(menu);

            var junction = NewsMenu.Create(menu.Id, newsEntity.Id, item.DisplayOrder);
            junctions.Add(junction);
        }

        return (menuEntities, junctions);
    }

    private async Task ValidateDuplicateMenuSlugs(List<string> menuSlugs)
    {
        if (menuSlugs.Count != menuSlugs.Distinct().Count())
            throw new BusinessException($"Menu slugs duplicate in request");
        var menuSlugExisted = await menuRepository.GetAllAsync(m => menuSlugs.Contains(m.Slug));
        if (menuSlugExisted.Any())
            throw new BusinessException($"Menu has slugs existed");
    }
}
