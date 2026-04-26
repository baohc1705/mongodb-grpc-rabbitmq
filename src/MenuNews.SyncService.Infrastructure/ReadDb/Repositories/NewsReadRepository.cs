using MenuNews.SyncService.Application.Common.Interfaces;
using MenuNews.SyncService.Application.DTOs;
using MenuNews.SyncService.Domain.Entities;
using MenuNews.SyncService.Domain.Enums;
using MenuNews.SyncService.Infrastructure.ReadDb.ReadModels;
using MongoDB.Driver;

namespace MenuNews.SyncService.Infrastructure.ReadDb.Repositories;

public class NewsReadRepository : INewsReadRepository
{
    private readonly IMongoCollection<NewsReadModel> collection;
    public NewsReadRepository(MongoDbContext context)
    {
        this.collection = context.NewsReadModel;
    }
    public async Task<IReadOnlyList<NewsDetailDto>> GetAllAsync(CancellationToken ct = default)
    {
        var news = await collection.Find(_ => true).ToListAsync();
        return news.Select(MapToDto).ToList();
    }

    public async Task<News?> GetByIdAsync(Guid newsId, CancellationToken ct = default)
    {
        var news = await collection.Find(n => n.Id.Equals(newsId)).FirstOrDefaultAsync();
        return MapToDomain(news);
    }

    private News? MapToDomain(NewsReadModel news)
    {
        return new News
        {
            Id = news.Id,
            Title = news.Title,
            Slug = news.Slug,
            Summary = news.Summary,
            Content = news.Content,
            Thumbnail = news.Thumbnail,
            Status = Enum.Parse<NewsStatus>(news.Status),
            PublishedAt = news.PublishedAt,
            ViewCount = news.ViewCount,
            IsActive = news.IsActive,
            CreatedAt = news.CreatedAt,
            UpdatedAt = news.UpdatedAt
        };
    }

    private NewsDetailDto MapToDto(NewsReadModel model)
    {
        return new NewsDetailDto
        (
            model.Id,
            model.Title,
            model.Slug,
            model.Summary,
            model.Content,
            model.Thumbnail,
            model.Status,
            model.PublishedAt,
            model.ViewCount,
            model.IsActive,
            model.CreatedAt,
            model.Menus.Select(m => new MenuEmbeddedDto(
                m.MenuId,
                m.Name,
                m.Slug,
                m.DisplayOrder,
                m.NmDisplayOrder
            )).ToList()
        );
    }
}
