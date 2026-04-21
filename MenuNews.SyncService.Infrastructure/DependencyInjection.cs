using MenuNews.SyncService.Domain.Interfaces;
using MenuNews.SyncService.Infrastructure.Persistence.WriteDb;
using MenuNews.SyncService.Infrastructure.Persistence.WriteDb.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MenuNews.SyncService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
    {
        // Connection sql server
        services.AddDbContext<AppDbContext>(option =>
        {
            option.UseSqlServer(configuration.GetConnectionString(typeof(AppDbContext).Name));
        });

        services.AddScoped<IUnitOfWork, UnitOfWork >();
        services.AddScoped<IMenuRepository, MenuRepository>();
        services.AddScoped<INewsRepository, NewsRepository>();
        services.AddScoped<INewsMenuRepository, NewsMenuRepository>();
        return services;
    }
}
