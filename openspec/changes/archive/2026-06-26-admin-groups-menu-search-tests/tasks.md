## 1. 後端：Group + Feature 權限模型

- [x] 1.1 在 `Features/Auth/` 新增 `GroupFeatureStore.cs`，hardcode User → Groups → Features 映射（admin / viewer）
- [x] 1.2 修改 `AuthService.cs`，登入驗證後取得使用者的 groups 與 features
- [x] 1.3 修改 `JwtService.cs`，將 features[] 序列化為逗號分隔字串寫入 JWT claim `features`
- [x] 1.4 修改 `MeResponse.cs`，加入 `Groups: List<string>` 與 `Features: List<string>`
- [x] 1.5 修改 `AuthController.cs` 的 `GetMe`，從 JWT claims 讀取 features 並填入 response
- [x] 1.6 新增 `Features/FeatureList/` 目錄，建立 `FeaturesController.cs`、`FeaturesService.cs`、`IFeaturesService.cs`、`FeaturesExtensions.cs`，實作 `GET /api/Features`
- [x] 1.7 在 `Program.cs` 註冊 FeaturesService

## 2. 後端：ExampleCategories

- [x] 2.1 新增 `Features/ExampleCategories/` 目錄，建立 `ExampleCategoryResponse.cs`（id, name）
- [x] 2.2 建立 `ExampleCategoriesService.cs`、`IExampleCategoriesService.cs`，hardcode 3 筆資料
- [x] 2.3 建立 `ExampleCategoriesController.cs`，實作 `POST /api/ExampleCategories`
- [x] 2.4 建立 `ExampleCategoriesExtensions.cs`，在 `Program.cs` 完成 DI 註冊

## 3. 後端：ExampleItems 擴充

- [x] 3.1 修改 `ItemResponse.cs`，加入 `CategoryId: int` 與 `CategoryName: string`
- [x] 3.2 修改 `ExampleItemsService.cs`，30 筆資料各自加入 categoryId（1~3 隨意分配），並參照 ExampleCategoriesService 填入 categoryName
- [x] 3.3 新增 `ExampleItemsSearchRequest.cs`（page, pageSize, sortField, sortOrder, name, description, categoryIds[]）
- [x] 3.4 修改 `ExampleItemsService.cs`，實作依 name/description 模糊查詢（contains, 不分大小寫）與 categoryIds 複選過濾
- [x] 3.5 修改 `ExampleItemsController.cs`，移除 `GET /api/ExampleItems`，新增 `POST /api/ExampleItems/Search`

## 4. 後端：樹狀選單

- [x] 4.1 新增 `Features/Menu/` 目錄，建立 `MenuNode.cs`（id, label, icon, route, requiredFeature, children）
- [x] 4.2 建立 `MenuService.cs`、`IMenuService.cs`，hardcode 完整選單樹
- [x] 4.3 在 `MenuService.cs` 實作 `GetFilteredMenu(IEnumerable<string> features)` 遞迴過濾邏輯（含群組節點子全空時自動移除）
- [x] 4.4 建立 `MenuController.cs`，實作 `POST /api/Menu/Items`，從 JWT claims 讀取 features 後呼叫過濾
- [x] 4.5 建立 `MenuExtensions.cs`，在 `Program.cs` 完成 DI 註冊

## 5. 前端：型別與 API 層

- [x] 5.1 修改 `types/api.ts`，更新 `MeResponse`（加入 groups, features），新增 `FeatureResponse`、`MenuNode`、`ExampleCategoryResponse`、`ExampleItemsSearchRequest`
- [x] 5.2 修改 `api/auth.api.ts`，更新 `getMe()` 回傳型別
- [x] 5.3 新增 `api/features.api.ts`，實作 `getFeatures()`
- [x] 5.4 新增 `api/menu.api.ts`，實作 `getMenuItems()`（POST）
- [x] 5.5 新增 `api/example-categories.api.ts`，實作 `getExampleCategories()`（POST）
- [x] 5.6 修改 `api/example-items.api.ts`，移除 `getAllItems`，新增 `searchItems(request: ExampleItemsSearchRequest)`（POST）

## 6. 前端：Pinia Stores 擴充

- [x] 6.1 修改 `user-info-store.ts`，加入 `groups: string[]`、`features: string[]` state
- [x] 6.2 在 `user-info-store.ts` 新增 `hasFeature(feature: string): boolean` 方法
- [x] 6.3 修改 `fetchUserInfo()` 正確填入 groups 與 features

## 7. 前端：MainSidebar.vue 遞迴選單

- [x] 7.1 新增 `components/MainLayout/SidebarMenuItem.vue`，接受單一 `MenuNode` prop，遞迴渲染子節點，群組節點支援展開/收合狀態
- [x] 7.2 修改 `MainSidebar.vue`，移除靜態 router.meta 邏輯，改為頁面載入時呼叫 `getMenuItems()`，以 `SidebarMenuItem` 渲染回傳的樹
- [x] 7.3 驗證 admin 登入後顯示完整選單，viewer 登入後「類別管理」與「系統管理」不顯示

## 8. 前端：ExampleItemsView 篩選 UI

- [x] 8.1 在 `ExampleItemsView.vue` 加入篩選列：名稱輸入框、說明輸入框（debounce 300ms）
- [x] 8.2 加入 PrimeVue MultiSelect 類別複選，頁面初始化時呼叫 `getExampleCategories()` 填入選項
- [x] 8.3 將原 `getAllItems` 呼叫改為 `searchItems`，篩選條件變更時重新查詢（重置 page 為 1）
- [x] 8.4 表格欄位加入 CategoryName 欄

## 9. 前端：Vitest 測試基礎架構

- [x] 9.1 安裝 `vitest`、`@vue/test-utils`、`happy-dom`、`@vitest/coverage-v8`
- [x] 9.2 修改 `vite.config.ts`，加入 `test: { environment: 'happy-dom' }` 設定
- [x] 9.3 在 `package.json` 加入 `"test": "vitest"` 與 `"test:coverage": "vitest run --coverage"` 指令

## 10. 前端：單元測試

- [x] 10.1 新增 `src/composables/__tests__/useTheme.spec.ts`：測試 init 讀取 localStorage、toggle 切換 isDark 並寫入 localStorage
- [x] 10.2 新增 `src/stores/__tests__/auth-store.spec.ts`：測試 login/logout 與 isAuthenticated 狀態
- [x] 10.3 新增 `src/stores/__tests__/user-info-store.spec.ts`：測試 `hasFeature()` 有/無 feature 時回傳正確布林值

## 11. 後端：單元測試

- [x] 11.1 新增 `Features/Auth/AuthServiceTests.cs`：正確密碼驗證成功、錯誤密碼失敗
- [x] 11.2 新增 `Features/ExampleItems/ExampleItemsServiceTests.cs`：分頁、名稱模糊查詢、類別過濾
- [x] 11.3 新增 `Features/ExampleCategories/ExampleCategoriesServiceTests.cs`：回傳 3 筆資料
- [x] 11.4 新增 `Features/Menu/MenuServiceTests.cs`：admin 完整樹、viewer 過濾結果、群組節點子全空自動移除
