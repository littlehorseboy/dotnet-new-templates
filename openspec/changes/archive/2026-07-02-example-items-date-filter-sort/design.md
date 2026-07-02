## Context

`ExampleItemsService`（`Features/ExampleItems/ExampleItemsService.cs`）目前是純 in-memory 假資料（30 筆），依 [[data-access]] 規範在 template 階段 SHALL 維持 in-memory，不接真實資料庫。前端 `ExampleItemsView.vue` 以 PrimeVue `DataTable` 的 lazy 模式運作，排序、分頁皆透過 `POST /api/ExampleItems/Search` 交由後端處理，現有的篩選列（名稱、說明、類別）遵循「填條件不自動查詢，需按查詢按鈕或 Enter」的既有互動模式（見 [[example-items-search]]）。

這次要新增日期欄位、日期區間查詢、全欄位排序。由於現行資料是記憶體陣列，排序目前用 LINQ `switch` + `OrderBy`/`OrderByDescending` 手動對應欄位名稱；若日後接 SQL Server，同樣的「欄位名稱字串 → 排序邏輯」對應方式必須改用白名單機制，否則直接把使用者傳入的 `sortField` 字串拼進 SQL 的 `ORDER BY` 子句會有 SQL Injection 風險。這個設計決策值得現在就寫成範例，讓後續開發者在真正串接資料庫時有依循。

## Goals / Non-Goals

**Goals:**
- `ItemResponse` 與假資料具備有意義、彼此不同的 `createdDate`，讓日期排序與區間篩選可被驗證。
- 排序涵蓋所有現有與新增欄位，不再有「未知案例 fallback 到 id」以外的遺漏欄位。
- 日期區間篩選在前端以單一 PrimeVue `DatePicker`（`selectionMode="range"`）呈現，行為與既有篩選列一致（不自動觸發查詢）。
- 在 `ExampleItemsService.cs` 留下清楚的 TODO 註解，示範未來 SQL Server + Dapper 的 `OFFSET/FETCH` 分頁與動態排序寫法，作為範本教學內容。

**Non-Goals:**
- 不會真的接上 SQL Server 或建立 `Items`/`Categories` 資料表 — 依 [[data-access]] 規範，`ExampleItemsService` 在 template 階段維持 in-memory。
- 不會建立 Repository interface/實作（`IItemRepository` 等）— 這屬於未來真正串接資料庫時的工作，本次只在既有 Service 內留下註解說明。
- 不處理時區轉換的通用機制，`createdDate` 假資料與篩選皆以伺服器本地時間（`DateTime`，非 `DateTimeOffset`）為準，與專案現行慣例一致。

## Decisions

### 1. `createdDate` 假資料產生方式

沿用現有 `Enumerable.Range(1, 30)` 產生器，以 `DateTime.Today.AddDays(-i)`（`i` 為索引）讓 30 筆資料各自落在過去 30 天內的不同日期，確保排序與區間篩選有可觀察的差異。不使用 `DateTime.Now`（假資料應是固定可預期的，避免每次啟動服務資料飄移導致測試不穩定）。

### 2. 日期區間篩選的邊界語意

`dateFrom`、`dateTo` 皆為選填 `DateTime?`，語意為「`createdDate` 落在 `[dateFrom, dateTo]` 閉區間」（兩端點皆含）。若只帶 `dateFrom` 則代表「大於等於」，只帶 `dateTo` 則代表「小於等於」。前端 `DatePicker` 的 `selectionMode="range"` 天然對應此語意（使用者選一個起訖區間）。

### 3. 排序改為完整白名單，移除隱性 fallback 缺口

現行 `switch (request.SortField.ToLowerInvariant())` 只有 `name`、`description` 兩個具名分支，其餘（含 `id`、`categoryName`）都落入 `default` 導致實際上永遠以 `id` 排序，即使前端欄位開了 `sortable`，使用者點擊 `categoryName` 表頭排序也不會有效果。本次補齊 `id`、`categoryName`、`createdDate` 三個分支，讓「前端哪些欄位可排序」與「後端支援哪些排序欄位」保持一致，避免使用者體感上的排序失效。

### 4. 未來 SQL Server + Dapper 寫法以 TODO 註解呈現（不實作）

在 `ExampleItemsService.Search` 方法上方（或方法內部合適位置）新增一段 TODO 註解，內容示範：

