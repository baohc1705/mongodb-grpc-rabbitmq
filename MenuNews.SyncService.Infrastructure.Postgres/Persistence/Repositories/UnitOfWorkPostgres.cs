using MenuNews.SyncService.Application.Common.Interfaces;
using MenuNews.SyncService.Infrastructure.Postgres.Persistence.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace MenuNews.SyncService.Infrastructure.Postgres.Persistence.Repositories;

public class UnitOfWorkPostgres : IUnitOfWork
{
    private IDbContextTransaction? transaction;
    private readonly PostgresDbContext context;

    public UnitOfWorkPostgres(PostgresDbContext context)
    {
       this.context = context;
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (transaction != null) 
            throw new Exception("Transaction has begin");
        transaction = await context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (transaction == null) 
            throw new Exception("No active transaction");
        await context.Database.CommitTransactionAsync(cancellationToken);
        await transaction.DisposeAsync();
        transaction = null;
    }

    public async ValueTask DisposeAsync()
    {
        if (transaction != null)
            await transaction.DisposeAsync();
        await context.DisposeAsync();
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (transaction == null)
            throw new Exception("No active transaction");
        await context.Database.RollbackTransactionAsync(cancellationToken);
        await transaction.DisposeAsync();
        transaction = null;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }
}
