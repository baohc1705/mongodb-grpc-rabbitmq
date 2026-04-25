using MenuNews.SyncService.Application.Constants;
using MenuNews.SyncService.Domain.Events;
using MenuNews.SyncService.Infrastructure.Messaging.Settings;
using MenuNews.SyncService.Infrastructure.ReadDb;
using MenuNews.SyncService.Infrastructure.ReadDb.ReadModels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace MenuNews.SyncService.Infrastructure.Messaging.Consumer;

public sealed class MenuUpdatedConsumer : DirectConsumerBase<MenuSyncEvent>
{
    private readonly IMongoCollection<MenuReadModel> collection;
    public MenuUpdatedConsumer(IOptions<RabbitMqSettings> settings, ILogger<MenuUpdatedConsumer> logger, MongoDbContext context) : base(settings, logger)
    {
        this.collection = context.MenuReadModel;
    }

    protected override string QueueName => "menu.updated.queue";

    protected override IEnumerable<string> BindingKeys => new[] { MenuRoutingKey.Updated };

    protected override string ConsumerName => nameof(MenuUpdatedConsumer);

    protected override async Task HandleMessageAsync(MenuSyncEvent message, string rountingKey, CancellationToken cancellationToken)
    {
        var filter = Builders<MenuReadModel>.Filter.Eq(m => m.Id, message.MenuId);

        var updateDefinition = Builders<MenuReadModel>.Update
            .Set(m => m.Name, message.Name)
            .Set(m => m.Slug, message.Slug)
            .Set(m => m.DisplayOrder, message.DisplayOrder)
            .Set(m => m.UpdatedAt, message.UpdatedAt);

        await collection.UpdateOneAsync(
            filter: filter,
            update: updateDefinition,
            options: new UpdateOptions { IsUpsert = true },
            cancellationToken
        );

        logger.LogInformation($"{nameof(MenuUpdatedConsumer)} updated message = [{message.MenuId}]");
    }
}
