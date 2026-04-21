using MediatR;
using MenuNews.SyncService.Domain.Entities;
using MenuNews.SyncService.Domain.Interfaces;

namespace MenuNews.SyncService.Application.Features.Menus.Commands.CreateMenu;

public class CreateMenuCommandHandler : IRequestHandler<CreateMenuCommand, Guid>
{
    private readonly IUnitOfWork unitOfWork;

    public CreateMenuCommandHandler(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateMenuCommand request, CancellationToken cancellationToken)
    {
        await unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var menu = new Menu
            {
                Name = request.Name,
                Slug = request.Slug,
                DisplayOrder = request.DisplayOrder,
            };

            menu.Active = true;
            menu.CreatedAt = DateTime.UtcNow;
            
            var newsItem = new List<(Domain.Entities.News news, int order)>();
            foreach (var item in request.NewsItems)
            {
                var news = new Domain.Entities.News
                {
                    Title = item.Title,
                    Slug = item.Slug,
                    Summary = item.Summary,
                    Content = item.Content,
                    Thumbnail = item.Thumbnail,
                    PublishedAt = item.PublishedAt
                };

                news.CreatedAt = DateTime.UtcNow;
                news.Active = true;
                news.ViewCount = 0;

                await unitOfWork.NewsRepository.AddAsync(news, cancellationToken);

                newsItem.Add((news, item.DisplayOrder));
            }

            await unitOfWork.MenuRepository.AddAsync(menu, cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            foreach (var (news, order) in newsItem)
            {
                var newsMenu = new NewsMenu
                {
                    MenuId = menu.Id,
                    NewsId = news.Id,
                    DisplayOrder = order
                };
                await unitOfWork.NewsMenuRepository.AddAsync(newsMenu, cancellationToken);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
            await unitOfWork.CommitTransactionAsync(cancellationToken);

            return menu.Id;
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw new ApplicationException("An error occurred while creating the menu.", ex);
        }
    }
}
