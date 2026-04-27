using MediatR;
using MenuNews.SyncService.Application.Features.News.Commands.CreateNewsWithMenusOutbox;
using MenuNews.SyncService.Application.Features.News.Commands.DeleteNews;
using MenuNews.SyncService.Application.Features.News.Commands.UpdateNews;
using MenuNews.SyncService.Application.Features.News.Queries.GetAllNews;
using MenuNews.SyncService.Application.Features.News.Queries.GetNewsById;
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

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var res = await mediator.Send(new GetAllNewsQuery());
        return Ok(res);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        try
        {
            var res = await mediator.Send(new GetNewsByIdQuery(id));
            return Ok(res);
        }
        catch
        {
            return NotFound();
        }
        
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateNewsWithMenusOutboxCommand command)
    {
        var res = await mediator.Send(command);
        return Ok(res);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromBody] UpdateNewsCommand command, Guid id)
    {
        if (id != command.Id) return BadRequest("Id not same");
        var res = await mediator.Send(command);
        return Ok(res);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await mediator.Send(new DeleteNewsCommand(id));
        return NoContent();
    }
} 
