## 1. 後端資料模型

- [x] 1.1 `Features/ExampleItems/Responses/ItemResponse.cs` 新增 `CreatedDate: DateTime` 屬性
- [x] 1.2 `Features/ExampleItems/Requests/ExampleItemsSearchRequest.cs` 新增 `DateFrom: DateTime?`、`DateTo: DateTime?` 屬性（皆選填，預設 `null`）

## 2. 後端假資料與查詢邏輯

- [x] 2.1 `ExampleItemsService.cs` 假資料產生器改用 `DateTime.Today.AddDays(-i)`（`i` 為 1~30 索引）填入 `CreatedDate`，確保 30 筆資料日期彼此不同
- [x] 2.2 `Search` 方法新增日期區間過濾：`dateFrom` 存在時過濾 `CreatedDate >= dateFrom`，`dateTo` 存在時過濾 `CreatedDate <= dateTo`（LINQ `Where`）
- [x] 2.3 `Search` 方法的排序 `switch` 補齊 `categoryName`、`createdDate` 兩個分支（連同既有 `id`、`name`、`description`），確認未知欄位仍 fallback 為 `id` 排序
- [x] 2.4 在 `Search` 方法附近新增 TODO 註解，依 design.md 的「未來 SQL Server + Dapper 寫法」範例撰寫（白名單動態排序、`OFFSET/FETCH` 分頁、`QueryMultipleAsync` 一次取得分頁資料與總筆數、`categoryName` 排序需要 `JOIN Categories` 的說明），並註明此為參考範例、不改變現行 in-memory 實作

## 3. 前端型別與 API 呼叫

- [x] 3.1 `vueappadmin.client/src/types/api.ts` 的 `ExampleItemsSearchRequest` 型別新增 `dateFrom?: string`、`dateTo?: string`（或依專案慣例的日期型別表示法）
- [x] 3.2 確認 `src/api/example-items.api.ts` 的 `searchItems` 呼叫不需額外調整（僅透傳 request 物件）

## 4. 前端篩選 UI

- [x] 4.1 `ExampleItemsView.vue` 匯入 PrimeVue `DatePicker`（`primevue/datepicker`），加入 `selectionMode="range"` 的日期區間篩選欄位，绑定 `filterDateRange` ref
- [x] 4.2 `buildRequest()` 依 `filterDateRange` 組出 `dateFrom`/`dateTo`（僅選了起或訖其中一端時對應欄位為 `null`）
- [x] 4.3 調整篩選列排版以容納第五個控件（日期區間），維持所有控件高度一致
- [x] 4.4 確認日期區間選擇不會自動觸發查詢，維持既有「按查詢按鈕或於文字輸入框按 Enter」的互動模式

## 5. 前端表格欄位與排序

- [x] 5.1 `ExampleItemsView.vue` 表格新增 `createdDate` 欄位（`<Column field="createdDate" sortable>`），決定顯示格式（例如 `YYYY-MM-DD`）
- [x] 5.2 為既有 `id`、`categoryName` 欄位補上 `sortable`（`name`、`description` 已具備）

## 6. 驗證

- [x] 6.1 手動測試：依 `createdDate` 升冪/降冪排序，確認結果正確（以自動化測試 `Search_SortByCreatedDateDesc_ReturnsNewestFirst` 覆蓋 desc，asc 為既有預設排序路徑）
- [x] 6.2 手動測試：依 `categoryName` 排序，確認結果正確（含中文排序行為是否符合預期）（自動化測試 `Search_SortByCategoryNameDesc_ReturnsDescendingOrder`，以 `StringComparer.Ordinal` 驗證與 LINQ `OrderByDescending` 預設比較邏輯一致）
- [x] 6.3 手動測試：日期區間篩選（僅起始、僅結束、兩者皆帶）分別驗證回傳筆數與內容（自動化測試 `Search_ByDateRange_ReturnsOnlyItemsWithinRange`、`Search_DateFromOnly_ReturnsItemsOnOrAfterDate`、`Search_DateToOnly_ReturnsItemsOnOrBeforeDate`）
- [x] 6.4 手動測試：日期區間篩選與名稱/類別條件複合查詢（自動化測試 `Search_DateRangeWithNameFilter_ReturnsItemsMatchingBoth`）
- [x] 6.5 確認既有測試（若有涵蓋 `ExampleItemsView`/`ExampleItemsService`）仍通過，並視需要補上新欄位/排序/日期篩選的測試案例（後端 13/13、前端 4/4 全數通過；`dotnet build`、`vue-tsc --noEmit` 皆無錯誤）
