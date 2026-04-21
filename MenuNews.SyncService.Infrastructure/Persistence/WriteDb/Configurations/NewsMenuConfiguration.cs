using MenuNews.SyncService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MenuNews.SyncService.Infrastructure.Persistence.WriteDb.Configurations;

public class NewsMenuConfiguration : IEntityTypeConfiguration<NewsMenu>
{
    public void Configure(EntityTypeBuilder<NewsMenu> builder)
    {
        builder.ToTable("NewsMenus");

        builder.HasKey(nm => nm.Id);
        builder.Property(nm => nm.Id)
            .HasColumnName("nm_id");

        builder.Property(nm => nm.MenuId)
            .HasColumnName("nm_menu_id");

        builder.Property(nm => nm.NewsId)
            .HasColumnName("nm_news_id");

        builder.Property(nm => nm.Active)
            .HasColumnName("nm_is_active");

        builder.Property(nm => nm.DisplayOrder)
            .HasColumnName("nm_display_order");

        builder.Property(nm => nm.CreatedAt)
           .HasColumnName("nm_created_at");

        builder.Property(nm => nm.UpdatedAt)
           .HasColumnName("nm_updated_at");

        builder.Property(nm => nm.DeletedAt)
           .HasColumnName("nm_deleted_at");

        // Cài đặt relationship với News và Menu
        builder.HasOne(nm => nm.News)
            .WithMany(n => n.NewsMenus)
            .HasForeignKey(nm => nm.NewsId)
            .IsRequired(false);

        builder.HasOne(nm => nm.Menu)
            .WithMany(m => m.NewsMenus)
            .HasForeignKey(nm => nm.MenuId)
            .IsRequired(false);
    }
}
