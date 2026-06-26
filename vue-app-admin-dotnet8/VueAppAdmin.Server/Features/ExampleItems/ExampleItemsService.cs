using VueAppAdmin.Server.Features.ExampleItems.Requests;
using VueAppAdmin.Server.Features.ExampleItems.Responses;

namespace VueAppAdmin.Server.Features.ExampleItems;

// TODO: 此服務使用記憶體靜態資料（30 筆），實際專案應改為資料庫查詢
public class ExampleItemsService : IExampleItemsService
{
    // demo 用類別資料，與 ExampleCategoriesService 的資料對應
    private static readonly (int Id, string Name)[] _categories =
    [
        (1, "A 類"), (2, "B 類"), (3, "C 類")
    ];

    // 產生 30 筆 demo 資料，類別依 id % 3 循環分配
    private static readonly List<ItemResponse> _items = Enumerable.Range(1, 30).Select(i =>
    {
        var catId = (i % 3) + 1;
        var catName = _categories.First(c => c.Id == catId).Name;
        return new ItemResponse
        {
            Id = i,
            Name = $"Item {i:D2}",
            Description = $"Description for item number {i}.",
            CategoryId = catId,
            CategoryName = catName
        };
    }).ToList();

    public ItemResponse? GetById(int id) => _items.FirstOrDefault(x => x.Id == id);

    // 在記憶體中執行篩選、排序後再分頁
    // 實際專案應將篩選排序轉為 SQL（或 ORM 查詢），避免全表載入
    public (IEnumerable<ItemResponse> Items, int Total) Search(ExampleItemsSearchRequest request)
    {
        var query = _items.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.Name))
            query = query.Where(x => x.Name.Contains(request.Name, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(request.Description))
            query = query.Where(x => x.Description.Contains(request.Description, StringComparison.OrdinalIgnoreCase));

        if (request.CategoryIds.Count > 0)
            query = query.Where(x => request.CategoryIds.Contains(x.CategoryId));

        // 排序欄位預設為 id；不支援的欄位名稱一律 fallback 到 id 排序
        query = request.SortField.ToLowerInvariant() switch
        {
            "name" => request.SortOrder == "desc"
                ? query.OrderByDescending(x => x.Name)
                : query.OrderBy(x => x.Name),
            "description" => request.SortOrder == "desc"
                ? query.OrderByDescending(x => x.Description)
                : query.OrderBy(x => x.Description),
            _ => request.SortOrder == "desc"
                ? query.OrderByDescending(x => x.Id)
                : query.OrderBy(x => x.Id),
        };

        var filtered = query.ToList();
        var total = filtered.Count;
        var skip = (request.Page - 1) * request.PageSize;
        var items = filtered.Skip(skip).Take(request.PageSize);
        return (items, total);
    }
}
