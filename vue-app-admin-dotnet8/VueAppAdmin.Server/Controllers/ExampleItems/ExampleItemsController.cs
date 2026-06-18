using Microsoft.AspNetCore.Mvc;
using VueAppAdmin.Server.Services.ExampleItems;

namespace VueAppAdmin.Server.Controllers.ExampleItems;

[ApiController]
[Route("api/[controller]")]
public class ExampleItemsController(ExampleItemsService exampleItemsService) : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll() => Ok(exampleItemsService.GetAll());

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var item = exampleItemsService.GetById(id);
        return item is null ? NotFound() : Ok(item);
    }
}