- 動態排序 SHALL 用 C# 端的白名單字典把 `sortField` 映射成實際 SQL 欄位字串（例如 `categoryName → "c.Name"`），映射後的字串才可安全地以字串插值方式組進 SQL 文字；**絕不**能把使用者輸入的原始字串直接放進 `ORDER BY`，否則構成 SQL Injection 風險。
- 分頁 SHALL 使用 SQL Server `OFFSET @Skip ROWS FETCH NEXT @PageSize ROWS ONLY`。
- 分頁資料與 `Total` 筆數 SHALL 用 Dapper 的 `QueryMultipleAsync` 在同一次往返中一起取得，避免兩次查詢造成的額外網路延遲與資料不一致風險（兩次查詢之間資料可能被異動）。
- `categoryName` 排序在真實 SQL 中需要 `INNER JOIN Categories c ON c.Id = i.CategoryId`，這與目前 in-memory 版本（`categoryName` 已是 `ItemResponse` 上的既有屬性，直接 `OrderBy` 即可）在成本與寫法上有本質差異，註解需要點出這個落差，避免後續開發者誤以為兩者等價。

參考範例（將以此為基礎撰寫進 TODO 註解，實際措辭與縮排於 apply 階段依 [[code-annotation]] 慣例調整）：

```csharp
// TODO: 未來改用 SQL Server + Dapper 時的參考寫法。
// 重點：排序欄位不可把使用者字串直接拼進 SQL（SQL Injection 風險），
// 需先經白名單字典映射成實際 SQL 欄位名稱，再組進 ORDER BY。
//
// private static readonly Dictionary<string, string> SortColumnMap = new(StringComparer.OrdinalIgnoreCase)
// {
//     ["id"] = "i.Id",
//     ["name"] = "i.Name",
//     ["description"] = "i.Description",
//     ["categoryName"] = "c.Name",
//     ["createdDate"] = "i.CreatedDate",
// };
//
// var sortColumn = SortColumnMap.GetValueOrDefault(request.SortField, "i.Id");
// var sortDirection = string.Equals(request.SortOrder, "desc", StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC";
//
// const string filterSql = @"
//     WHERE (@Name IS NULL OR i.Name LIKE '%' + @Name + '%')
//       AND (@Description IS NULL OR i.Description LIKE '%' + @Description + '%')
//       AND (@DateFrom IS NULL OR i.CreatedDate >= @DateFrom)
//       AND (@DateTo IS NULL OR i.CreatedDate <= @DateTo)";
//
// var sql = $@"
//     SELECT i.Id, i.Name, i.Description, i.CategoryId, c.Name AS CategoryName, i.CreatedDate
//     FROM Items i
//     INNER JOIN Categories c ON c.Id = i.CategoryId
//     {filterSql}
//     ORDER BY {sortColumn} {sortDirection}
//     OFFSET @Skip ROWS FETCH NEXT @PageSize ROWS ONLY;
//
//     SELECT COUNT(*)
//     FROM Items i
//     {filterSql};
// ";
//
// using var multi = await db.QueryMultipleAsync(sql, new
// {
//     request.Name,
//     request.Description,
//     request.DateFrom,
//     request.DateTo,
//     Skip = (request.Page - 1) * request.PageSize,
//     request.PageSize
// });
//
// var items = (await multi.ReadAsync<ItemResponse>()).AsList();
// var total = await multi.ReadSingleAsync<int>();
```

此註解僅供閱讀參考，`ExampleItemsService.Search` 的實際執行邏輯仍維持 LINQ-to-objects 對 in-memory 清單操作，不引入 `IDbConnection` 或任何 Dapper 呼叫。

### 4a. `CategoryIds` 複選過濾在真實 SQL 中的對應方式（補充說明，非本次實作範圍）

TODO 註解中可視情況補一句提示：`categoryIds` 複選過濾在真實 SQL 中通常以 `STRING_SPLIT` 或 Table-Valued Parameter 處理，對應 `ExampleItemsController.cs` 現有註解「為何用 POST 而非 GET」的理由（陣列參數不易透過 query string 傳遞）。是否要在 TODO 中展開此範例由實作者在 apply 階段依篇幅拿捏，非必要項目。

## Risks / Trade-offs

- **[風險] TODO 註解與實際 in-memory 實作出現「文件與程式碼不同步」的觀感** → 緩解：註解明確標示「僅供未來參考，目前實作為 in-memory」，並集中放在 `Search` 方法附近，降低誤解風險。
- **[風險] 假資料日期以 `DateTime.Today.AddDays(-i)` 產生，跨日執行測試時資料會整批往前推一天，若既有測試斷言了絕對日期字串會失敗** → 緩解：測試/驗證應以相對關係（例如「第 1 筆日期晚於第 2 筆」或區間篩選後的筆數）斷言，不斷言絕對日期值。
- **[取捨] 日期篩選採單一 `DatePicker` range 模式而非兩個獨立日期輸入框** → 取捨：range 模式互動較單一化、程式碼較少，但無法個別清空起或訖其中一端；此為可接受的簡化，與現有篩選列「四欄縮減」的排版限制相符。

## Open Questions

（無，待 apply 階段若發現需要澄清的細節再補充）
