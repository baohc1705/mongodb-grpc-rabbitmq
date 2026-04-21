using MenuNews.SyncService.Application.Common.Mappers;
using Microsoft.Extensions.DependencyInjection;

namespace MenuNews.SyncService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationService(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });

        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile(typeof(ApplicationMappingProfile));
        });

        return services;
    }
}
