namespace MenuNews.SyncService.Application.Common.Interfaces.Messaging;

public interface IRabbitMqPublisher
{
    Task PublishAsync<T>(T message, string routingKey, CancellationToken ct = default) where T : class;
}
