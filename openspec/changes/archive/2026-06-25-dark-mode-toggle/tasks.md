## 1. PrimeVue darkModeSelector 設定

- [x] 1.1 `main.ts` 的 PrimeVue 設定加入 `options: { darkModeSelector: '.p-dark' }`

## 2. 新增 useTheme composable

- [x] 2.1 建立 `src/composables/useTheme.ts`，以 `ref<boolean>` 管理 `isDark` 狀態
- [x] 2.2 實作 `init()`：讀取 localStorage `theme` key，無記錄時 fallback 至 `window.matchMedia('(prefers-color-scheme: dark)').matches`，並呼叫 `apply()`
- [x] 2.3 實作 `apply()`：同步設定 `document.documentElement.classList.toggle('p-dark', isDark.value)` 與 `document.documentElement.setAttribute('data-bs-theme', isDark.value ? 'dark' : 'light')`
- [x] 2.4 實作 `toggle()`：反轉 `isDark`，儲存至 localStorage，呼叫 `apply()`
- [x] 2.5 export `{ isDark, init, toggle }`

## 3. App.vue 初始化

- [x] 3.1 `App.vue` import `useTheme`，在 `onMounted` 呼叫 `init()`

## 4. MainHeader.vue 加入切換按鈕

- [x] 4.1 import `useTheme`，取得 `isDark` 與 `toggle`
- [x] 4.2 在 Header 右上角（登出按鈕左側）加入按鈕，深色時顯示 `bi-sun`，淺色時顯示 `bi-moon-stars-fill`，點擊呼叫 `toggle()`

## 5. 全域 Bootstrap 語義 class 適配（dark mode 連動）

- [x] 5.1 `MainHeader.vue` 移除 `navbar-dark bg-dark`，改用 `bg-body-tertiary`，讓 header 跟隨 `data-bs-theme` 自動切換
- [x] 5.2 `MainHeader.vue` 將 `text-white-50` 改為 `text-body-secondary`、`btn-outline-light` 改為 `btn-outline-secondary`
- [x] 5.3 `MainSidebar.vue` 移除背景 class（原 `bg-light` → `bg-body-secondary` → 無背景），保留 `border-end`

## 6. MainLayout sidebar 改為 CSS Grid

- [x] 6.1 `MainLayout.vue` 的 content wrapper 從 `d-flex` 改為 CSS Grid，`grid-template-columns: minmax(8rem, max-content) 1fr`
- [x] 6.2 `MainSidebar.vue` 移除 inline style `width: 220px; min-height: 100%`，改由 Grid column 控制寬度與撐高
