using MediatR;

namespace MenuNews.SyncService.Application.Features.Menus.Commands.DeleteMenu;

public record DeleteMenuCommand (Guid Id) : IRequest;
