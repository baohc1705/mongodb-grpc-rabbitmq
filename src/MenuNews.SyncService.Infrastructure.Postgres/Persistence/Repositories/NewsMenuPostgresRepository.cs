using MenuNews.SyncService.Application.Common.Interfaces.PostgresRepository;
using MenuNews.SyncService.Domain.Entities;
using MenuNews.SyncService.Infrastructure.Persistence.Repositories;
using MenuNews.SyncService.Infrastructure.Postgres.Persistence.Data;

namespace MenuNews.SyncService.Infrastructure.Postgres.Persistence.Repositories;

public class NewsMenuPostgresRepository : BaseRepository<NewsMenu, PostgresDbContext>, INewsMenuPostgresRepository
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
