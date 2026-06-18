## 1. 清除 main.css

- [x] 1.1 重寫 `vueappadmin.client/src/assets/main.css`，只保留 `@import './base.css'`，移除所有 scaffold 規則（`#app`、`.green`、`a` 連結樣式、`@media (min-width: 1024px)` block）
- [x] 1.2 驗證：瀏覽器開啟應用，桌面寬度下 `body` 無 `display: flex`，`MainLayout` 填滿整個視窗高度

## 2. 清除 base.css

- [x] 2.1 重寫 `vueappadmin.client/src/assets/base.css`，移除所有 `--vt-c-*` 自訂屬性、`--section-gap`、semantic color 變數、`@media (prefers-color-scheme: dark)` block，只保留 box-sizing reset 與 `body` 基礎樣式
- [x] 2.2 驗證：DevTools Elements 面板確認 `:root` 無 `--vt-c-*` 變數宣告

## 3. 修正 MainSidebar.vue import

- [x] 3.1 修改 `vueappadmin.client/src/components/MainLayout/MainSidebar.vue` 的 import，從 `import { useRouter, RouterLink } from 'vue-router'` 改為 `import { useRouter } from 'vue-router'`
- [x] 3.2 驗證：Sidebar 選單連結仍正常顯示並可導航

## 4. 移除 logo.svg

- [x] 4.1 確認 `vueappadmin.client/src/assets/logo.svg` 未被任何 `.vue`、`.ts`、`.css` 引用（grep 確認）
- [x] 4.2 刪除 `vueappadmin.client/src/assets/logo.svg`
