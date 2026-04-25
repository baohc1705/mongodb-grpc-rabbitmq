namespace MenuNews.SyncService.Application.Constants;

public class RabbitMqConstants
{
    public const string MenuExchange = "menu.exchange";
    public const string NewsExchange = "news.exchange";

    public const string MenuSyncQueue = "menu.sync.queue";
    public const string NewsSyncQueue = "news.sync.queue";

    public const string MenuSyncRoutingKey = "menu.created";
    public const string NewsSyncRoutingKey = "news.sync";
}
