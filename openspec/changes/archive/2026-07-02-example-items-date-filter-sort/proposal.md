## Why

`example-items-search` 目前的清單只示範了文字模糊查詢與類別複選，缺少日期欄位與日期區間查詢，排序也只開放 `name`、`description` 兩欄（其餘 fallback 到 `id`）。這使範本無法示範「日期查詢條件」與「全欄位排序」這兩種常見的清單頁需求。此外，`ExampleItemsService` 依 [[data-access]] 的既有規範會維持 in-memory 假資料，但目前完全沒有任何線索告訴後續開發者「真的接資料庫時 SQL 該怎麼寫」，尤其是 SQL Server `OFFSET/FETCH` 分頁搭配動態排序時，如何避免把使用者輸入的排序欄位直接拼進 SQL（SQL Injection 風險）。這次一併把這段示範以 TODO 註解的形式補進 `ExampleItemsService.cs`，讓範本更完整。

## What Changes

- `ItemResponse` 新增 `createdDate: DateTime` 欄位，in-memory 假資料需產生有差異的日期（非全部同一天），才能驗證排序與區間篩選有效。
- `ExampleItemsSearchRequest` 新增 `dateFrom: DateTime?`、`dateTo: DateTime?` 兩個選填欄位，代表 `createdDate` 的查詢區間（含首尾兩端點）。
- `ExampleItemsService.Search` 的排序邏輯 SHALL 補齊所有欄位分支：`id`、`name`、`description`、`categoryName`、`createdDate`，不再有欄位 fallback 到 `id` 的情況（未知欄位名稱除外，仍 fallback 到 `id`）。
- `ExampleItemsService.cs` 新增一段 TODO 註解，示範未來改用 SQL Server + Dapper 時的 `OFFSET/FETCH` 分頁查詢寫法：包含以白名單字典做動態排序欄位映射（避免 SQL Injection）、`QueryMultipleAsync` 一次取得分頁資料與總筆數、`categoryName` 排序需要 `JOIN Categories` 的說明。此註解僅供閱讀參考，**不**改變 `ExampleItemsService` 現行 in-memory 實作（維持 [[data-access]] 既有規範）。
- 前端 `src/types/api.ts` 的 `ExampleItemsSearchRequest` 型別鏡射同步新增 `dateFrom`、`dateTo`。
- `ExampleItemsView.vue` 篩選列新增日期區間篩選（PrimeVue `DatePicker`，`selectionMode="range"`），與既有「查詢」按鈕行為一致：選擇日期不自動觸發查詢，需點擊查詢按鈕或在其他文字輸入框按 Enter。
- `ExampleItemsView.vue` 表格新增 `createdDate` 欄位（顯示格式待 design.md 決定），並將所有欄位（`id`、`name`、`description`、`categoryName`、`createdDate`）開啟 `sortable`。

## Capabilities

### New Capabilities

（無）

### Modified Capabilities

- `vue-app-admin-dotnet8/example-items-search`：`ItemResponse` 新增日期欄位、`Search` request 新增日期區間條件、排序支援擴及所有欄位、前端篩選 UI 新增日期區間選擇器。

## Impact

- 後端：`Features/ExampleItems/Responses/ItemResponse.cs`、`Features/ExampleItems/Requests/ExampleItemsSearchRequest.cs`、`Features/ExampleItems/ExampleItemsService.cs`。
- 前端：`vueappadmin.client/src/types/api.ts`、`vueappadmin.client/src/views/ExampleItemsView.vue`。
- 不影響資料庫（仍為 in-memory），不影響 `IDbConnection`/`Dapper` 既有註冊方式，不新增 NuGet 或 npm 相依套件（PrimeVue `DatePicker` 已隨 PrimeVue 4.5.5 內建）。
- 假資料筆數不變（30 筆），僅補上日期欄位。
