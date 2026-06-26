using VueAppAdmin.Server.Features.ExampleItems;
using VueAppAdmin.Server.Features.ExampleItems.Requests;

namespace VueAppAdmin.Server.Tests.Features.ExampleItems;

// ExampleItemsService 直接 new()，不需 mock（記憶體資料，無外部相依）
public class ExampleItemsServiceTests
{
    private readonly ExampleItemsService _sut = new();

    [Fact]
    public void Search_NoFilters_ReturnsFirstPageOf10()
    {
        var request = new ExampleItemsSearchRequest { Page = 1, PageSize = 10 };
        var (items, total) = _sut.Search(request);

        // 共 30 筆 demo 資料，第一頁應回傳 10 筆
        Assert.Equal(30, total);
        Assert.Equal(10, items.Count());
    }

    [Fact]
    public void Search_ByName_ReturnsMatchingItems()
    {
        var request = new ExampleItemsSearchRequest { Page = 1, PageSize = 30, Name = "Item 1" };
        var (items, _) = _sut.Search(request);

        Assert.All(items, i => Assert.Contains("Item 1", i.Name, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Search_ByDescription_ReturnsMatchingItems()
    {
        var request = new ExampleItemsSearchRequest { Page = 1, PageSize = 30, Description = "number 5" };
        var (items, _) = _sut.Search(request);

        Assert.All(items, i => Assert.Contains("number 5", i.Description, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Search_ByCategoryIds_ReturnsOnlyMatchingCategories()
    {
        var request = new ExampleItemsSearchRequest { Page = 1, PageSize = 30, CategoryIds = [1] };
        var (items, _) = _sut.Search(request);

        Assert.All(items, i => Assert.Equal(1, i.CategoryId));
    }

    [Fact]
    public void Search_MultipleCategories_ReturnsAllMatchingItems()
    {
        var request = new ExampleItemsSearchRequest { Page = 1, PageSize = 30, CategoryIds = [1, 2] };
        var (items, _) = _sut.Search(request);

        Assert.All(items, i => Assert.True(i.CategoryId == 1 || i.CategoryId == 2));
    }

    [Fact]
    public void GetById_ExistingId_ReturnsItem()
    {
        var result = _sut.GetById(1);
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public void GetById_NonExistingId_ReturnsNull()
    {
        var result = _sut.GetById(999);
        Assert.Null(result);
    }
}
