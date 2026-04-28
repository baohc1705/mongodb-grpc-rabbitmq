using AutoMapper;
using MediatR;
using MenuNews.SyncService.Application.Common.Interfaces;
using MenuNews.SyncService.Application.DTOs;

namespace MenuNews.SyncService.Application.Features.News.Queries.GetAllNews;

public class GetAllNewsQueryCommand : IRequestHandler<GetAllNewsQuery, IReadOnlyList<NewsDetailDto>>
{
    private readonly INewsReadRepository newsReadRepository;
    private readonly IMapper mapper;


    public GetAllNewsQueryCommand(INewsReadRepository newsReadRepository, IMapper mapper)
    {
        this.newsReadRepository = newsReadRepository;
        this.mapper = mapper;
    }

    public async Task<IReadOnlyList<NewsDetailDto>> Handle(GetAllNewsQuery request, CancellationToken cancellationToken)
    {
        var news = await newsReadRepository.GetAllAsync(cancellationToken);
        return mapper.Map<IReadOnlyList<NewsDetailDto>>(news);
    }
}
