using MenuNews.SyncService.Application.Common.Interfaces.UnitOfWork;
using MenuNews.SyncService.Infrastructure.Persistence.UnitOfWork;
using MenuNews.SyncService.Infrastructure.Postgres.Persistence.Data;

namespace MenuNews.SyncService.Infrastructure.Postgres.Persistence.Repositories.UnitOfWork;

public class PostgresUnitOfWork : BaseUnitOfWork<PostgresDbContext>, IPostgresUnitOfWork
{
    public PostgresUnitOfWork(PostgresDbContext context) : base(context)
    {
    }
}
