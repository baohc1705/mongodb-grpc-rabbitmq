using MenuNews.SyncService.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace MenuNews.SyncService.Infrastructure.Messaging.Publisher;

public sealed class RabbitMqPublisher : IRabbitMqPublisher
{
    private readonly RabbitMqConnectionManager connectionManager;
    private readonly ILogger<RabbitMqPublisher> logger;
    private IConnection? connection;
    private IChannel? channel;

    public RabbitMqPublisher(RabbitMqConnectionManager connectionManager, ILogger<RabbitMqPublisher> logger)
    {
        this.connectionManager = connectionManager;
        this.logger = logger;
    }

    public async Task PublishAsync<T>(string exchange, string routingKey, T message, CancellationToken ct = default) where T : class
    {
        connection = await connectionManager.GetConnectionAsync(ct);

        channel = await connection.CreateChannelAsync(cancellationToken: ct);

        await channel.ExchangeDeclareAsync(
            exchange: exchange,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false,
            cancellationToken: ct
        );

       
        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);
        var messageId = Guid.NewGuid().ToString();

        var props = new BasicProperties
        {
            Persistent = true,
            ContentType = "application/json",
            ContentEncoding = "utf-8",
            MessageId = messageId,
            Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
        };

        await channel.BasicPublishAsync(
            exchange: exchange,
            routingKey: routingKey,
            mandatory: true,
            basicProperties: props,
            body: body,
            cancellationToken: ct
        );

        logger.LogDebug($"Published -> exchange={exchange}, routingKey={routingKey}, messageId={messageId}");
    }
}
