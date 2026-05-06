using MenuNews.SyncService.Application.Common.Interfaces;
using MenuNews.SyncService.Domain.Entities;
using MenuNews.SyncService.Infrastructure.Postgres.Persistence.Data;

namespace MenuNews.SyncService.Infrastructure.Postgres.Persistence.Repositories;

public class NewsMenuPostgresRepository : BasePostgresRepository<NewsMenu>, INewsMenuRepository
{
    public NewsMenuPostgresRepository(PostgresDbContext context) : base(context)
    {
    }

    public Task SoftDeleteByMenuIdAsync(Guid menuId, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task SoftDeleteByNewsIdAsync(Guid newsId, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}
