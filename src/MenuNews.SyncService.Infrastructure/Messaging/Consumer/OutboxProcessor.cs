using MenuNews.SyncService.Application.Common.Interfaces;
using MenuNews.SyncService.Application.Constants;
using MenuNews.SyncService.Domain.Events;
using MenuNews.SyncService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace MenuNews.SyncService.Infrastructure.Messaging.Consumer;

public class OutboxProcessor : BackgroundService
{
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly ILogger<OutboxMessage> logger;

    public OutboxProcessor(
        IServiceScopeFactory serviceScopeFactory, 
       
        ILogger<OutboxMessage> logger)
    {
        this.serviceScopeFactory = serviceScopeFactory;
       
        this.logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var publisher = scope.ServiceProvider.GetRequiredService<IRabbitMqPublisher>();
            var pendingMessage = await context.OutboxMessages
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
        if (eventType == NewsRountingKey.Upserted)
        {
            var @event = JsonSerializer.Deserialize<NewsSyncEvent>(payload)
                ?? throw new InvalidOperationException($"Failed to deserialize payload for eventType='{eventType}'");
            return publisher.PublishAsync(@event, eventType, ct);
        }

        throw new InvalidOperationException($"No handler registered for eventType='{eventType}'");
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);
    }
}
