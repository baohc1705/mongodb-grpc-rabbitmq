using MenuNews.SyncService.Application.Common.Interfaces;
using MenuNews.SyncService.Infrastructure.Postgres.Persistence.Data;
using MenuNews.SyncService.Infrastructure.Postgres.Persistence.Repositories;
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

        services.AddScoped<IMenuRepository, MenuPostgresRepository>();
        services.AddScoped<INewsRepository, NewsPostgresRepository>();
        services.AddScoped<INewsMenuRepository, NewsMenuPostgresRepository>();
        return services;
    }
}
