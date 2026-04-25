using MenuNews.SyncService.Domain.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MenuNews.SyncService.Infrastructure.Persistence.Configurations;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessage");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("outbox_id")
            .ValueGeneratedNever();

        builder.Property(x => x.EventType)
            .HasColumnName("outbox_event_type");

        builder.Property(x => x.Payload)
            .HasColumnName("outbox_payload");

        builder.Property(x => x.CreatedAt)
            .HasColumnName("outbox_created_at");

        builder.Property(x => x.ProcessedAt)
            .HasColumnName("outbox_processed_at");

        builder.Property(x => x.Status)
            .HasColumnName("outbox_status");
    }
}
