namespace MenuNews.SyncService.Application.Common.Interfaces;

public interface IRabbitMqPublisher
{
    Task PublishAsync<T>(string exchange, string routingKey, T message, CancellationToken ct = default) where T : class;
}
