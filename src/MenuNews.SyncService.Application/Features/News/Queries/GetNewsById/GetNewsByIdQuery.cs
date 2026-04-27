using MediatR;
using MenuNews.SyncService.Application.DTOs;

namespace MenuNews.SyncService.Application.Features.News.Queries.GetNewsById;

public record GetNewsByIdQuery(Guid id) : IRequest<NewsDetailDto>;
