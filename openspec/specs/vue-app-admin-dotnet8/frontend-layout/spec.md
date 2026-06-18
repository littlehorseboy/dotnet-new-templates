## ADDED Requirements

### Requirement: MainLayout 提供後台主版面骨架
`MainLayout.vue` SHALL 作為已登入頁面的 layout 元件，包含 `MainHeader`、`MainSidebar` 與主內容區 `<RouterView>`，透過 Vue Router 的 nested routes 機制掛載。

#### Scenario: 已登入頁面使用 MainLayout
- **WHEN** 已登入使用者進入 `/dashboard`
- **THEN** 頁面渲染 MainLayout（含 header 與 sidebar），內容區顯示 DashboardView

### Requirement: MainSidebar 從 router meta 衍生選單
`MainSidebar.vue` SHALL 讀取 router 中 `meta.showInSidebar === true` 的路由，自動產生選單連結（顯示 `meta.sidebarLabel`，連結至該路由 path）。

#### Scenario: 路由加入 meta.showInSidebar 後自動出現在選單
- **WHEN** router 中有路由設定 `meta: { showInSidebar: true, sidebarLabel: 'Example Items' }`
- **THEN** Sidebar 顯示該選單項目，點擊後導向對應路由

#### Scenario: 無 showInSidebar 的路由不出現在選單
- **WHEN** router 中有路由未設定 `meta.showInSidebar`（或設為 `false`）
- **THEN** Sidebar 不顯示該路由的選單項目

### Requirement: MainHeader 顯示應用名稱與登出按鈕
`MainHeader.vue` SHALL 顯示應用程式名稱（取自 env 或 hardcoded）及登出按鈕，點擊登出後呼叫 `auth-store.logout()` 並導向 `/login`。

#### Scenario: 點擊登出
- **WHEN** 已登入使用者點擊 Header 的登出按鈕
- **THEN** `auth-store.logout()` 被呼叫，router 導向 `/login`

### Requirement: DashboardView 提供登入後首頁 placeholder
`DashboardView.vue` SHALL 為簡單的 placeholder 頁面，顯示歡迎文字與登入使用者的 `displayName`，供開發者替換為實際 dashboard 內容。

#### Scenario: Dashboard 顯示使用者名稱
- **WHEN** 已登入使用者進入 `/dashboard`
- **THEN** 頁面顯示含 `user-info-store.displayName` 的歡迎訊息

### Requirement: ExampleItemsView 展示列表頁模式
`ExampleItemsView.vue` SHALL 呼叫 `GET /api/ExampleItems`，以 PrimeVue DataTable 顯示回傳的資料列表，並展示 Axios 呼叫與非同步載入的基本模式。

#### Scenario: 頁面載入時取得資料
- **WHEN** 使用者進入 ExampleItems 頁面
- **THEN** 頁面顯示 loading 狀態，API 回應後 DataTable 呈現資料列
