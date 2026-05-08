using MenuNews.SyncService.Application.Common.Interfaces.UnitOfWork;

namespace MenuNews.SyncService.Infrastructure.Persistence.UnitOfWork;

public class SqlUnitOfWork : BaseUnitOfWork<AppDbContext>, ISqlUnitOfWork
{
    public SqlUnitOfWork(AppDbContext context) : base(context)
    {
    }
}
