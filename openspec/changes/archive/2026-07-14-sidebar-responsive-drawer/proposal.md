## Why

`vue-app-admin-dotnet8` 範本的 `MainSidebar.vue` 目前是永遠顯示的固定欄位（透過 `MainLayout.vue` 的 CSS Grid `minmax(8rem, max-content) 1fr` 呈現），在窄螢幕（如平板直式、手機）下會擠壓主內容區的可用寬度，且沒有任何斷點或收合機制。範本標榜可直接作為後台起手式使用，缺少基本 RWD 支援會讓使用者在小螢幕裝置上難以使用，需要補上響應式側邊欄行為。

## What Changes

- `MainSidebar.vue` 在小螢幕（< 768px，對應 Bootstrap `md` 斷點）時，不再以 CSS Grid 固定欄位呈現，改以 PrimeVue `Drawer` 元件（`primevue/drawer`，v4 元件名稱）以覆蓋層（overlay）形式顯示；在 ≥768px 維持現行固定欄位行為不變。
- `MainHeader.vue` 新增選單開關按鈕（漢堡圖示 `bi-list`），放置於品牌文字（`VueAppAdmin`）左側；按鈕僅在 <768px 時顯示（`d-md-none`），點擊切換 Drawer 開關狀態。
- 新增共享的側邊欄開合狀態（例如 `useSidebarDrawer` composable，比照現有 `useTheme.ts` 的模式），供 `MainHeader`（觸發開關）與 `MainSidebar`/`MainLayout`（渲染 Drawer）共用。
- 使用者點擊 Drawer 內的選單項目、點擊背景遮罩，或路由切換完成後，Drawer SHALL 自動關閉。
- `MainLayout.vue` 的 Grid 版面在 <768px 時 SHALL 調整為單欄（主內容區佔滿寬度），側邊欄改由 Drawer 覆蓋顯示，不佔版面寬度。

**BREAKING**：無對外 API 或路由變更，僅影響 `vueappadmin.client` 前端版面元件的內部結構；若有專案已直接客製化 `MainSidebar.vue`/`MainHeader.vue`/`MainLayout.vue`，套用此範本更新時需要重新比對合併。

## Capabilities

### New Capabilities

（無）

### Modified Capabilities

- `frontend-layout`：`MainSidebar` 新增小螢幕下的 Drawer 覆蓋層行為與斷點規則；`MainHeader` 新增選單開關按鈕；`MainLayout` 的版面 Grid 規則新增響應式斷點行為。

## Impact

- 受影響檔案：
  - `vue-app-admin-dotnet8/vueappadmin.client/src/components/MainLayout/MainHeader.vue`
  - `vue-app-admin-dotnet8/vueappadmin.client/src/components/MainLayout/MainSidebar.vue`
  - `vue-app-admin-dotnet8/vueappadmin.client/src/views/layouts/MainLayout.vue`
  - 新增 `vue-app-admin-dotnet8/vueappadmin.client/src/composables/useSidebarDrawer.ts`
- 依賴：改用專案已安裝的 `primevue@4.5.5` 提供的 `Drawer` 元件（`primevue/drawer`），不需新增套件；沿用既有 `bootstrap-icons`（`bi-list`）與 Bootstrap `md` 斷點（768px）判斷邏輯。
- 不影響後端 API、路由結構或既有選單資料來源（`getMenuItems()`）。
