## 1. C# 共用基礎設施（Shared/）

- [x] 1.1 `ApiResponse.cs`、`ApiPagedResponse.cs`：補上用途與泛型結構說明
- [x] 1.2 `ExceptionHandlingMiddleware.cs`：說明全域例外攔截與回應格式
- [x] 1.3 `DatabaseExtensions.cs`：說明 Dapper 連線注入方式
- [x] 1.4 `Jwt/JwtOptions.cs`：說明各屬性對應 appsettings.json 的設定鍵
- [x] 1.5 `Jwt/JwtExtensions.cs`：說明 JWT 驗證設定流程
- [x] 1.6 `Jwt/IJwtService.cs`、`JwtService.cs`：說明 Token 產生邏輯與 claims 內容
- [x] 1.7 `Logging/SerilogHelper.cs`：說明 Serilog 初始化目的

## 2. C# Auth Feature

- [x] 2.1 `GroupFeatureStore.cs`：在 `_userGroups` 與 `_groupFeatures` 上方加上顯著 TODO 標注，說明這是 demo hardcode 轉換表，實際專案需替換為資料庫查詢
- [x] 2.2 `AuthController.cs`：說明各端點功能與 JWT 產生流程
- [x] 2.3 `AuthService.cs`：說明登入驗證邏輯與 features 取得流程
- [x] 2.4 `UserRepository.cs`：說明資料庫查詢與密碼驗證流程
- [x] 2.5 `PasswordHasherHelper.cs`：說明 BCrypt 使用方式（檔案不存在，跳過）
- [x] 2.6 `AuthExtensions.cs`：說明 DI 注入組合
- [x] 2.7 `LoginRequest.cs`、`LoginResponse.cs`、`MeResponse.cs`：必要時補充欄位說明

## 3. C# ExampleItems Feature

- [x] 3.1 `ExampleItemsController.cs`：說明各端點功能與分頁參數
- [x] 3.2 `ExampleItemsService.cs`：說明查詢邏輯與分頁排序處理
- [x] 3.3 `ExampleItemsSearchRequest.cs`：說明各搜尋參數意義
- [x] 3.4 `ExampleItemsExtensions.cs`：說明 DI 注入組合
- [x] 3.5 `ItemResponse.cs`：必要時補充欄位說明（自說明，無需加）

## 4. C# ExampleCategories Feature

- [x] 4.1 `ExampleCategoriesController.cs`：說明各端點功能
- [x] 4.2 `ExampleCategoriesService.cs`：說明查詢邏輯
- [x] 4.3 `ExampleCategoriesExtensions.cs`：說明 DI 注入組合
- [x] 4.4 `ExampleCategoryResponse.cs`：必要時補充欄位說明（自說明，無需加）

## 5. C# FeatureList Feature

- [x] 5.1 `FeaturesController.cs`：說明功能清單端點用途
- [x] 5.2 `FeaturesService.cs`：說明如何從 JWT claims 取出 features
- [x] 5.3 `FeaturesExtensions.cs`：說明 DI 注入組合
- [x] 5.4 `FeatureResponse.cs`：必要時補充欄位說明（自說明，無需加）

## 6. C# Menu Feature

- [x] 6.1 `MenuController.cs`：說明選單端點用途
- [x] 6.2 `MenuService.cs`：說明依 features 過濾選單節點的邏輯
- [x] 6.3 `MenuNode.cs`：說明樹狀選單節點結構
- [x] 6.4 `MenuExtensions.cs`：說明 DI 注入組合

## 7. C# Program.cs

- [x] 7.1 `Program.cs`：說明 Serilog 雙層 logger 設計（system vs feature）、中介軟體順序、全域 AuthorizeFilter

## 8. C# 測試程式碼

- [x] 8.1 `AuthServiceTests.cs`：說明測試結構與 NSubstitute mock 用法
- [x] 8.2 `ExampleItemsServiceTests.cs`：說明分頁測試情境
- [x] 8.3 `ExampleCategoriesServiceTests.cs`：說明測試情境
- [x] 8.4 `MenuServiceTests.cs`：說明 features 過濾測試邏輯

## 9. 前端基礎設施（TypeScript）

- [x] 9.1 `src/main.ts`：說明 Vue app 初始化與插件順序
- [x] 9.2 `src/router/index.ts`：說明路由保護邏輯與 `requiresAuth` meta 使用方式
- [x] 9.3 `src/lib/axios.ts`：說明 request interceptor（自動注入 Bearer token）與 response interceptor（401 處理、token 過期）
- [x] 9.4 `src/types/api.ts`：說明 `ApiResponse<T>`、`ApiPagedResponse<T>` 型別結構

## 10. 前端 API Layer

- [x] 10.1 `src/api/auth.api.ts`：說明登入與取得使用者資訊的 API 封裝
- [x] 10.2 `src/api/example-items.api.ts`：說明分頁搜尋參數與回應型別
- [x] 10.3 `src/api/example-categories.api.ts`：說明分類列表 API
- [x] 10.4 `src/api/menu.api.ts`：說明選單 API 結構
- [x] 10.5 `src/api/features.api.ts`：說明功能清單 API

## 11. 前端 Stores 與 Composables

- [x] 11.1 `src/stores/auth-store.ts`：說明登入狀態管理、token 存取方式
- [x] 11.2 `src/stores/user-info-store.ts`：說明使用者資訊快取與 `hasFeature` 用途
- [x] 11.3 `src/composables/useTheme.ts`：說明深淺色切換邏輯

## 12. 前端 Vue 元件

- [x] 12.1 `src/App.vue`：說明根元件結構
- [x] 12.2 `src/views/LoginView.vue`：說明表單驗證與登入流程
- [x] 12.3 `src/views/ExampleItemsView.vue`：說明分頁排序資料載入邏輯
- [x] 12.4 `src/views/layouts/MainLayout.vue`：說明整體版面結構
- [x] 12.5 `src/components/MainLayout/MainHeader.vue`：說明深淺色切換與使用者資訊顯示
- [x] 12.6 `src/components/MainLayout/MainSidebar.vue`：說明選單渲染與 feature 權限顯示邏輯
- [x] 12.7 `src/components/MainLayout/SidebarMenuItem.vue`：說明遞迴選單節點結構
- [x] 12.8 `src/components/NavigationProgress.vue`：說明路由切換進度條用途
- [x] 12.9 `src/views/DashboardView.vue`、`NotFoundView.vue`：必要時補充說明（自說明，無需加）

## 13. README 更新

- [x] 13.1 `vueappadmin.client/README.md`：測試指令改為 `npm run test -- --run`
- [x] 13.2 `VueAppAdmin.Server/README.md`：補上 Menu、FeatureList、ExampleCategories 的 API endpoint 說明
- [x] 13.3 `VueAppAdmin.Server.Tests/README.md`：補上 ExampleCategoriesServiceTests、MenuServiceTests 至目錄結構說明

## 14. template.json 確認

- [x] 14.1 對照 `launchSettings.json` 確認 `HttpPort`（5159）、`HttpsPort`（7173）、`IisPort`（21655）、`IisSslPort`（44385）的 `replaces` 值正確
- [x] 14.2 對照 `vite.config.ts` 確認 `SpaPort`（23288）的 `replaces` 值正確
- [x] 14.3 確認 `pnpm-workspace.yaml` 已在排除清單中，且無其他應排除但遺漏的檔案
