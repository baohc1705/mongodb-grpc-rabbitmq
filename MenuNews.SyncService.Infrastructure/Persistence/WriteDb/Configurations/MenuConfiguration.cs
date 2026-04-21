using MenuNews.SyncService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MenuNews.SyncService.Infrastructure.Persistence.WriteDb.Configurations;

public class MenuConfiguration : IEntityTypeConfiguration<Menu>
{
    public void Configure(EntityTypeBuilder<Menu> builder)
    {
        builder.ToTable("Menus");

        builder.Property(m => m.Id)
            .HasColumnName("menu_id")
            .ValueGeneratedNever();

        builder.Property(m => m.Name)
            .HasColumnName("menu_name");
        
        builder.Property(m => m.Slug) 
            .HasColumnName("menu_slug");
        builder.Property(m => m.DisplayOrder)
               .HasColumnName("menu_display_order");

        builder.Property(m => m.Active)
            .HasColumnName("menu_is_active");

        builder.Property(m => m.CreatedAt)
           .HasColumnName("menu_created_at");

        builder.Property(m => m.UpdatedAt)
           .HasColumnName("menu_updated_at");

        builder.Property(m => m.DeletedAt)
           .HasColumnName("menu_deleted_at");
    }
}
