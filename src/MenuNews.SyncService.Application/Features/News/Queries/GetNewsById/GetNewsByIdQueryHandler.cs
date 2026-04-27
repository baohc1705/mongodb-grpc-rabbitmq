using MediatR;
using MenuNews.SyncService.Application.Common.Exceptions;
using MenuNews.SyncService.Application.Common.Interfaces;
using MenuNews.SyncService.Application.DTOs;

namespace MenuNews.SyncService.Application.Features.News.Queries.GetNewsById;

public class GetNewsByIdQueryHandler : IRequestHandler<GetNewsByIdQuery, NewsDetailDto>
{
    private readonly INewsReadRepository newsReadRepository;

    public GetNewsByIdQueryHandler(INewsReadRepository newsReadRepository)
    {
        this.newsReadRepository = newsReadRepository;
    }

    public async Task<NewsDetailDto> Handle(GetNewsByIdQuery request, CancellationToken cancellationToken)
    {
        return await newsReadRepository.GetAsync(request.id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.News), request.id);
    }
}
