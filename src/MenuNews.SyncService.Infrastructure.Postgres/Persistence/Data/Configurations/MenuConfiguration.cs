using MenuNews.SyncService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MenuNews.SyncService.Infrastructure.Postgres.Persistence.Data.Configurations;

public class MenuConfiguration : IEntityTypeConfiguration<Menu>
{
    public void Configure(EntityTypeBuilder<Menu> builder)
    {
        builder.ToTable("menus");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
            .HasColumnName("menu_id")
            .ValueGeneratedNever();

        builder.Property(b => b.Name)
            .HasColumnName("menu_name");

        builder.Property(b => b.Slug)
            .HasColumnName("menu_slug");

        builder.Property(b => b.DisplayOrder)
            .HasColumnName("menu_display_order");

        builder.Property(b => b.IsActive)
            .HasColumnName("menu_is_active");

        builder.Property(b => b.CreatedAt)
            .HasColumnName("menu_created_at");

        builder.Property(b => b.UpdatedAt)
            .HasColumnName("menu_updated_at");

        builder.Property(b => b.DeletedAt)
            .HasColumnName("menu_deleted_at");
    }
}
