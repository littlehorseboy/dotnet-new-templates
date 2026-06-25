## Why

`vue-app-admin-dotnet8` 目前無法手動切換深色／淺色主題，只能被動跟隨系統偏好。作為 template，應示範使用者可主動控制的 dark mode toggle，並持久化偏好。

## What Changes

- **`main.ts`** 加入 `darkModeSelector: '.p-dark'`，讓 PrimeVue 以 class 切換深色主題
- **新增 `composables/useTheme.ts`** 管理 dark mode 狀態：讀取 localStorage，初始值 fallback 至 `prefers-color-scheme`，toggle 時同步更新 `html` 的 `.p-dark` class 與 `data-bs-theme` attribute
- **`MainHeader.vue`** 右上角加入太陽／月亮切換按鈕（Bootstrap Icons：`bi-sun` / `bi-moon-stars-fill`）

## Capabilities

**New Capabilities**
- `vue-app-admin-dotnet8/theme-toggle` — 使用者可手動切換深色／淺色主題，偏好持久化於 localStorage

**Modified Capabilities**
- （無）

## Impact

- **受影響檔案**：`main.ts`、`MainHeader.vue`、新增 `composables/useTheme.ts`
- **PrimeVue dark mode**：透過 `html.p-dark` 觸發，需設定 `darkModeSelector`
- **Bootstrap dark mode**：透過 `html[data-bs-theme="dark"]` 觸發（Bootstrap 5.3+ 原生支援）
- **不影響**：後端、路由、auth 流程、其他元件
