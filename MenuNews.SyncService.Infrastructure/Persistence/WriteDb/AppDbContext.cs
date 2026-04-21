using MenuNews.SyncService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MenuNews.SyncService.Infrastructure.Persistence.WriteDb;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Menu> Menus { get; set; }
    public DbSet<News> News { get; set; }
    public DbSet<NewsMenu> NewsMenus { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
