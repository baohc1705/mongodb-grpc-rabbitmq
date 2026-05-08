using MenuNews.SyncService.Domain.Entities;
using MenuNews.SyncService.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace MenuNews.SyncService.Infrastructure.Postgres.Persistence.Data;

public class PostgresDbContext : DbContext
{
    public PostgresDbContext(DbContextOptions<PostgresDbContext> options) : base(options)
    {
    }
       
    public DbSet<Menu> Menus { get; set; }
    public DbSet<News> News { get; set; }
    public DbSet<NewsMenu> NewsMenus { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PostgresDbContext).Assembly);
    }

}
