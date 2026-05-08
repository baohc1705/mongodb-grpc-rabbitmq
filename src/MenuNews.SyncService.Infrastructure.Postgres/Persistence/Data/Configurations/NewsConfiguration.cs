using MenuNews.SyncService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MenuNews.SyncService.Infrastructure.Postgres.Persistence.Data.Configurations;

public class NewsConfiguration : IEntityTypeConfiguration<News>
{
    public void Configure(EntityTypeBuilder<News> builder)
    {
        builder.ToTable("news");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
            .HasColumnName("new_id")
            .ValueGeneratedNever();

        builder.Property(b => b.Title)
            .HasColumnName("new_title");

        builder.Property(b => b.Slug)
            .HasColumnName("new_slug");

        builder.Property(b => b.Summary)
            .HasColumnName("new_summary");

        builder.Property(b => b.Content)
            .HasColumnName("new_content");

        builder.Property(b => b.Thumbnail)
            .HasColumnName("new_thumbnail");

        builder.Property(b => b.Status)
            .HasColumnName("new_status");

        builder.Property(b => b.PublishedAt)
            .HasColumnName("new_published_at");

        builder.Property(b => b.ViewCount)
            .HasColumnName("new_view_count");

        builder.Property(b => b.CreatedAt)
            .HasColumnName("new_created_at");

        builder.Property(b => b.UpdatedAt)
            .HasColumnName("new_updated_at");

        builder.Property(b => b.IsActive)
            .HasColumnName("new_is_active");

        builder.Property(b => b.DeletedAt)
            .HasColumnName("new_deleted_at");
    }
}
