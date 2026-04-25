namespace MenuNews.SyncService.Domain.Events;

public sealed class OutboxStatus
{
    public const string PENDING = "PENDING";
    public const string PROCESSED = "PROCESSED";
    public const string FAILED = "FAILED";
}


public sealed class OutboxMessage
{
    public Guid Id { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public string Status { get; set; } = OutboxStatus.PENDING;

    public static OutboxMessage Create(string eventType, string payload)
    {
        return new OutboxMessage
        {
            Id = Guid.NewGuid(),
            EventType = eventType,
            Payload = payload,
            CreatedAt = DateTime.UtcNow,
            ProcessedAt = null,
            Status = OutboxStatus.PENDING
        };
    }
}
