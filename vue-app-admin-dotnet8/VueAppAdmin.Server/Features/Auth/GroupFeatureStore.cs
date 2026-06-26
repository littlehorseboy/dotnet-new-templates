namespace VueAppAdmin.Server.Features.Auth;

// ============================================================
// !! DEMO 用 Hardcode 轉換表 !!
//
// 此類別以記憶體 Dictionary 模擬「使用者 → 群組 → 功能權限」的對應關係，
// 僅供範本展示用途。
//
// 實際專案 TODO：
//   1. 將使用者、群組、功能權限資料移至資料庫
//   2. 透過 IUserRepository / 群組 Repository 進行查詢
//   3. 刪除此靜態類別
// ============================================================
public static class GroupFeatureStore
{
    // TODO: 替換為資料庫查詢 — 使用者所屬群組
    private static readonly Dictionary<string, string[]> _userGroups = new()
    {
        ["admin"] = ["SuperAdmins"],
        ["viewer"] = ["ReadOnly"]
    };

    // TODO: 替換為資料庫查詢 — 群組擁有的功能識別字
    // 功能識別字格式：<資源>:<動作>，例如 "items:read"、"categories:manage"
    private static readonly Dictionary<string, string[]> _groupFeatures = new()
    {
        ["SuperAdmins"] = ["items:read", "items:write", "categories:manage", "menu:admin"],
        ["ReadOnly"] = ["items:read"]
    };

    public static string[] GetGroups(string username)
        => _userGroups.TryGetValue(username, out var groups) ? groups : [];

    // 取得使用者所有群組的功能聯集（去重複）
    public static string[] GetFeatures(string username)
    {
        var groups = GetGroups(username);
        return [.. groups
            .SelectMany(g => _groupFeatures.TryGetValue(g, out var f) ? f : [])
            .Distinct()];
    }
}
