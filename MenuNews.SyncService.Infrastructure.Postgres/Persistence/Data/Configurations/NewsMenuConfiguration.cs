using MenuNews.SyncService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MenuNews.SyncService.Infrastructure.Postgres.Persistence.Data.Configurations;

public class NewsMenuConfiguration : IEntityTypeConfiguration<NewsMenu>
{
    public void Configure(EntityTypeBuilder<NewsMenu> builder)
    {
        builder.ToTable("news_menu");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
            .HasColumnName("nm_id")
            .ValueGeneratedNever();

        builder.Property(b => b.NewsId)
            .HasColumnName("nm_news_id");

        builder.Property(b => b.MenuId)
            .HasColumnName("nm_menu_id");

        builder.Property(b => b.CreatedAt)
            .HasColumnName("nm_created_at");

        builder.Property(b => b.UpdatedAt)
            .HasColumnName("nm_updated_at");

        builder.Property(b => b.DeletedAt)
            .HasColumnName("nm_deleted_at");

        builder.Property(b => b.IsActive)
            .HasColumnName("nm_is_active");

        builder.Property(b => b.DisplayOrder)
            .HasColumnName("nm_display_order");

        // config khoa ngoai cho bang menu new
        builder.HasOne(b => b.Menu)
            .WithMany(b => b.NewsMenus)
            .HasForeignKey(b => b.MenuId)
            .IsRequired(false);

        builder.HasOne(b => b.News)
            .WithMany(b => b.NewsMenus)
            .HasForeignKey(b => b.NewsId)
            .IsRequired(false);
    }
}
