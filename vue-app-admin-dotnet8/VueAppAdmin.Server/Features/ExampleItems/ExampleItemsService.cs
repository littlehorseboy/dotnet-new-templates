using VueAppAdmin.Server.Features.ExampleItems.Responses;

namespace VueAppAdmin.Server.Features.ExampleItems;

public class ExampleItemsService : IExampleItemsService
{
    private static readonly List<ItemResponse> _items =
    [
        new() { Id = 1, Name = "Example Item 1", Description = "This is the first example item." },
        new() { Id = 2, Name = "Example Item 2", Description = "This is the second example item." },
        new() { Id = 3, Name = "Example Item 3", Description = "This is the third example item." }
    ];

    public IEnumerable<ItemResponse> GetAll() => _items;

    public ItemResponse? GetById(int id) => _items.FirstOrDefault(x => x.Id == id);
}
