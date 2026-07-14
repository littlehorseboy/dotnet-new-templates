## MODIFIED Requirements

### Requirement: MainLayout 提供後台主版面骨架
`MainLayout.vue` SHALL 作為已登入頁面的 layout 元件，包含 `MainHeader`、`MainSidebar` 與主內容區 `<RouterView>`，透過 Vue Router 的 nested routes 機制掛載。在螢幕寬度 ≥768px（Bootstrap `md` 斷點）時，`MainLayout.vue` SHALL 以 CSS Grid（`minmax(8rem, max-content) 1fr`）並排顯示固定側邊欄與主內容區。在螢幕寬度 <768px 時，`MainLayout.vue` SHALL 改為單欄版面，主內容區 SHALL 佔滿可用寬度，側邊欄不佔用版面空間，改由 `MainSidebar` 內的覆蓋層（Drawer）呈現。

#### Scenario: 已登入頁面使用 MainLayout
- **WHEN** 已登入使用者進入 `/dashboard`
- **THEN** 頁面渲染 MainLayout（含 header 與 sidebar），內容區顯示 DashboardView

#### Scenario: 寬螢幕維持固定雙欄版面
- **WHEN** 已登入使用者在螢幕寬度 ≥768px 檢視任一頁面
- **THEN** 版面以 CSS Grid 並排顯示固定側邊欄與主內容區，與現行行為一致

#### Scenario: 窄螢幕改為單欄版面
- **WHEN** 已登入使用者在螢幕寬度 <768px 檢視任一頁面
- **THEN** 主內容區佔滿可用寬度，側邊欄不佔用版面空間

### Requirement: MainHeader 顯示應用名稱、選單開關與登出按鈕
`MainHeader.vue` SHALL 顯示應用程式名稱及登出按鈕，點擊登出後 SHALL 以 `confirm()` 請使用者確認，確認後呼叫 `auth-store.logout()` 並導向 `/login`；取消則不執行任何操作。`MainHeader.vue` SHALL 在應用程式名稱左側提供選單開關按鈕（Bootstrap Icon `bi-list`），此按鈕 SHALL 僅在螢幕寬度 <768px 時顯示；點擊此按鈕 SHALL 切換側邊欄 Drawer 的開合狀態（透過共用的開合狀態管理，如 `useSidebarDrawer`）。

#### Scenario: 點擊登出並確認
- **WHEN** 已登入使用者點擊 Header 的登出按鈕並在 confirm 對話框選擇確定
- **THEN** `auth-store.logout()` 被呼叫，router 導向 `/login`

#### Scenario: 點擊登出但取消
- **WHEN** 已登入使用者點擊 Header 的登出按鈕並在 confirm 對話框選擇取消
- **THEN** 不執行任何操作，使用者維持在當前頁面

#### Scenario: 窄螢幕顯示選單開關按鈕
- **WHEN** 已登入使用者在螢幕寬度 <768px 檢視 Header
- **THEN** 應用程式名稱左側顯示選單開關按鈕（`bi-list`）

#### Scenario: 寬螢幕不顯示選單開關按鈕
- **WHEN** 已登入使用者在螢幕寬度 ≥768px 檢視 Header
- **THEN** 選單開關按鈕不顯示（側邊欄已固定可見）

#### Scenario: 點擊選單開關按鈕開啟側邊欄
- **WHEN** 使用者在螢幕寬度 <768px 點擊選單開關按鈕，且側邊欄 Drawer 目前關閉
- **THEN** 側邊欄 Drawer 開啟並以覆蓋層顯示於畫面上

## ADDED Requirements

### Requirement: MainSidebar 於窄螢幕以 Drawer 呈現
在螢幕寬度 <768px 時，`MainSidebar.vue` SHALL 以 PrimeVue `Drawer`（`import Drawer from 'primevue/drawer'`）呈現選單內容，Drawer 的開合狀態 SHALL 綁定共用的側邊欄開合狀態（如 `useSidebarDrawer` composable 匯出的 `isSidebarOpen`）。Drawer 內部 SHALL 重用既有的 `menuNodes` 資料與 `SidebarMenuItem` 遞迴元件渲染選單，不得重複實作選單資料擷取或渲染邏輯。在螢幕寬度 ≥768px 時，側邊欄 SHALL 維持現行固定欄位呈現方式，不使用 Drawer 覆蓋層。

#### Scenario: 窄螢幕以 Drawer 呈現選單
- **WHEN** 已登入使用者在螢幕寬度 <768px 開啟側邊欄
- **THEN** 選單內容以覆蓋層（Drawer）形式顯示在畫面上，覆蓋主內容區而非擠壓其寬度

#### Scenario: 點擊 Drawer 背景遮罩關閉側邊欄
- **WHEN** 側邊欄 Drawer 開啟中，使用者點擊 Drawer 以外的背景遮罩區域
- **THEN** Drawer 關閉

#### Scenario: 點擊選單項目後自動關閉 Drawer
- **WHEN** 側邊欄 Drawer 開啟中，使用者點擊其中一個選單項目觸發路由切換
- **THEN** 路由完成切換後，Drawer SHALL 自動關閉

#### Scenario: 寬螢幕維持固定欄位呈現
- **WHEN** 已登入使用者在螢幕寬度 ≥768px 檢視頁面
- **THEN** 側邊欄以現行固定欄位方式顯示，不透過 Drawer 覆蓋層呈現

### Requirement: useSidebarDrawer 提供跨元件共用的側邊欄開合狀態
`src/composables/useSidebarDrawer.ts` SHALL 以模組層級狀態（比照 `useTheme.ts` 的模式）匯出側邊欄開合狀態與操作函式（至少包含表示目前是否開啟的狀態，以及開啟、關閉、切換三種操作），供 `MainHeader.vue` 與 `MainSidebar.vue` 共用同一份開合狀態，不透過元件 props/emit 傳遞。

#### Scenario: Header 與 Sidebar 共用同一份開合狀態
- **WHEN** `MainHeader.vue` 呼叫 composable 提供的切換操作
- **THEN** `MainSidebar.vue` 綁定的開合狀態同步更新，Drawer 對應開啟或關閉
