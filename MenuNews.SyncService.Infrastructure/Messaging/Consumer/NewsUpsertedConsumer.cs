using MenuNews.SyncService.Application.Constants;
using MenuNews.SyncService.Domain.Events;
using MenuNews.SyncService.Infrastructure.Messaging.Settings;
using MenuNews.SyncService.Infrastructure.ReadDb;
using MenuNews.SyncService.Infrastructure.ReadDb.ReadModels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace MenuNews.SyncService.Infrastructure.Messaging.Consumer;

public sealed class NewsUpsertedConsumer : DirectConsumerBase<NewsSyncEvent>
{
    private readonly MongoDbContext context;
    public NewsUpsertedConsumer(IOptions<RabbitMqSettings> settings, ILogger<NewsUpsertedConsumer> logger, MongoDbContext context) : base(settings, logger)
    {
        this.context = context;
    }

    protected override string QueueName => RabbitMqConstants.NewsSyncQueue;

    protected override IEnumerable<string> BindingKeys => new[] {NewsRountingKey.Upserted};

    protected override string ConsumerName => nameof(NewsUpsertedConsumer);

    protected override async Task HandleMessageAsync(NewsSyncEvent message, string rountingKey, CancellationToken cancellationToken)
    {
        var filter = Builders<NewsReadModel>.Filter.Eq(n => n.NewsId, message.NewsId);
        var doc = new NewsReadModel
        {
            NewsId = message.NewsId,
            Title = message.Title,
            Slug = message.Slug,
            Summary = message.Summary,
            Content = message.Content,
            Thumbnail = message.Thumbnail,
            Status = message.Status.ToString(),
            PublishedAt = message.PublishedAt,
            ViewCount = message.ViewCount,
            IsActive = message.IsActive,
            CreatedAt = message.CreatedAt,
            UpdatedAt = message.UpdatedAt,
            Menus = message.Menus.Select(m => new MenuEmbedded
            {
                MenuId = m.MenuId,
                Name = m.Name,
                Slug = m.Slug,
                DisplayOrder = m.DisplayOrder,
                NmDisplayOrder = m.NmDisplayOrder,
            }).ToList()
        };

        await context.NewsReadModel.ReplaceOneAsync(
            filter: filter, 
            replacement: doc, 
            options: new ReplaceOptions { IsUpsert = true }, 
            cancellationToken: cancellationToken
        );

        logger.LogInformation($"NewsReadModel upserted: {message.NewsId} with {message.Menus.Count} news item(s)");
    }
}
