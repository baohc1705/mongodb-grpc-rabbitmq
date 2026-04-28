using MenuNews.SyncService.Application.Constants;
using MenuNews.SyncService.Domain.Events;
using MenuNews.SyncService.Infrastructure.Messaging.Settings;
using MenuNews.SyncService.Infrastructure.ReadDb;
using MenuNews.SyncService.Application.Common.Models.ReadModels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace MenuNews.SyncService.Infrastructure.Messaging.Consumer;

public sealed class MenuInsertedConsumer : DirectConsumerBase<MenuSyncEvent>
{
    private readonly MongoDbContext context;
    
    public MenuInsertedConsumer(IOptions<RabbitMqSettings> settings, ILogger<MenuInsertedConsumer> logger, MongoDbContext context) : base(settings, logger)
    {
        this.context = context;
    }

    protected override string QueueName => RabbitMqConstants.MenuSyncQueue;

    protected override IEnumerable<string> BindingKeys => new[] { MenuRoutingKey.Inserted };

    protected override string ConsumerName => nameof(MenuInsertedConsumer);

    protected override async Task HandleMessageAsync(MenuSyncEvent message, string rountingKey, CancellationToken cancellationToken)
    {
        var filter = Builders<MenuReadModel>.Filter.Eq(m => m.Id, message.MenuId);
        var doc = new MenuReadModel
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

        await context.MenuReadModel.ReplaceOneAsync(
            filter: filter, 
            replacement: doc, 
            new ReplaceOptions { IsUpsert = true }, 
            cancellationToken
        );

        logger.LogInformation($"MenuReadModel upserted: {message.MenuId} with {message.News.Count} news item(s)");
    }
}
