using MenuNews.SyncService.Application.Common.Interfaces;
using MenuNews.SyncService.Domain.Entities;
using MenuNews.SyncService.Infrastructure.Postgres.Persistence.Data;

namespace MenuNews.SyncService.Infrastructure.Postgres.Persistence.Repositories;

public class NewsPostgresRepository : BasePostgresRepository<News>, INewsRepository
{
    public NewsPostgresRepository(PostgresDbContext context) : base(context)
    {
    }
}
