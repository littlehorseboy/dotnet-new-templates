namespace VueAppAdmin.Server.Features.Menu;

// TODO: 此服務使用靜態樹狀結構定義選單，實際專案應改為資料庫查詢
public class MenuService : IMenuService
{
    // 完整選單樹，包含所有節點（不論權限）
    // Route = null 的節點為群組節點（資料夾），本身不可點擊
    // RequiredFeature = null 的節點對所有登入使用者可見
    private static readonly List<MenuNode> _tree =
    [
        new() {
            Id = 1, Label = "儀表板", Icon = "bi-speedometer2", Route = "/dashboard", RequiredFeature = null,
            Children = []
        },
        new() {
            Id = 2, Label = "資料管理", Icon = "bi-folder2-open", Route = null, RequiredFeature = null,
            Children =
            [
                new() { Id = 3, Label = "範例清單", Icon = "bi-list-ul",      Route = "/example-items",      RequiredFeature = "items:read",        Children = [] },
                new() { Id = 4, Label = "類別管理", Icon = "bi-tags",          Route = "/example-categories", RequiredFeature = "categories:manage",  Children = [] },
            ]
        },
        new() {
            Id = 5, Label = "系統管理", Icon = "bi-gear", Route = null, RequiredFeature = null,
            Children =
            [
                new() { Id = 6, Label = "群組管理", Icon = "bi-people", Route = "/groups", RequiredFeature = "menu:admin", Children = [] },
            ]
        },
    ];

    public IEnumerable<MenuNode> GetFilteredMenu(IEnumerable<string> features)
    {
        var featureSet = new HashSet<string>(features);
        return FilterNodes(_tree, featureSet);
    }

    // 遞迴過濾選單節點：
    //   - 節點有 RequiredFeature 但使用者無此功能 → 整個節點連同子節點排除
    //   - 群組節點（Route = null）過濾後無任何子節點 → 排除（避免顯示空資料夾）
    private static List<MenuNode> FilterNodes(IEnumerable<MenuNode> nodes, HashSet<string> features)
    {
        var result = new List<MenuNode>();
        foreach (var node in nodes)
        {
            if (node.RequiredFeature != null && !features.Contains(node.RequiredFeature))
                continue;

            var filteredChildren = FilterNodes(node.Children, features);

            if (node.Route == null && filteredChildren.Count == 0)
                continue;

            result.Add(new MenuNode
            {
                Id = node.Id,
                Label = node.Label,
                Icon = node.Icon,
                Route = node.Route,
                RequiredFeature = node.RequiredFeature,
                Children = filteredChildren
            });
        }
        return result;
    }
}
