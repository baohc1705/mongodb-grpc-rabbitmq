using MediatR;
using MenuNews.SyncService.Application.Features.News.Commands.CreateNewsWithMenusOutbox;
using Microsoft.AspNetCore.Mvc;

namespace MenuNews.SyncService.RestfulApi.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class NewsController : ControllerBase
{
    private readonly ISender mediator;

    public NewsController(ISender mediator)
    {
        this.mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateNewsWithMenusOutboxCommand command)
    {
        var res = await mediator.Send(command);
        return Ok(res);
    }
}
