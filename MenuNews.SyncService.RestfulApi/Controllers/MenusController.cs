using MediatR;
using MenuNews.SyncService.Application.Features.Menus.Commands.CreateMenu;
using MenuNews.SyncService.Application.Features.Menus.Queries.GetAllMenus;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> GetAllMenus(CancellationToken cancellationToken)
        {
            var query = new GetAllMenusQuery();
            var result = await mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMenu([FromBody] CreateMenuCommand request, CancellationToken cancellationToken)
        {
            var command = new CreateMenuCommand
            {
                Name = request.Name,
                Slug = request.Slug,
                DisplayOrder = request.DisplayOrder,
                NewsItems = request.NewsItems.Select(ni => new CreateNewsItemRequest
                {
                    Title = ni.Title,
                    Slug = ni.Slug,
                    Summary = ni.Summary,
                    Content = ni.Content,
                    Thumbnail = ni.Thumbnail,
                    PublishedAt = ni.PublishedAt,
                    DisplayOrder = ni.DisplayOrder
                }).ToList()
            };
            var menuId = await mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetAllMenus), new { id = menuId }, null);
        }
    }
}
