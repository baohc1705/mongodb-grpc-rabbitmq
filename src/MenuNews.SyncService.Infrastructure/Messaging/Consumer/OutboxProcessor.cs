using MenuNews.SyncService.Application.Common.Interfaces.Messaging;
using MenuNews.SyncService.Domain.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace MenuNews.SyncService.Infrastructure.Messaging.Consumer;

public abstract class OutboxProcessor<TOutbox, TDbContext> : BackgroundService
    where TOutbox : OutboxMessage
    where TDbContext : DbContext
{
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly ILogger<OutboxProcessor<TOutbox, TDbContext>> logger;
   

    public OutboxProcessor(
        IServiceScopeFactory serviceScopeFactory,

        ILogger<OutboxProcessor<TOutbox, TDbContext>> logger)
    {
        this.serviceScopeFactory = serviceScopeFactory;

        this.logger = logger;

       
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();
            var context = scope.ServiceProvider.GetRequiredService<TDbContext>();
            var publisher = scope.ServiceProvider.GetRequiredService<IRabbitMqPublisher>();

            var dbSet = context.Set<TOutbox>();

            var pendingMessage = await dbSet
                .Where(x => x.Status.Equals(OutboxStatus.PENDING))
                .ToListAsync(stoppingToken);

            foreach (var message in pendingMessage)
            {
                try
                {
                    await PublishOutboxMessageAsync(publisher, message.Payload, message.EventType, stoppingToken);
                    logger.LogInformation($"Da published outbox[{message.Id}]");
                    message.Status = OutboxStatus.PROCESSED;
                    message.ProcessedAt = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Failed to publish outbox[{message.Id}] eventType='{message.EventType}'");
                    message.Status = OutboxStatus.FAILED;
                }
            }

            await context.SaveChangesAsync(stoppingToken);
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }

    private Task PublishOutboxMessageAsync(IRabbitMqPublisher publisher, string payload, string eventType, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(eventType))
            throw new InvalidOperationException($"EventType is null or empty");

        if (eventType.StartsWith("menu."))
        {
            var message = JsonSerializer.Deserialize<MenuSyncEvent>(payload)
                    ?? throw new InvalidOperationException($"Failed to deserialize payload for eventType='{eventType}'");
            return publisher.PublishAsync(message, eventType, ct);
        }

        var newsMessage = JsonSerializer.Deserialize<NewsSyncEvent>(payload)
                ?? throw new InvalidOperationException($"Failed to deserialize payload for eventType='{eventType}'");
        return publisher.PublishAsync(newsMessage, eventType, ct);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);
    }
}
