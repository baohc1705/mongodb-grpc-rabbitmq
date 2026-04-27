using MediatR;

namespace MenuNews.SyncService.Application.Features.News.Commands.DeleteNews;

public record DeleteNewsCommand (Guid Id) : IRequest;
