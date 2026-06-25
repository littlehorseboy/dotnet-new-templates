using VueAppAdmin.Server.Features.ExampleItems.Responses;

namespace VueAppAdmin.Server.Features.ExampleItems;

public class ExampleItemsService : IExampleItemsService
{
    private static readonly List<ItemResponse> _items = Enumerable.Range(1, 30).Select(i => new ItemResponse
    {
        Id = i,
        Name = $"Item {i:D2}",
        Description = $"Description for item number {i}. Category: {(i % 3 == 0 ? "C" : i % 3 == 1 ? "A" : "B")}."
    }).ToList();

    public IEnumerable<ItemResponse> GetAll() => _items;

    public ItemResponse? GetById(int id) => _items.FirstOrDefault(x => x.Id == id);

    public (IEnumerable<ItemResponse> Items, int Total) GetPaged(int skip, int top, string sortField, string sortOrder)
    {
        var query = sortField.ToLowerInvariant() switch
        {
            "name" => sortOrder == "desc"
                ? _items.OrderByDescending(x => x.Name)
                : _items.OrderBy(x => x.Name),
            "description" => sortOrder == "desc"
                ? _items.OrderByDescending(x => x.Description)
                : _items.OrderBy(x => x.Description),
            _ => sortOrder == "desc"
                ? _items.OrderByDescending(x => x.Id)
                : _items.OrderBy(x => x.Id),
        };

        var total = _items.Count;
        var items = query.Skip(skip).Take(top);
        return (items, total);
    }
}
