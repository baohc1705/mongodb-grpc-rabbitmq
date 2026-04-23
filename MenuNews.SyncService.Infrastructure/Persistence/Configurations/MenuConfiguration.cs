using MenuNews.SyncService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MenuNews.SyncService.Infrastructure.Persistence.Configurations;

public sealed class MenuConfiguration : IEntityTypeConfiguration<Menu>
{
    public void Configure(EntityTypeBuilder<Menu> builder)
    {
        builder.ToTable("Menus");

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id)
            .HasColumnName("menu_id")
            .ValueGeneratedNever();

        builder.Property(m => m.Name)
            .HasColumnName("menu_name")
            .HasMaxLength(200);
           

        builder.Property(m => m.Slug)
            .HasColumnName("menu_slug")
            .HasMaxLength(200);
           

        builder.HasIndex(m => m.Slug).IsUnique();

        builder.Property(m => m.DisplayOrder)
            .HasColumnName("menu_display_order");

        builder.Property(m => m.IsActive)
            .HasColumnName("menu_is_active");

        builder.Property(m => m.CreatedAt)
            .HasColumnName("menu_created_at");
            

        builder.Property(m => m.UpdatedAt)
            .HasColumnName("menu_updated_at");

        builder.Property(m => m.DeletedAt)
            .HasColumnName("menu_deleted_at");

        // Configure khoa ngoại với NewsMenu
        builder.HasMany(m => m.NewsMenus)
            .WithOne(nm => nm.Menu)
            .HasForeignKey(nm => nm.MenuId)
            .IsRequired(false);
    }
}
