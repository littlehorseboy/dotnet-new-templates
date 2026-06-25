## 1. API 通訊基礎建設

- [x] 1.1 建立 `src/types/api.ts`：定義 `ApiResponse<T>`、`LoginRequest`、`LoginResponse`、`MeResponse`、`ItemResponse`
- [x] 1.2 建立 `src/lib/axios.ts`：`axios.create()` instance，request interceptor 注入 Bearer token
- [x] 1.3 在 `src/lib/axios.ts` 加入 response interceptor：處理 `success: false`、401 自動登出（login endpoint 例外）、`isRedirectingToLogin` flag 防重複 redirect
- [x] 1.4 建立 `src/api/auth.api.ts`：`login()`、`getMe()`，unwrap `ApiResponse.result`
- [x] 1.5 建立 `src/api/example-items.api.ts`：`getAllItems()`（unwrap `results ?? []`）、`getItemById()`

## 2. 更新 Auth Store 與 User Info Store

- [x] 2.1 重寫 `src/stores/auth-store.ts`：移除 `init()`、移除所有 `axios.defaults.headers` 操作，僅保留 `login()`、`logout()`
- [x] 2.2 更新 `src/stores/user-info-store.ts`：改用 `getMe()`，新增 `isLoading: boolean`、`error: string | null` state，`fetchUserInfo()` 實作 loading/error 生命週期

## 3. 更新 Views 改用 API Modules

- [x] 3.1 更新 `src/views/LoginView.vue`：移除 `import axios`，改用 `login()`；新增 `isSubmitting` ref 控制按鈕 disabled + spinner；input 加入 `autocomplete`；錯誤訊息顯示 `err.message`
- [x] 3.2 更新 `src/views/ExampleItemsView.vue`：移除 inline `ExampleItem` interface，改用 `ItemResponse`；改用 `getAllItems()`；新增 `error` state 與重試按鈕

## 4. Router 強化

- [x] 4.1 在 `src/types/router.d.ts` 擴充 `RouteMeta`：新增 `sidebarIcon?: string`、`title?: string`
- [x] 4.2 更新 `src/router/index.ts`：根路由加入 `redirect: { name: 'dashboard' }`；各 route 加入 `meta.title` 與 `meta.sidebarIcon`；新增 catch-all 404 route 指向 `NotFoundView`；加入 `router.afterEach` 更新 `document.title`
- [x] 4.3 建立 `src/views/NotFoundView.vue`：顯示 404 訊息與「回首頁」連結

## 5. Layout 與 UX 元件

- [x] 5.1 建立 `src/components/NavigationProgress.vue`：使用 PrimeVue `ProgressBar`，hook `router.beforeEach`/`afterEach` 控制 width 動畫；`position: fixed` 頂部 3px；`:deep()` 覆蓋 PrimeVue 預設樣式
- [x] 5.2 更新 `src/App.vue`：掛載 `<NavigationProgress />`
- [x] 5.3 更新 `src/components/MainLayout/MainSidebar.vue`：加入 `<i class="bi" :class="route.meta.sidebarIcon">` icon 渲染（有 `sidebarIcon` 才顯示）
- [x] 5.4 更新 `src/components/MainLayout/MainHeader.vue`：logout 前加入 `confirm()` 確認，取消則不執行任何操作
- [x] 5.5 更新 `src/views/DashboardView.vue`：加入 4 個統計卡 placeholder（使用者/已完成/進行中/異常）及最近活動空狀態骨架

## 6. 清理啟動邏輯

- [x] 6.1 更新 `src/main.ts`：移除 `import { useAuthStore }`、移除 `authStore.init()` 呼叫
