using MenuNews.SyncService.Application.Common.Interfaces.PostgresRepository;
using MenuNews.SyncService.Domain.Entities;
using MenuNews.SyncService.Infrastructure.Persistence.Repositories;
using MenuNews.SyncService.Infrastructure.Postgres.Persistence.Data;

namespace MenuNews.SyncService.Infrastructure.Postgres.Persistence.Repositories;

public class NewsPostgresRepository : BaseRepository<News, PostgresDbContext>, INewsPostgresRepository
{
    public NewsPostgresRepository(PostgresDbContext context) : base(context)
    {
    }
}
