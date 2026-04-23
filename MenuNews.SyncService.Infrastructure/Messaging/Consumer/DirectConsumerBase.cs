using MenuNews.SyncService.Infrastructure.Messaging.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace MenuNews.SyncService.Infrastructure.Messaging.Consumer;

public abstract class DirectConsumerBase<T> : BackgroundService
{
    private readonly RabbitMqSettings settings;
    protected readonly ILogger logger;
    private IConnection? connection;
    private IChannel? channel;

    protected DirectConsumerBase(IOptions<RabbitMqSettings> settings, ILogger logger)
    {
        this.settings = settings.Value;
        this.logger = logger;
    }
    protected abstract string QueueName { get; }
    protected abstract IEnumerable<string> BindingKeys { get; }
    protected abstract string ConsumerName { get; }

    protected abstract Task HandleMessageAsync(T message, string rountingKey, CancellationToken cancellationToken);

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
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
            cancellationToken: cancellationToken
        );

        await channel.QueueDeclareAsync(
            queue: QueueName, 
            durable: true, 
            exclusive: false, 
            autoDelete: false, 
            cancellationToken: cancellationToken
        );

        foreach(var key in BindingKeys)
        {
            await channel.QueueBindAsync(
                queue: QueueName, 
                exchange: settings.DirectExchange, 
                routingKey: key, 
                cancellationToken: cancellationToken
            );

            logger.LogInformation($"Bound queue='{QueueName}' to exchange='{settings.DirectExchange}' with rountingKey='{key}'");
        }

        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new AsyncEventingBasicConsumer(channel!);

        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);
            var incomingRountingKey = ea.RoutingKey; // Rounting key thuc te cua message

            try
            {
                var message = JsonSerializer.Deserialize<T>(json);
                if (message is null)
                {
                    logger.LogWarning($"{ConsumerName}: Null message received");
                    await channel!.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
                    return;
                }

                logger.LogInformation($"{ConsumerName}: Received message with rountingKey='{incomingRountingKey}'");

                await HandleMessageAsync(message, incomingRountingKey, stoppingToken);

                await channel!.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
            }
            catch(Exception ex)
            {
                logger.LogError($"{ConsumerName} Error handling message with rountingKey='{incomingRountingKey}'");
                await channel!.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false, cancellationToken: stoppingToken);
            }
        };

        await channel!.BasicConsumeAsync(
            queue: QueueName, 
            autoAck: false, 
            consumer: consumer, 
            cancellationToken: stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation($"[{ConsumerName}] Stopping...");
        if (channel is not null) await channel.CloseAsync();
        if (connection is not null) await connection.CloseAsync();
        await base.StopAsync(cancellationToken);
    }
}
