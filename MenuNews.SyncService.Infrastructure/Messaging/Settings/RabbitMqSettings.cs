namespace MenuNews.SyncService.Infrastructure.Messaging.Settings;

public sealed class RabbitMqSettings
{
    public string HostName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
