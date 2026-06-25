using Microsoft.AspNetCore.Mvc;
using VueAppAdmin.Server.Features.ExampleItems.Responses;
using VueAppAdmin.Server.Shared;

namespace VueAppAdmin.Server.Features.ExampleItems;

[ApiController]
[Route("api/[controller]")]
public class ExampleItemsController(IExampleItemsService exampleItemsService) : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll()
        => Ok(ApiResponse<ItemResponse>.OkList(exampleItemsService.GetAll()));

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var item = exampleItemsService.GetById(id);
        return item is null
            ? NotFound(ApiResponse<object>.Fail("找不到指定項目"))
            : Ok(ApiResponse<ItemResponse>.Ok(item));
    }

}
