using MenuNews.SyncService.Application.Common.Interfaces.PostgresRepository;
using MenuNews.SyncService.Domain.Entities;
using MenuNews.SyncService.Infrastructure.Persistence.Repositories;
using MenuNews.SyncService.Infrastructure.Postgres.Persistence.Data;

namespace MenuNews.SyncService.Infrastructure.Postgres.Persistence.Repositories;

public class MenuPostgresRepository : BaseRepository<Menu, PostgresDbContext>, IMenuPostgresRepository
{
    public MenuPostgresRepository(PostgresDbContext context) : base(context)
    {
    }
}
