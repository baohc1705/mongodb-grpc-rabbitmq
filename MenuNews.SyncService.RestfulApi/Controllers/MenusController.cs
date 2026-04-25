using MediatR;
using MenuNews.SyncService.Application.Features.Menus.Commands.CreateMenuWithNews;
using MenuNews.SyncService.Application.Features.Menus.Commands.UpdateMenu;
using MenuNews.SyncService.Application.Features.Menus.Queries.GetMenus;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MenuNews.SyncService.RestfulApi.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class MenusController : ControllerBase
    {
        private readonly ISender mediator;

        public MenusController(ISender mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var sw = Stopwatch.StartNew();
            var result = await mediator.Send(new GetMenusQuery());
            sw.Stop();
            Console.WriteLine($"Controller: {sw.ElapsedMilliseconds}");
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMenu([FromBody] CreateMenuWithNewsCommand command, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateMenu(Guid id, [FromBody] UpdateMenuCommand command, CancellationToken cancellationToken)
        {
            if (id != command.Id)
                return BadRequest("Id not same");
            var res = await mediator.Send(command, cancellationToken);
            return Ok(res);
        }
    }
}
