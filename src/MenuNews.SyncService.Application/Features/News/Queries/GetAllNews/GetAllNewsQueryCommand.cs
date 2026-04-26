using MediatR;
using MenuNews.SyncService.Application.Common.Interfaces;
using MenuNews.SyncService.Application.DTOs;

namespace MenuNews.SyncService.Application.Features.News.Queries.GetAllNews;

public class GetAllNewsQueryCommand : IRequestHandler<GetAllNewsQuery, IReadOnlyList<NewsDetailDto>>
{
    private readonly INewsReadRepository newsReadRepository;


    public GetAllNewsQueryCommand(INewsReadRepository newsReadRepository)
    {
        this.newsReadRepository = newsReadRepository;
    }

    public async Task<IReadOnlyList<NewsDetailDto>> Handle(GetAllNewsQuery request, CancellationToken cancellationToken)
    {
        return await newsReadRepository.GetAllAsync(cancellationToken);
    }
}
