using AutoMapper;
using MediatR;
using MenuNews.SyncService.Application.Common.Exceptions;
using MenuNews.SyncService.Application.Common.Interfaces;
using MenuNews.SyncService.Application.DTOs;

namespace MenuNews.SyncService.Application.Features.News.Queries.GetNewsById;

public class GetNewsByIdQueryHandler : IRequestHandler<GetNewsByIdQuery, NewsDetailDto>
{
    private readonly INewsReadRepository newsReadRepository;
    private readonly IMapper mapper;

    public GetNewsByIdQueryHandler(INewsReadRepository newsReadRepository, IMapper mapper)
    {
        this.newsReadRepository = newsReadRepository;
        this.mapper = mapper;
    }

    public async Task<NewsDetailDto> Handle(GetNewsByIdQuery request, CancellationToken cancellationToken)
    {
        var news = await newsReadRepository.GetAsync(n => n.Id == request.id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.News), request.id);
        
        return mapper.Map<NewsDetailDto>(news);
    }
}
