using Microsoft.AspNetCore.Mvc;
using VueAppAdmin.Server.Features.ExampleItems.Responses;
using VueAppAdmin.Server.Shared;

namespace VueAppAdmin.Server.Features.ExampleItems;

[ApiController]
[Route("api/[controller]")]
public class ExampleItemsController(IExampleItemsService exampleItemsService) : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll(
        [FromQuery] int skip = 0,
        [FromQuery] int top = 10,
        [FromQuery] string sortField = "id",
        [FromQuery] string sortOrder = "asc")
    {
        var (items, total) = exampleItemsService.GetPaged(skip, top, sortField, sortOrder);
        return Ok(ApiPagedResponse<ItemResponse>.OkPaged(items, total));
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var item = exampleItemsService.GetById(id);
        return item is null
            ? NotFound(ApiResponse<object>.Fail("找不到指定項目"))
            : Ok(ApiResponse<ItemResponse>.Ok(item));
    }
}
