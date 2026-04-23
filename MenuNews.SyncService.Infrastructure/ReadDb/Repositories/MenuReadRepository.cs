using MenuNews.SyncService.Application.Common.Interfaces;
using MenuNews.SyncService.Application.DTOs;
using MenuNews.SyncService.Infrastructure.ReadDb.ReadModels;
using MongoDB.Driver;

namespace MenuNews.SyncService.Infrastructure.ReadDb.Repositories;

public sealed class MenuReadRepository : IMenuReadRepository
{
    private readonly MongoDbContext context;
    private readonly IMongoCollection<MenuReadModel> collection;

    public MenuReadRepository(MongoDbContext context)
    {
        this.context = context;
        collection = this.context.MenuReadModel;
    }

    public async Task<IReadOnlyCollection<MenuDetailDto>> GetAllAsync(CancellationToken ct = default)
    {
        var menus = await collection
            .Find(_ => true)
            .SortBy(m => m.DisplayOrder)
            .ToListAsync(ct);
        return menus.Select(MapToDto).ToList();
    }

    private static MenuDetailDto MapToDto(MenuReadModel doc)
    {
        return new MenuDetailDto(
            doc.Id, 
            doc.Name, 
            doc.Slug, 
            doc.DisplayOrder, 
            doc.IsActive, 
            doc.CreatedAt,
            doc.News.Select(n => new NewsEmbeddedDto(
                n.NewsId, 
                n.Title, 
                n.Slug, 
                n.Summary, 
                n.Thumbnail,
                n.Status, 
                n.PublishedAt, 
                n.ViewCount, 
                n.CreatedAt, 
                n.DisplayOrder
        )).ToList());
    }
}
