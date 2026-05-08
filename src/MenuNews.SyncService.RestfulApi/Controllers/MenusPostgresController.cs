using MediatR;
using MenuNews.SyncService.Application.Features.Menus.Commands.CreateMenuWithNewsPostgres;
using MenuNews.SyncService.Application.Features.Menus.Queries.GetMenusPostgres;
using Microsoft.AspNetCore.Mvc;

namespace MenuNews.SyncService.RestfulApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenusPostgresController : ControllerBase
{
    private readonly ISender mediator;

    public MenusPostgresController(ISender mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllMenus()
    {
        try
        {
            var res = await mediator.Send(new GetMenusPostgresQuery());
            if (!res.Any()) 
                return NoContent();
            return Ok(res);
        }
        catch(Exception ex)
        {
            return StatusCode(500, new 
            { 
                message = "Internal Server Error", 
                detail = ex.Message
            });
        }
    }

    [HttpPost("postgres")]
    public async Task<IActionResult> CreatePostgres([FromBody] CreateMenuWithNewsPostgresCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var res = await mediator.Send(command, cancellationToken);
            return Ok(res);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Internal Server Error",
                detail = ex.Message
            });
        }
    }
} 
