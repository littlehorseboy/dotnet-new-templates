## Context

`vue-app-admin-dotnet8` 範本的後台版面（`MainLayout.vue` + `MainHeader.vue` + `MainSidebar.vue`）目前完全以 Bootstrap 5 CSS 類別與固定 CSS Grid 建構，`MainSidebar` 永遠佔用 `minmax(8rem, max-content)` 欄寬，沒有任何響應式行為。專案已安裝 `primevue@4.5.5`，但目前僅用於資料呈現元件（`DataTable`、`ProgressBar` 等），版面骨架尚未使用任何 PrimeVue 元件。`useTheme.ts` composable 已示範「以模組層級 `ref` + 具名 export 函式」共享跨元件狀態的既有慣例（無需 Pinia store），本次沿用相同模式管理 Drawer 開合狀態。

## Goals / Non-Goals

**Goals:**
- 在 <768px（Bootstrap `md` 斷點）時，側邊欄改以 PrimeVue `Drawer`（覆蓋層）呈現，不再佔用版面寬度。
- 在 ≥768px 時，維持現行固定欄位行為與現有視覺，不造成回歸。
- Header 新增選單開關按鈕，僅在窄螢幕顯示。
- 選單項目點擊、背景遮罩點擊、路由切換後，Drawer 自動關閉。
- 沿用專案既有的 composable 共享狀態慣例（比照 `useTheme.ts`），不引入 Pinia store 或新的狀態管理套件。

**Non-Goals:**
- 不重新設計側邊欄選單的視覺樣式、圖示或巢狀展開邏輯（`SidebarMenuItem.vue` 內部邏輯不變）。
- 不將整體版面從 Bootstrap 遷移至 PrimeVue（僅 Drawer 這個元件本身使用 PrimeVue）。
- 不處理 Header 右側使用者名稱/登出按鈕在極窄螢幕下的排版（非本次需求範圍，如有嚴重溢出問題可另開 change）。
- 不新增自訂斷點系統或 `useBreakpoint` 通用 composable；僅以 CSS media query／Bootstrap `d-md-*` 工具類別判斷顯示與否，Drawer 開合邏輯本身不需要 JS 讀取視窗寬度。

## Decisions

### 1. 使用 PrimeVue `Drawer`（非自製 off-canvas）
專案已依賴 `primevue@4.5.5`，`Drawer` 是官方元件（v4 將舊版 `Sidebar` 更名為 `Drawer`），內建 overlay、背景遮罩點擊關閉、ESC 關閉、`position="left"` 等行為，不需要自行處理 focus trap／z-index／transition。相較於純 CSS 手刻 off-canvas，可降低可維護性風險與程式碼量。

### 2. 斷點策略：CSS media query 決定「顯示形式」，JS 只管「開合狀態」
- ≥768px：`MainSidebar` 以現行固定欄位渲染（`MainLayout.vue` Grid 不變）；Drawer 版本不渲染或不影響版面（PrimeVue `Drawer` 未開啟時預設不佔版面空間）。
- <768px：`MainLayout.vue` Grid 改為單欄（僅 `main` 佔滿寬度），側邊欄改由 `MainSidebar` 內的 `Drawer` 呈現。
- 採用 CSS media query（`@media (min-width: 768px)`）或 Bootstrap 現有斷點工具類別判斷「目前該用哪種呈現形式」，避免額外引入 `window.matchMedia` 監聽器造成的 resize 事件處理複雜度與 SSR/初始渲染閃爍風險。
- Drawer 開關按鈕本身用 `d-md-none` 只在窄螢幕顯示；≥768px 時按鈕隱藏，因為側邊欄已固定可見。

### 3. 共享開合狀態：`useSidebarDrawer` composable（比照 `useTheme.ts` 模式）
在 `src/composables/useSidebarDrawer.ts` 建立模組層級 `const isSidebarOpen = ref(false)`，export `isSidebarOpen`、`openSidebar`、`closeSidebar`、`toggleSidebar`。`MainHeader.vue` 呼叫 `toggleSidebar`，`MainSidebar.vue`（或 `MainLayout.vue`）將 `isSidebarOpen` 綁定至 `Drawer` 的 `v-model:visible`。此模式與現有 `useTheme.ts` 一致，維持專案風格一致性，不需要新增 Pinia store。

### 4. Drawer 內容重用 `SidebarMenuItem`
`Drawer` 內部直接複用現有 `menuNodes` 資料與 `SidebarMenuItem` 遞迴元件，避免重複實作選單渲染邏輯；`MainSidebar.vue` 內同時保留「固定欄位」與「Drawer」兩種渲染路徑，共用同一份 `menuNodes` state 與 `getMenuItems()` 呼叫。

### 5. 路由切換與選單點擊時自動關閉
在 `router.afterEach`（或 `MainLayout.vue` 監聽 `route.fullPath` 變化）時呼叫 `closeSidebar()`；`SidebarMenuItem` 的葉節點點擊（`RouterLink` 導向）觸發路由切換即可間接觸發關閉，不需要在 `SidebarMenuItem` 內额外綁定關閉邏輯，降低元件之間耦合。

## Risks / Trade-offs

- **[風險] Drawer 與固定欄位重複渲染 `SidebarMenuItem`，若處理不當可能導致兩份 DOM 同時存在或狀態不同步** → 以 CSS media query 控制何者可見（而非 `v-if` 條件渲染整個元件樹),並確保 `menuNodes` 是單一資料來源（同一個 `ref`),兩種呈現只是視覺容器不同。
- **[風險] 現有已客製化 `MainSidebar.vue`/`MainHeader.vue`/`MainLayout.vue` 的既有專案，套用此範本更新時需要手動合併** → 已於 proposal 的 BREAKING 段落註明，屬範本更新的預期行為，非本次程式碼變更可避免。
- **[風險] PrimeVue Drawer 預設樣式（z-index、寬度、陰影）可能與 Bootstrap 既有樣式衝突** → 實作時以 PrimeVue 官方 `Drawer` props（如 `style`/`pt`）局部覆寫寬度與陰影，並在窄螢幕實機／DevTools 響應式模式下驗證無視覺衝突。
- **[取捨] 未引入通用 `useBreakpoint` composable** → 目前僅這一處需要斷點判斷，先以 CSS media query 解決；若未來有更多元件需要 JS 層級的斷點狀態，再另開 change 抽象化。

## Migration Plan

此變更僅影響範本原始碼（`vue-app-admin-dotnet8`），無資料庫或執行期資料遷移。已由範本產生的既有專案不會自動套用此變更，需使用者依範本更新流程手動比對合併 `MainHeader.vue`、`MainSidebar.vue`、`MainLayout.vue` 與新增的 `useSidebarDrawer.ts`。無需 rollback 機制；如需回退，直接還原上述檔案即可。
