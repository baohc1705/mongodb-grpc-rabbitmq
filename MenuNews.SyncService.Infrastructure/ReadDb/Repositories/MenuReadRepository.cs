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

    public async Task<(List<MenuDetailDto> Items, long TotalCount)> GetAllAsync(bool activeOnly, int skip, int take, CancellationToken ct = default)
    {
        var filter = activeOnly
            ? Builders<MenuReadModel>.Filter.Eq(m => m.IsActive, true)
            : Builders<MenuReadModel>.Filter.Empty;

        var total = await collection.CountDocumentsAsync(filter, cancellationToken: ct);
        var docs = await collection
            .Find(filter)
            .SortBy(m => m.DisplayOrder)
            .Skip(skip)
            .Limit(take)
            .ToListAsync(ct);
        return (docs.Select(MapToDto).ToList(), total);
    }

    public async Task<IReadOnlyCollection<MenuDetailDto>> GetAllAsync(CancellationToken ct = default)
    {
        var menus = await collection
            .Find(_ => true)
            .SortBy(m => m.DisplayOrder)
            .ToListAsync(ct);
        return menus.Select(MapToDto).ToList();
    }

    public async Task<MenuDetailDto?> GetByIdAsync(Guid menuId, CancellationToken ct = default)
    {
        var filter = Builders<MenuReadModel>.Filter.Eq(m => m.Id, menuId);
        var docs = await collection
            .Find(filter)
            .FirstOrDefaultAsync(ct);
        return docs is null ? null : MapToDto(docs);
    }

    private static MenuDetailDto MapToDto(MenuReadModel doc) => new(
        doc.Id, doc.Name, doc.Slug, doc.DisplayOrder, doc.IsActive, doc.CreatedAt,
        doc.News.Select(n => new NewsEmbeddedDto(
            n.NewsId, n.Title, n.Slug, n.Summary, n.Thumbnail,
            n.Status, n.PublishedAt, n.ViewCount, n.CreatedAt, n.DisplayOrder
        )).ToList()
    );
}
