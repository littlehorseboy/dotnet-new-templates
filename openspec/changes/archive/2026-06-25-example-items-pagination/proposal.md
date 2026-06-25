## Why

`vue-app-admin-dotnet8` 的 `ExampleItems` 列表目前一次回傳全部資料，缺乏分頁與排序示範。作為 template，應展示前後端協作的 server-side 分頁與欄位排序標準模式，供使用者複製至真實功能。

## What Changes

- **後端** `ExampleItemsController.GetAll` 新增接收 `skip`、`top`、`sortField`、`sortOrder` query parameters
- **後端** `ExampleItemsService` 擴充假資料至 30 筆，支援 Skip / Take / OrderBy 邏輯
- **後端** 新增 `ApiPagedResponse<T>` 型別，繼承 `ApiResponse<T>` 並加入 `Total` 欄位
- **前端** `example-items.api.ts` 的 `getAllItems` 加入分頁排序參數，回傳型別改為 `ApiPagedResponse`
- **前端** `ExampleItemsView.vue` 改用 PrimeVue DataTable lazy 模式，處理 `@page` / `@sort` 事件
- **前端** `types/api.ts` 新增 `ApiPagedResponse<T>` 介面

## Capabilities

**New Capabilities**
- `vue-app-admin-dotnet8/example-items-pagination` — ExampleItems 列表的 server-side 分頁與欄位排序端到端行為

**Modified Capabilities**
- `vue-app-admin-dotnet8/response-contracts` — 新增 `ApiPagedResponse<T>` 作為分頁端點專用回傳型別及其工廠方法

## Impact

- **受影響 API**：`GET /api/ExampleItems`（query params 新增，response 型別改變）
- **受影響元件**：`ExampleItemsView.vue`、`example-items.api.ts`、`types/api.ts`
- **受影響後端**：`ExampleItemsController`、`ExampleItemsService`、`ApiPagedResponse<T>`（新增）
- **不影響**：其他 controller、auth 流程、現有 `ApiResponse<T>` 合約
- **Breaking**：`GET /api/ExampleItems` response schema 新增 `total` 欄位（既有欄位不變，向後相容）
