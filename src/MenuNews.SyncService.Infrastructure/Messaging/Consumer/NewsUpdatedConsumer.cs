using MenuNews.SyncService.Application.Constants;
using MenuNews.SyncService.Domain.Events;
using MenuNews.SyncService.Infrastructure.Messaging.Settings;
using MenuNews.SyncService.Infrastructure.ReadDb;
using MenuNews.SyncService.Infrastructure.ReadDb.ReadModels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace MenuNews.SyncService.Infrastructure.Messaging.Consumer;

public class NewsUpdatedConsumer : DirectConsumerBase<NewsSyncEvent>
{
    private readonly IMongoCollection<NewsReadModel> collection;
    public NewsUpdatedConsumer(
        IOptions<RabbitMqSettings> settings, 
        ILogger<NewsUpdatedConsumer> logger,
        MongoDbContext context
        ) : base(settings, logger)
    {
        collection = context.NewsReadModel;
    }

    protected override string QueueName => "news.updated.queue";

    protected override IEnumerable<string> BindingKeys => new[] { NewsRountingKey.Updated };

    protected override string ConsumerName => nameof(NewsUpdatedConsumer);

    protected override async Task HandleMessageAsync(NewsSyncEvent message, string rountingKey, CancellationToken cancellationToken)
    {
        var filter = Builders<NewsReadModel>.Filter.Eq(n => n.Id, message.Id);

        var updateDefinition = Builders<NewsReadModel>.Update
            .Set(n => n.Title, message.Title)
            .Set(n => n.Slug, message.Slug)
            .Set(n => n.Content, message.Content)
            .Set(n => n.Thumbnail, message.Thumbnail)
            .Set(n => n.PublishedAt, message.PublishedAt)
            .Set(n => n.UpdatedAt, message.UpdatedAt);

        await collection.UpdateOneAsync(
            filter: filter, 
            update: updateDefinition, 
            options: new UpdateOptions { IsUpsert = false }, 
            cancellationToken: cancellationToken
        );

        logger.LogInformation($"{nameof(NewsUpdatedConsumer)} updated message = [{message.Id}]");
    }
}
