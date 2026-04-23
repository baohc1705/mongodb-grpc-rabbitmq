using MenuNews.SyncService.Infrastructure.Messaging.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace MenuNews.SyncService.Infrastructure.Messaging;
/*
 Quan ly connection singleton den RabbitMQ, de tranh viec tao nhieu connection toi RabbitMQ, chi can tao 1 connection va share cho cac publisher va consumer
 */
public sealed class RabbitMqConnectionManager : IAsyncDisposable
{
    private readonly RabbitMqSettings settings;
    private readonly ILogger<RabbitMqConnectionManager> logger;
    private IConnection? connection;
    //private IChannel? channel;

    public RabbitMqConnectionManager(IOptions<RabbitMqSettings> settings, ILogger<RabbitMqConnectionManager> logger)
    {
        this.settings = settings.Value;
        this.logger = logger;
    }

    public async Task<IConnection> GetConnectionAsync(CancellationToken cancellation)
    {
        if (connection is { IsOpen: true })
        {
            return connection;
        }

        try
        {
            if (connection is { IsOpen: true })
            {
                return connection;
            }

            var factory = new ConnectionFactory
            {
                HostName = settings.HostName,
                UserName = settings.UserName,
                Password = settings.Password
            };

            connection = await factory.CreateConnectionAsync();

            return connection;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create RabbitMQ connection");
            throw;
        }
    }

    public async Task<IChannel> GetChannel(string exchange, string queue, string routingKey, CancellationToken cancellationToken)
    {
        var connection = await GetConnectionAsync(cancellationToken);
        var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.ExchangeDeclareAsync(
            exchange: exchange,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken
        );

        await channel.QueueDeclareAsync(
            queue: queue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken
        );

        await channel.QueueBindAsync(
            queue: queue,
            exchange: exchange,
            routingKey: routingKey,
            cancellationToken: cancellationToken
        );

        return channel;
    }

    public async ValueTask DisposeAsync()
    {

        if (connection is not null)
        {
            await connection.CloseAsync();
            connection.Dispose();
        }
    }
}
