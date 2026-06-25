using VueAppAdmin.Server.Features.ExampleItems.Responses;

namespace VueAppAdmin.Server.Features.ExampleItems;

public interface IExampleItemsService
{
    IEnumerable<ItemResponse> GetAll();
    ItemResponse? GetById(int id);
    (IEnumerable<ItemResponse> Items, int Total) GetPaged(int skip, int top, string sortField, string sortOrder);
}
