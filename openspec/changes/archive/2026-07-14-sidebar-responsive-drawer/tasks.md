## 1. 共用開合狀態 composable

- [x] 1.1 新增 `vue-app-admin-dotnet8/vueappadmin.client/src/composables/useSidebarDrawer.ts`，比照 `useTheme.ts` 模式，以模組層級 `ref` 匯出 `isSidebarOpen`、`openSidebar`、`closeSidebar`、`toggleSidebar`

## 2. MainHeader 新增選單開關按鈕

- [x] 2.1 於 `MainHeader.vue` 品牌文字（`VueAppAdmin`）左側新增選單開關按鈕，使用 Bootstrap Icon `bi-list`
- [x] 2.2 按鈕加上 `d-md-none`，僅在 <768px 顯示
- [x] 2.3 按鈕 `@click` 綁定 `useSidebarDrawer` 的 `toggleSidebar`

## 3. MainSidebar 支援 Drawer 呈現

- [x] 3.1 於 `MainSidebar.vue` 匯入 PrimeVue `Drawer`（`import Drawer from 'primevue/drawer'`）
- [x] 3.2 新增 <768px 專用的 Drawer 渲染路徑，`v-model:visible` 綁定 `useSidebarDrawer` 的 `isSidebarOpen`，`position="left"`
- [x] 3.3 Drawer 內部重用既有 `menuNodes` 與 `SidebarMenuItem` 遞迴渲染，不重複實作選單邏輯
- [x] 3.4 確認 ≥768px 時維持現行固定欄位渲染路徑不變（以 CSS media query 或 Bootstrap 斷點類別控制顯示，而非條件渲染整個元件樹）
- [x] 3.5 調整 Drawer 局部樣式（寬度、z-index、陰影)，避免與既有 Bootstrap 樣式衝突

## 4. MainLayout 響應式版面調整

- [x] 4.1 於 `MainLayout.vue` 新增 <768px 的 CSS media query，將 Grid 版面改為單欄（`main` 佔滿寬度）
- [x] 4.2 確認 ≥768px 時 Grid 版面（`minmax(8rem, max-content) 1fr`）行為不變

## 5. 路由切換自動關閉 Drawer

- [x] 5.1 於 `MainLayout.vue`（或 router 層級）監聽路由變化，路由切換完成後呼叫 `closeSidebar()`

## 6. 驗證

- [x] 6.1 於瀏覽器 DevTools 響應式模式驗證 ≥768px 與 <768px 兩種斷點下的版面與按鈕顯示行為
- [x] 6.2 驗證窄螢幕下：開啟 Drawer、點擊背景遮罩關閉、點擊選單項目後自動關閉，三種情境皆正確
- [x] 6.3 驗證寬螢幕下版面與互動與變更前一致，無視覺或功能回歸
- [x] 6.4 更新 `openspec/specs/vue-app-admin-dotnet8/frontend-layout/spec.md`（依 delta spec 套用 MODIFIED/ADDED 內容）於 archive 階段完成
