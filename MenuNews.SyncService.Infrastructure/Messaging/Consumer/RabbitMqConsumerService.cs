using MenuNews.SyncService.Application.Constants;
using MenuNews.SyncService.Domain.Events;
using MenuNews.SyncService.Infrastructure.ReadDb;
using MenuNews.SyncService.Infrastructure.ReadDb.ReadModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace MenuNews.SyncService.Infrastructure.Messaging.Consumer;

public sealed class RabbitMqConsumerService : BackgroundService
{
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly RabbitMqConnectionManager connectionManager;
    private readonly ILogger<RabbitMqConsumerService> logger;
    private IChannel? menuChannel;
    private IChannel? newsChannel;
    public RabbitMqConsumerService(IServiceScopeFactory serviceScopeFactory, RabbitMqConnectionManager connectionManager, ILogger<RabbitMqConsumerService> logger)
    {
        this.serviceScopeFactory = serviceScopeFactory;
        this.connectionManager = connectionManager;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
         await Task.WhenAll(
             ConsumeMenuAsync(stoppingToken)
             //ConsumeNewsAsync(stoppingToken)
         );
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);

        if (menuChannel is not null)
        {
            await menuChannel.CloseAsync(cancellationToken);
            await menuChannel.DisposeAsync();
        }

    }
    private async Task ConsumeMenuAsync(CancellationToken stoppingToken)
    {
        menuChannel = await connectionManager.GetChannel(
            exchange: RabbitMqConstants.MenuExchange, 
            queue:RabbitMqConstants.MenuSyncQueue, 
            routingKey: RabbitMqConstants.MenuSyncRoutingKey, 
            stoppingToken
        );

        // Xử lý tối đa 10 message đồng thời trên channel này
        await menuChannel.BasicQosAsync(
            prefetchSize: 0,
            prefetchCount: 10,
            global: false,
            cancellationToken: stoppingToken
        );

        var consumer = new AsyncEventingBasicConsumer(menuChannel);

        consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                var message = JsonSerializer.Deserialize<MenuSyncEvent>(json);

                if (message is not null)
                    await HandelMenuSyncEventAsync(message, stoppingToken);

                await menuChannel.BasicAckAsync(
                    ea.DeliveryTag,
                    multiple: false,
                    cancellationToken: stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing MenuSyncEvent");
                // requeue: false -> đẩy vào dead-letter queue nếu có cấu hình
                await menuChannel.BasicNackAsync(
                    deliveryTag: ea.DeliveryTag,
                    multiple: false,
                    requeue: false,
                    cancellationToken: stoppingToken);
            }
        };

        await menuChannel.BasicConsumeAsync(
            queue: RabbitMqConstants.MenuSyncQueue,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);
        logger.LogInformation( $"MenuSync consumer started -> queue: {RabbitMqConstants.MenuSyncQueue}");

        await Task.Delay(Timeout.Infinite, stoppingToken).ConfigureAwait(false);
    }



    private async Task HandelMenuSyncEventAsync(MenuSyncEvent message, CancellationToken stoppingToken)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var mongo = scope.ServiceProvider.GetRequiredService<MongoDbContext>();

        var filter = Builders<MenuReadModel>.Filter.Eq(m => m.Id, message.MenuId);

        if (message.EventType.Equals("DELETE"))
        {
            var update = Builders<MenuReadModel>.Update
                .Set(m => m.IsActive, false)
                .Set(m => m.UpdatedAt, DateTime.Now);

            await mongo.MenuReadModel.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = false}, stoppingToken);

            logger.LogInformation($"MenuReadModel soft-deleted: {message.MenuId}");
            return;
        }

        var replacement = new MenuReadModel
        {
            Id = message.MenuId,
            Name = message.Name,
            Slug = message.Slug,
            DisplayOrder = message.DisplayOrder,
            IsActive = message.IsActive,
            CreatedAt = message.CreatedAt,
            UpdatedAt = message.UpdatedAt,
            News = message.News.Select(n => new NewsEmbedded
            {
                NewsId = n.NewsId,
                Title = n.Title,
                Slug = n.Slug,
                Summary = n.Summary,
                Thumbnail = n.Thumbnail,
                Status = n.Status.ToString(),
                PublishedAt = n.PublishedAt,
                ViewCount = n.ViewCount,
                CreatedAt = n.CreatedAt,
                DisplayOrder = n.DisplayOrder
            }).ToList()
        };

        await mongo.MenuReadModel.ReplaceOneAsync(filter, replacement, new ReplaceOptions { IsUpsert = true }, stoppingToken);
        logger.LogInformation($"MenuReadModel upserted: {message.MenuId} with {message.News.Count} news item(s)");
    }
    



}
