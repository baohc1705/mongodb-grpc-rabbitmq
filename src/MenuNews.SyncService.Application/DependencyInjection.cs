using FluentValidation;
using MenuNews.SyncService.Application.Common.Behaviors;
using MenuNews.SyncService.Application.Mappers;
using Microsoft.Extensions.DependencyInjection;

namespace MenuNews.SyncService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationService(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile(typeof(ApplicationMappingProfile));
        });

        return services;
    }
}
