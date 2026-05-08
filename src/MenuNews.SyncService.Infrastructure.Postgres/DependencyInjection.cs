using MenuNews.SyncService.Application.Common.Interfaces.PostgresRepository;
using MenuNews.SyncService.Application.Common.Interfaces.UnitOfWork;
using MenuNews.SyncService.Infrastructure.Messaging.Consumer;
using MenuNews.SyncService.Infrastructure.Postgres.Persistence.Data;
using MenuNews.SyncService.Infrastructure.Postgres.Persistence.Repositories;
using MenuNews.SyncService.Infrastructure.Postgres.Persistence.Repositories.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MenuNews.SyncService.Infrastructure.Postgres;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructurePostgresService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PostgresDbContext>(opt =>
        {
            opt.UseNpgsql(configuration.GetConnectionString(nameof(PostgresDbContext)));
        });

        services.AddScoped<IMenuPostgresRepository, MenuPostgresRepository>();
        services.AddScoped<INewsPostgresRepository, NewsPostgresRepository>();
        services.AddScoped<INewsMenuPostgresRepository, NewsMenuPostgresRepository>();
        services.AddScoped<IPostgresUnitOfWork, PostgresUnitOfWork>();
        services.AddScoped<IOutboxMessagePostgresRepository, OutboxMessagePostgresRepository>();

        services.AddHostedService<PostgresOutboxProcessor>();
        return services;
    }
}
