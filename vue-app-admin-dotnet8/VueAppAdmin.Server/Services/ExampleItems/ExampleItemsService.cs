using VueAppAdmin.Server.DTO.Response.ExampleItems;

namespace VueAppAdmin.Server.Services.ExampleItems;

public class ExampleItemsService
{
    private static readonly List<ExampleItemResponse> _items =
    [
        new() { Id = 1, Name = "Example Item 1", Description = "This is the first example item." },
        new() { Id = 2, Name = "Example Item 2", Description = "This is the second example item." },
        new() { Id = 3, Name = "Example Item 3", Description = "This is the third example item." }
    ];

    public IEnumerable<ExampleItemResponse> GetAll() => _items;

    public ExampleItemResponse? GetById(int id) => _items.FirstOrDefault(x => x.Id == id);
}
