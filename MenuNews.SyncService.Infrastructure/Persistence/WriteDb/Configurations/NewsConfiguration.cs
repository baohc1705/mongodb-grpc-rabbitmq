using MenuNews.SyncService.Domain.Entities;
using MenuNews.SyncService.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MenuNews.SyncService.Infrastructure.Persistence.WriteDb.Configurations;

public class NewsConfiguration : IEntityTypeConfiguration<News>
{
    public void Configure(EntityTypeBuilder<News> builder)
    {
        builder.ToTable("News");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.Id)
            .HasColumnName("new_id")
            .ValueGeneratedNever();


        builder.Property(n => n.Title)
            .HasColumnName("new_title");


        builder.Property(n => n.Slug)
            .HasColumnName("new_slug");


        builder.Property(n => n.Summary)
            .HasColumnName("new_summary");


        builder.Property(n => n.Content)
            .HasColumnName("new_content");


        builder.Property(n => n.Thumbnail)
            .HasColumnName("new_thumbnail");

        builder.Property(n => n.Status)
            .HasConversion(
                v => v.ToString(),
                v => (NewsStatus)Enum.Parse(typeof(NewsStatus), v)
            )
            .HasColumnName("new_status");
        builder.Property(n => n.PublishedAt)
            .HasColumnName("new_published_at");

        builder.Property(n => n.ViewCount)
            .HasColumnName("new_view_count");

        builder.Property(n => n.Active)
            .HasColumnName("new_is_active");

        builder.Property(n => n.CreatedAt)
            .HasColumnName("new_created_at");

        builder.Property(n => n.UpdatedAt)
            .HasColumnName("new_updated_at");

        builder.Property(n => n.DeletedAt)
            .HasColumnName("new_deleted_at");

    }
}
