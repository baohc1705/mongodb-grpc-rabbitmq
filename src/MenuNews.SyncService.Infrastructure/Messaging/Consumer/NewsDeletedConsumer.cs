using MenuNews.SyncService.Application.Constants;
using MenuNews.SyncService.Domain.Events;
using MenuNews.SyncService.Infrastructure.Messaging.Settings;
using MenuNews.SyncService.Infrastructure.ReadDb;
using MenuNews.SyncService.Infrastructure.ReadDb.ReadModels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace MenuNews.SyncService.Infrastructure.Messaging.Consumer;

public sealed class NewsDeletedConsumer : DirectConsumerBase<NewsSyncEvent>
{
    private readonly IMongoCollection<NewsReadModel> collection;
    public NewsDeletedConsumer(
        IOptions<RabbitMqSettings> settings, 
        ILogger<NewsDeletedConsumer> logger, 
        MongoDbContext context) : base(settings, logger)
    {
        this.collection = context.NewsReadModel;
    }

    protected override string QueueName => "news.deleted.queue";

    protected override IEnumerable<string> BindingKeys => new[] {NewsRountingKey.Deleted};

    protected override string ConsumerName => nameof(NewsDeletedConsumer);

    protected override async Task HandleMessageAsync(NewsSyncEvent message, string rountingKey, CancellationToken cancellationToken)
    {
        var filter = Builders<NewsReadModel>.Filter.Eq(n => n.Id, message.Id);
        await collection.DeleteOneAsync(filter);
        logger.LogInformation($"[{nameof(NewsDeletedConsumer)}] deleted with new id = [{message.Id}]");
    }
}
