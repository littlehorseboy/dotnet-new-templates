## 1. 後端：新增 ApiPagedResponse\<T\>

- [x] 1.1 在 `VueAppAdmin.Server/Shared/` 新增 `ApiPagedResponse.cs`，繼承 `ApiResponse<T>`，加入 `Total` 屬性與 `OkPaged(IEnumerable<T> results, int total)` 靜態工廠方法

## 2. 後端：擴充假資料與 Service 分頁排序邏輯

- [x] 2.1 `ExampleItemsService` 的 in-memory 假資料擴充至 30 筆，各筆 `Name` 與 `Description` 值有所差異
- [x] 2.2 新增 `GetPaged(int skip, int top, string sortField, string sortOrder)` 方法，依 `sortField` switch 對應 LINQ 排序屬性（`id`、`name`、`description`），預設回退 `id`；`sortOrder` 非 `"desc"` 一律視為 `"asc"`；排序後執行 `.Skip(skip).Take(top)` 並回傳資料與 `total`

## 3. 後端：更新 Controller

- [x] 3.1 `ExampleItemsController.GetAll` 加入 `[FromQuery] int skip = 0`、`[FromQuery] int top = 10`、`[FromQuery] string sortField = "id"`、`[FromQuery] string sortOrder = "asc"` 參數
- [x] 3.2 呼叫 `GetPaged`，以 `ApiPagedResponse<ItemResponse>.OkPaged(results, total)` 回傳

## 4. 前端：新增型別

- [x] 4.1 `types/api.ts` 新增 `ApiPagedResponse<T>` 介面，繼承 `ApiResponse<T>` 的欄位並加入 `total: number`

## 5. 前端：更新 API 函式

- [x] 5.1 `example-items.api.ts` 的 `getAllItems` 加入參數 `(params: { skip: number; top: number; sortField: string; sortOrder: string })`，以 query string 傳入，回傳型別改為 `{ items: ItemResponse[]; total: number }`

## 6. 前端：更新 ExampleItemsView

- [x] 6.1 新增 reactive 狀態：`totalRecords`（number）、`lazyParams`（含 `first`、`rows`、`sortField`、`sortOrder`）
- [x] 6.2 DataTable 加入 `:lazy="true"`、`:paginator="true"`、`:rows="10"`、`:totalRecords="totalRecords"`、`@page="onPage"`、`@sort="onSort"`
- [x] 6.3 `id`、`name`、`description` 欄加入 `:sortable="true"`（id 欄不需排序可省略）
- [x] 6.4 實作 `onPage(event)` 與 `onSort(event)` handler，更新 `lazyParams` 後呼叫 `loadItems`
- [x] 6.5 `loadItems` 改為以 `lazyParams` 的 `first`/`rows`/`sortField`/`sortOrder` 呼叫 `getAllItems`，並更新 `totalRecords`
- [x] 6.6 `onMounted` 以預設 lazyParams（skip=0, top=10, sortField="id", sortOrder="asc"）呼叫 `loadItems`

