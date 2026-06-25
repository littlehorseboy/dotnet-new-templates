## Why

Vue client 原本直接在 view/store 裡呼叫 `axios`，缺乏集中的 API 通訊層，導致 token 注入、錯誤處理邏輯散落各處，且 server 的 `ApiResponse<T>` 包裝沒有對應的 TypeScript 型別。同時 UX 有幾個明顯缺漏：登入按鈕無 loading state、API 失敗無錯誤提示、缺少 404 頁面、route 切換無進度回饋。

## What Changes

**API 通訊層（第一批）**
- 新增 `src/lib/axios.ts`：中央 axios instance，request interceptor 自動注入 Bearer token，response interceptor 統一處理 `ApiResponse<T>` 的 `success: false`、401 自動登出（login endpoint 除外以防誤 redirect）
- 新增 `src/types/api.ts`：對應 server `ApiResponse<T>` 的 TS 型別，以及 `LoginRequest`、`LoginResponse`、`MeResponse`、`ItemResponse`
- 新增 `src/api/auth.api.ts`：`login()`、`getMe()`
- 新增 `src/api/example-items.api.ts`：`getAllItems()`、`getItemById()`
- 更新 `auth-store.ts`：移除 axios header 操作，token 由 interceptor 全權管理
- 更新 `user-info-store.ts`：改用 `getMe()`，新增 `isLoading`、`error` state
- 更新 `LoginView.vue`、`ExampleItemsView.vue`：所有 axios 呼叫改用 API modules
- 更新 `main.ts`：移除已無用的 `authStore.init()`

**UX 精進（第二批）**
- 新增 `src/types/router.d.ts`：擴充 `RouteMeta` 型別（`sidebarIcon`、`title`）
- 新增 `src/views/NotFoundView.vue`：404 頁面
- 新增 `src/components/NavigationProgress.vue`：頂部 route 切換進度條（使用 PrimeVue `ProgressBar`）
- 更新 `App.vue`：掛載 `NavigationProgress`
- 更新 `router/index.ts`：404 catch-all route、`/` redirect to dashboard、`title` meta、`afterEach` 更新 `document.title`
- 更新 `LoginView.vue`：`isSubmitting` state、按鈕 spinner + disable、`autocomplete`
- 更新 `ExampleItemsView.vue`：error state + 重試按鈕
- 更新 `DashboardView.vue`：4 個統計卡 + 最近活動骨架
- 更新 `MainHeader.vue`：logout 前 `confirm()` 確認
- 更新 `MainSidebar.vue`：根據 `sidebarIcon` meta 顯示 Bootstrap Icons

## Capabilities

### New Capabilities
- `frontend-api-layer`：集中的 API 通訊層，包含 axios instance、response interceptors、feature 級 API modules、與 server `ApiResponse<T>` 對應的 TS 型別

### Modified Capabilities
- `frontend-auth`：token 管理責任從 auth-store 移至 axios interceptor；login 失敗的 401 不觸發 redirect；user-info-store 新增 loading/error state
- `frontend-layout`：sidebar 加入 icon 支援；router 加入 404 catch-all、`/` redirect、`document.title` 更新；App 層加入 route 切換進度條
- `frontend-setup`：移除 `authStore.init()` 啟動邏輯

## Impact

- **受影響的 views/stores**：`LoginView.vue`、`ExampleItemsView.vue`、`DashboardView.vue`、`auth-store.ts`、`user-info-store.ts`
- **受影響的 components**：`MainHeader.vue`、`MainSidebar.vue`、`App.vue`
- **新增依賴**：無（PrimeVue `ProgressBar` 已在現有依賴中）
- **行為異動**：`authStore.init()` 不再需要呼叫；登入帳密錯誤的錯誤訊息現在直接顯示 server 回傳的 `message`
