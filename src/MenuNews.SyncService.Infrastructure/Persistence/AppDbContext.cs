using MenuNews.SyncService.Domain.Entities;
using MenuNews.SyncService.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace MenuNews.SyncService.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<Menu> Menus { get; set; }
    public DbSet<News> News { get; set; }
    public DbSet<NewsMenu> NewsMenus { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
