## Context

PrimeVue 4 Aura 以 `darkModeSelector` 控制深色主題切換方式，預設為 `'system'`（跟隨 `prefers-color-scheme`）。Bootstrap 5.3+ 支援 `data-bs-theme="dark"` 屬性切換深色主題。兩者皆可透過操作 `<html>` 元素觸發，無需額外套件。

## Goals / Non-Goals

**Goals:**
- 使用者點擊按鈕可即時切換深色／淺色主題
- 初始值優先讀取 localStorage，無記錄時 fallback 至系統偏好（`prefers-color-scheme`）
- PrimeVue 與 Bootstrap 的深色主題同步切換

**Non-Goals:**
- 不做每個頁面獨立主題設定
- 不支援其他 theme preset（Lara、Material 等）切換
- 不做 SSR / hydration 考量

## Decisions

### 決策 1：darkModeSelector 設為 `.p-dark`

```ts
app.use(PrimeVue, {
    theme: {
        preset: Aura,
        options: { darkModeSelector: '.p-dark' }
    }
});
```

**原因**：class-based 讓 JS 完全掌控切換時機，不受系統偏好影響。`'system'` 模式無法讓使用者手動覆蓋。

### 決策 2：composable useTheme()

```ts
// composables/useTheme.ts
const isDark = ref(false)

function init() {
    const saved = localStorage.getItem('theme')
    if (saved) {
        isDark.value = saved === 'dark'
    } else {
        isDark.value = window.matchMedia('(prefers-color-scheme: dark)').matches
    }
    apply()
}

function toggle() {
    isDark.value = !isDark.value
    localStorage.setItem('theme', isDark.value ? 'dark' : 'light')
    apply()
}

function apply() {
    document.documentElement.classList.toggle('p-dark', isDark.value)
    document.documentElement.setAttribute('data-bs-theme', isDark.value ? 'dark' : 'light')
}
```

**原因**：composable 比 Pinia store 輕量，不需要跨元件共享複雜狀態；theme 狀態是純 UI 行為，不屬於 app domain state。

### 決策 3：icon 使用 Bootstrap Icons

- 淺色模式顯示 `bi-moon-stars-fill`（點擊切換到深色）
- 深色模式顯示 `bi-sun`（點擊切換到淺色）

**原因**：專案已引入 `bootstrap-icons`，不需額外依賴。

### 決策 4：init() 在 App.vue 的 onMounted 呼叫

在根元件 mount 時執行 `init()`，確保所有子元件渲染前 `<html>` 的 class 已正確設置。

## Risks / Trade-offs

- **FOUC（Flash of Unstyled Content）** → `init()` 在 `onMounted` 執行，Vue mount 前有短暫 flash。可接受範圍，template 示範用途不需要 SSR-level 解法
- **Bootstrap dark mode 覆蓋問題** → `MainHeader.vue` 目前硬寫 `navbar-dark bg-dark`，deep dark 模式下視覺可能重疊，可接受，不在本次修改範圍

## Open Questions

（無）
