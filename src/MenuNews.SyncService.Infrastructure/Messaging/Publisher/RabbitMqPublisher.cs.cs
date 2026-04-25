using MenuNews.SyncService.Application.Common.Interfaces;
using MenuNews.SyncService.Infrastructure.Messaging.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace MenuNews.SyncService.Infrastructure.Messaging.Publisher;

public sealed class RabbitMqPublisher : IRabbitMqPublisher, IAsyncDisposable
{
    private readonly RabbitMqSettings settings;
    private readonly ILogger<RabbitMqPublisher> logger;
    private IConnection? connection;
    private IChannel? channel;

    public RabbitMqPublisher(IOptions<RabbitMqSettings> settings, ILogger<RabbitMqPublisher> logger)
    {
        this.settings = settings.Value;
        this.logger = logger;
    }

    public async Task PublishAsync<T>(T message, string routingKey, CancellationToken ct = default) where T : class
    {
        await EnsureConnectionAsync(ct);

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        var props = new BasicProperties
        {
            Persistent = true,
            ContentType = "application/json",
        };

        await channel!.BasicPublishAsync(
            exchange: settings.DirectExchange, 
            routingKey: routingKey, 
            mandatory: false, 
            basicProperties: props, 
            body: body, 
            cancellationToken: ct
        );

        logger.LogInformation($"Published to exchange='{settings.DirectExchange}' with routingKey='{routingKey}'");
       
    }
    public async ValueTask DisposeAsync()
    {
        if (channel is not null) await channel.CloseAsync();
        if (connection is not null) await connection.CloseAsync();
    }
    private async Task EnsureConnectionAsync(CancellationToken ct)
    {
        if (connection is { IsOpen: true }) return;
        var factory = new ConnectionFactory
        {
            HostName = settings.HostName,
            UserName = settings.UserName,
            Password = settings.Password
        };

        connection = await factory.CreateConnectionAsync();
        channel = await connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(
            exchange: settings.DirectExchange, 
            type: ExchangeType.Direct, 
            durable: true, 
            autoDelete: false, 
            cancellationToken: ct
        );

        logger.LogInformation($"Connected to exchange '{settings.DirectExchange}'");
    }
}
