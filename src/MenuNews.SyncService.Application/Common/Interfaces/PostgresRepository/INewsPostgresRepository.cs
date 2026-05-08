using MenuNews.SyncService.Domain.Entities;

namespace MenuNews.SyncService.Application.Common.Interfaces.PostgresRepository;

public interface INewsPostgresRepository : IBaseRepository<News>
{
}
