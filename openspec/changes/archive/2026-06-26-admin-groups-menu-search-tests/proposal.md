## Why

`vue-app-admin-dotnet8` 範本目前缺乏可示範的權限管理、動態選單與進階查詢模式，對照真實後台系統差距明顯，不足以作為產品級範本的參考起點。此次擴充補齊這些核心能力，同時補上前後端單元測試，讓範本具備完整可複製的架構示範。

## What Changes

- **新增 Group + Feature 權限系統**：User → Groups → Features 三層結構，hardcode in-memory，GetMe response 擴充 `groups[]`、`features[]`，前端加入 `hasFeature()` helper
- **新增 GET /api/Features**：回傳所有可用 feature 清單
- **新增樹狀選單 API**：`POST /api/Menu/Items`，後端依 JWT claims 中的 features 過濾節點後回傳，MainSidebar.vue 改為遞迴渲染並支援展開/收合
- **ExampleItems 查詢擴充**：原 `GET /api/ExampleItems` 改為 `POST /api/ExampleItems/Search`，支援名稱/說明模糊查詢與類別複選過濾；`ItemResponse` 加入 `categoryId`、`categoryName`
- **新增 POST /api/ExampleCategories**：回傳類別清單，前端 PrimeVue MultiSelect 使用
- **後端單元測試**：在既有 `VueAppAdmin.Server.Tests` 專案（xUnit + NSubstitute）補上 Service 層測試案例
- **前端單元測試**：安裝 Vitest + @vue/test-utils + happy-dom，涵蓋 composables 與 stores

## Capabilities

### New Capabilities

- `group-feature-permissions`：AWS 風格的 Group + Feature 權限模型，in-memory 實作，涵蓋後端 API 與前端 helper
- `tree-menu`：後端過濾的樹狀選單，MenuNode 含 `requiredFeature`，前端遞迴渲染
- `example-items-search`：POST 查詢 API，支援模糊查詢與類別複選，ExampleItems 資料加入 category 關聯
- `example-categories-api`：類別清單 API，供 ExampleItems 篩選的 MultiSelect 使用
- `unit-tests`：前後端單元測試基礎架構與示範測試案例

### Modified Capabilities

（無）

## Impact

- **後端 API**：新增 Features、Menu、ExampleCategories 端點；ExampleItems GET 改為 POST；AuthController GetMe response schema 變更（**BREAKING** 對現有前端消費者）
- **前端**：`user-info-store` 新增 `groups`、`features`、`hasFeature()`；`MainSidebar.vue` 大幅重構為遞迴元件；`ExampleItemsView.vue` 加入篩選 UI；新增 `menu.api.ts`、`categories.api.ts`
- **相依套件**：前端加入 `vitest`、`@vue/test-utils`、`happy-dom`、`@vitest/coverage-v8`；後端測試專案已就緒，無需新增套件
- **資料結構**：30 筆 ExampleItems 加入 categoryId 欄位
