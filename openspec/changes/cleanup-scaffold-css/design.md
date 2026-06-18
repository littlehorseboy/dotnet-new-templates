## Context

`vue-app-admin-dotnet8` 的 client 端由 `create-vue` scaffold 初始化，預設產生的 `main.css` 與 `base.css` 包含 Vue 官方 demo 用的樣式。這些樣式在開發者開始撰寫 Bootstrap admin layout 後便成為殘留，部分規則直接與 Bootstrap utilities 衝突，導致可見的排版破版。此設計說明清除策略，並說明保留邊界。

## Goals / Non-Goals

**Goals:**
- 移除所有與 Bootstrap admin layout 衝突的 scaffold CSS 規則
- 移除未被任何元件引用的 CSS 自訂屬性與資源
- 修正 `MainSidebar.vue` 的多餘 import
- 確保清除後 layout 視覺行為與預期一致（`MainLayout` 全螢幕、header 固頂、sidebar 側欄、內容區可捲動）

**Non-Goals:**
- 引入新的 CSS 框架或設計 token 系統
- 修改任何 Bootstrap utility class 的用法
- 調整 `MainLayout`、`MainHeader`、`MainSidebar` 的結構或功能

## Decisions

### 決策 1：重寫 main.css，不保留 scaffold 規則

**選擇**：完整重寫 `main.css`，只保留 `@import './base.css'`。

**理由**：
- `#app { max-width: 1280px; padding: 2rem }` 限縮 admin layout，需移除
- `@media (min-width: 1024px) { body { display: flex; place-items: center } }` 是排版破版的直接原因
- `a, .green` 的綠色樣式覆蓋 Bootstrap `.nav-link` 色彩
- 沒有規則值得保留；「選擇性刪除」反而製造遺漏風險

**捨棄的替代方案**：用 override 壓掉問題規則（`#app { max-width: none }` 等）→ 留著垃圾，混淆維護者。

### 決策 2：重寫 base.css，只保留 body 基礎樣式

**選擇**：保留 box-sizing reset、`body` 的 font-family / font-size / min-height / line-height，移除全部 `--vt-c-*` 變數、`--section-gap`、dark mode `@media` block。

**理由**：
- `--vt-c-*` 是 Vue theme 系統專用，無任何元件引用
- Dark mode block 覆蓋 `background` 與 `color`，但 app 不支援深色模式，保留反而是潛在 bug 來源
- Body 的 font-family / font-size 對 Bootstrap 有合理的補充作用（Bootstrap 預設 font-family 相同，但明確設定有利於模板使用者客製化）

### 決策 3：移除 MainSidebar.vue 的 RouterLink import

**選擇**：直接刪除 `import { useRouter, RouterLink } from 'vue-router'` 中的 `RouterLink`，改為 `import { useRouter } from 'vue-router'`。

**理由**：`app.use(router)` 已將 `<RouterLink>` 全域注冊，`<script setup>` 中明確 import 是多餘的，且可能誤導維護者認為需要手動管理。

### 決策 4：移除 logo.svg

**選擇**：刪除 `src/assets/logo.svg`。

**理由**：無任何 `.vue`、`.ts`、`.css` 檔案 import 或引用此檔案；scaffold 殘留資源，保留只會讓模板使用者誤以為有意義。

## Risks / Trade-offs

- **風險**：模板使用者若已在自己的 fork 中引用 `--vt-c-*` 變數 → 緩解：這是 template 初始化後才修改，使用者不會有此依賴；且變數無語意價值，不應被引用
- **風險**：移除 `body font-family` 導致 Bootstrap 預設字型與原本不同 → 緩解：Bootstrap 5 `body` 字型與此處相同（`-apple-system`, `BlinkMacSystemFont`, `Segoe UI` 等），移除後行為不變
