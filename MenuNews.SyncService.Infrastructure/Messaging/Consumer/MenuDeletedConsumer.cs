using MenuNews.SyncService.Application.Constants;
using MenuNews.SyncService.Domain.Events;
using MenuNews.SyncService.Infrastructure.Messaging.Settings;
using MenuNews.SyncService.Infrastructure.ReadDb;
using MenuNews.SyncService.Infrastructure.ReadDb.ReadModels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace MenuNews.SyncService.Infrastructure.Messaging.Consumer;

public sealed class MenuDeletedConsumer : DirectConsumerBase<MenuSyncEvent>
{
    private readonly IMongoCollection<MenuReadModel> collection;
    public MenuDeletedConsumer(
        IOptions<RabbitMqSettings> settings,
        ILogger<MenuDeletedConsumer> logger,
        MongoDbContext context) : base(settings, logger)
    {
        this.collection = context.MenuReadModel;
    }

    protected override string QueueName => "menu.deleted.queue";

    protected override IEnumerable<string> BindingKeys => new[] { MenuRoutingKey.Deleted };

    protected override string ConsumerName => nameof(MenuDeletedConsumer);

    protected override async Task HandleMessageAsync(MenuSyncEvent message, string rountingKey, CancellationToken cancellationToken)
    {
        await collection.DeleteOneAsync(m => m.Id.Equals(message.MenuId), cancellationToken);
        logger.LogInformation($"{nameof(MenuDeletedConsumer)} deleted menuId = [{message.MenuId}]");
    }
}
