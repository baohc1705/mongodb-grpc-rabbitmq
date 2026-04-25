using MenuNews.SyncService.Application.Common.Interfaces;
using MenuNews.SyncService.Infrastructure.Messaging;
using MenuNews.SyncService.Infrastructure.Messaging.Consumer;
using MenuNews.SyncService.Infrastructure.Messaging.Publisher;
using MenuNews.SyncService.Infrastructure.Messaging.Settings;
using MenuNews.SyncService.Infrastructure.Persistence;
using MenuNews.SyncService.Infrastructure.Persistence.Repositories;
using MenuNews.SyncService.Infrastructure.Persistence.UnitOfWork;
using MenuNews.SyncService.Infrastructure.ReadDb;
using MenuNews.SyncService.Infrastructure.ReadDb.ClassMaps;
using MenuNews.SyncService.Infrastructure.ReadDb.Repositories;
using MenuNews.SyncService.Infrastructure.ReadDb.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MenuNews.SyncService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
    {
        // SQL Server
        services.AddDbContext<AppDbContext>(opt =>
        {
            opt.UseSqlServer(configuration.GetConnectionString(nameof(AppDbContext)));
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IMenuRepository, MenuRepository>();
        services.AddScoped<INewsRepository, NewsRepository>();
        services.AddScoped<INewsMenuRepository, NewsMenuRepository>();

        // MongoDB
        MongoClassMaps.Register();

        services.Configure<MongoDbSettings>(
            configuration.GetSection(nameof(MongoDbSettings))
        );

        services.AddSingleton<MongoDbContext>();
        services.AddScoped<IMenuReadRepository, MenuReadRepository>();

        // Rabbit Mq
        services.Configure<RabbitMqSettings>(
            configuration.GetSection(nameof(RabbitMqSettings)));
    
        services.AddScoped<IRabbitMqPublisher, RabbitMqPublisher>();
        services.AddHostedService<MenuUpsertedConsumer>();
        services.AddHostedService<MenuUpdatedConsumer>();
        services.AddHostedService<MenuDeletedConsumer>();

        services.AddHostedService<NewsUpsertedConsumer>();
        return services;
    }
}
