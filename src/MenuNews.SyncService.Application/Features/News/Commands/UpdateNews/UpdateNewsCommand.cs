using MediatR;
using MenuNews.SyncService.Application.DTOs;

namespace MenuNews.SyncService.Application.Features.News.Commands.UpdateNews;

public record UpdateNewsCommand(
    Guid Id,
    string Title,
    string Slug,
    string Summary,
    string Content,
    string Thumbnail,
    DateTime PublishedAt
) : IRequest<NewsDto>;