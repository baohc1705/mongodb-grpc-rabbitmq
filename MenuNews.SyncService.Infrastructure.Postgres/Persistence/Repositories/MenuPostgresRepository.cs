using MenuNews.SyncService.Application.Common.Interfaces;
using MenuNews.SyncService.Domain.Entities;
using MenuNews.SyncService.Infrastructure.Postgres.Persistence.Data;

namespace MenuNews.SyncService.Infrastructure.Postgres.Persistence.Repositories;

public class MenuPostgresRepository : BasePostgresRepository<Menu>, IMenuRepository
{
    public MenuPostgresRepository(PostgresDbContext context) : base(context)
    {
    }
}
