## ADDED Requirements

### Requirement: NavigationProgress 顯示路由切換進度
`src/components/NavigationProgress.vue` SHALL 在 `App.vue` 最頂層掛載，使用 PrimeVue `ProgressBar`（`import ProgressBar from 'primevue/progressbar'`）實作頂部進度條。元件 SHALL 於 `router.beforeEach` 時顯示（width 從 30% 開始，200ms 後推進至 70%），於 `router.afterEach` 時完成至 100% 後淡出隱藏。進度條 SHALL 固定在頁面頂部（`position: fixed; top: 0`），高度 3px，z-index 9999。

#### Scenario: 路由切換時顯示進度條
- **WHEN** 使用者點擊 sidebar 連結觸發路由切換
- **THEN** 頁面頂部出現 3px 進度條，切換完成後淡出消失

#### Scenario: 快速切換路由進度條正確結束
- **WHEN** 路由切換完成（router.afterEach 觸發）
- **THEN** 進度條推進至 100% 並在 300ms 後隱藏，不殘留在畫面上

## MODIFIED Requirements

### Requirement: MainSidebar 從 router meta 衍生選單
`MainSidebar.vue` SHALL 讀取 router 中 `meta.showInSidebar === true` 的路由，自動產生選單連結。選單項目 SHALL 顯示 `meta.sidebarLabel` 文字，若路由設定了 `meta.sidebarIcon`（Bootstrap Icon class name，如 `bi-speedometer2`），SHALL 在文字前渲染對應的 `<i class="bi :class="sidebarIcon">` icon。元件 SHALL 依賴 Vue Router 的全域注冊取得 `<RouterLink>`，不得在 `<script setup>` 中明確 import `RouterLink`。

#### Scenario: 路由設有 sidebarIcon 時顯示 icon
- **WHEN** router 中有路由設定 `meta: { showInSidebar: true, sidebarLabel: 'Dashboard', sidebarIcon: 'bi-speedometer2' }`
- **THEN** Sidebar 顯示該選單項目，項目前方顯示 Bootstrap Icon `bi-speedometer2`

#### Scenario: 路由未設 sidebarIcon 時僅顯示文字
- **WHEN** router 中有路由設定 `meta.showInSidebar: true` 但無 `sidebarIcon`
- **THEN** Sidebar 顯示選單項目但不渲染 icon `<i>` 元素

#### Scenario: 路由加入 meta.showInSidebar 後自動出現在選單
- **WHEN** router 中有路由設定 `meta: { showInSidebar: true, sidebarLabel: 'Example Items' }`
- **THEN** Sidebar 顯示該選單項目，點擊後導向對應路由

### Requirement: MainHeader 顯示應用名稱與登出按鈕
`MainHeader.vue` SHALL 顯示應用程式名稱及登出按鈕，點擊登出後 SHALL 以 `confirm()` 請使用者確認，確認後呼叫 `auth-store.logout()` 並導向 `/login`；取消則不執行任何操作。

#### Scenario: 點擊登出並確認
- **WHEN** 已登入使用者點擊 Header 的登出按鈕並在 confirm 對話框選擇確定
- **THEN** `auth-store.logout()` 被呼叫，router 導向 `/login`

#### Scenario: 點擊登出但取消
- **WHEN** 已登入使用者點擊 Header 的登出按鈕並在 confirm 對話框選擇取消
- **THEN** 不執行任何操作，使用者維持在當前頁面

### Requirement: DashboardView 提供統計卡與活動記錄骨架
`DashboardView.vue` SHALL 包含 4 個統計卡（使用者、已完成、進行中、異常）作為 placeholder，以及最近活動區塊（初始顯示「尚無活動記錄」空狀態）。統計卡 SHALL 使用 Bootstrap Icon 搭配對應顏色（primary/success/warning/danger）。

#### Scenario: Dashboard 顯示使用者名稱與統計骨架
- **WHEN** 已登入使用者進入 `/dashboard`
- **THEN** 頁面顯示含 `user-info-store.displayName` 的歡迎訊息，4 個統計卡可見，最近活動顯示空狀態

### Requirement: ExampleItemsView 展示列表頁模式含錯誤處理
`ExampleItemsView.vue` SHALL 以 `getAllItems()` 從 `src/api/example-items.api.ts` 取得資料，以 PrimeVue DataTable 顯示。元件 SHALL 管理 `loading`、`error` state；API 失敗時 SHALL 顯示含錯誤訊息的 alert 與「重試」按鈕，點擊重試按鈕 SHALL 重新呼叫 `loadItems()`。

#### Scenario: 頁面載入時取得資料
- **WHEN** 使用者進入 ExampleItems 頁面
- **THEN** 頁面顯示 loading 狀態，API 回應後 DataTable 呈現資料列

#### Scenario: API 失敗顯示錯誤訊息與重試按鈕
- **WHEN** `getAllItems()` 拋出 Error
- **THEN** 頁面顯示 alert 含錯誤訊息，以及「重試」按鈕；不顯示空的 DataTable

#### Scenario: 點擊重試重新載入
- **WHEN** 使用者點擊「重試」按鈕
- **THEN** `loadItems()` 被重新呼叫，loading state 重置，嘗試再次取得資料

### Requirement: router 支援 404 頁面與 document.title 更新
`router/index.ts` SHALL 包含 catch-all route（`path: '/:pathMatch(.*)*'`），指向 `NotFoundView.vue`，meta 設定 `noAuthRequired: true`。router SHALL 設定 `redirect: { name: 'dashboard' }` 於根路由 `/`，使直接進入 `/` 自動導向 dashboard。每個 route SHALL 設定 `meta.title`，`router.afterEach` SHALL 更新 `document.title` 為 `<title> | VueAppAdmin`，無 title 時 fallback 為 `VueAppAdmin`。

#### Scenario: 進入未定義路由顯示 404 頁面
- **WHEN** 使用者進入不存在的路徑（如 `/not-exist`）
- **THEN** 渲染 `NotFoundView.vue`，顯示 404 訊息與返回首頁連結

#### Scenario: 進入 / 自動導向 dashboard
- **WHEN** 已登入使用者進入 `/`
- **THEN** router redirect 至 `/dashboard`

#### Scenario: route 切換後 document.title 更新
- **WHEN** 使用者進入 `/dashboard`（`meta.title: 'Dashboard'`）
- **THEN** `document.title` 為 `Dashboard | VueAppAdmin`
