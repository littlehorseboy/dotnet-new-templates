## Why

`vue-app-admin-dotnet8` 的 client 端保留了 `create-vue` 預設模板的 CSS 與資源，這些殘留與 Bootstrap admin layout 直接衝突，導致 `MainLayout` 在桌面寬度下排版破版（body 被強制 `display: flex; place-items: center`，全螢幕骨架失效）。同時有多個未被任何元件引用的 CSS 變數、檔案與 import，增加維護時的混淆成本。

## What Changes

- **重寫 `main.css`**：移除 `#app` max-width/padding 限制、scaffold 連結樣式（`.green`、`a` green 色覆蓋）、與破壞 layout 的 `@media` block；保留 `base.css` import
- **重寫 `base.css`**：移除全部 `--vt-c-*` CSS 變數（約 20 個）、`--section-gap`、未使用的 dark mode `@media (prefers-color-scheme: dark)` block；保留 box-sizing reset 與 body 基礎樣式（font-family、font-size、min-height 等）
- **修改 `MainSidebar.vue`**：移除多餘的 `import { RouterLink }` （Vue Router `app.use(router)` 已全域注冊，不需明確 import）
- **移除 `assets/logo.svg`**：scaffold 殘留資源，無任何元件引用

## Capabilities

### New Capabilities

（無）

### Modified Capabilities

- `vue-app-admin-dotnet8/frontend-setup`：`main.css` 與 `base.css` 的內容規格調整，移除 scaffold 殘留樣式
- `vue-app-admin-dotnet8/frontend-layout`：`MainSidebar.vue` 的 import 規格調整，移除多餘的 `RouterLink` import

## Impact

- **受影響檔案**：`src/assets/main.css`、`src/assets/base.css`、`src/components/MainLayout/MainSidebar.vue`、`src/assets/logo.svg`
- **排版修正**：`MainLayout` 的 `vh-100 d-flex flex-column` 在桌面寬度下將正常填滿視窗，不再被 body flexbox 置中干擾
- **無 API / 後端影響**
- **無破壞性變更**（使用此 template 的開發者不依賴這些 scaffold CSS 的行為）
