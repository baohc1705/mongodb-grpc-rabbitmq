using MediatR;
using MenuNews.SyncService.Application.DTOs;

namespace MenuNews.SyncService.Application.Features.News.Queries.GetAllNews;

public record GetAllNewsQuery() : IRequest<IReadOnlyList<NewsDetailDto>>;

