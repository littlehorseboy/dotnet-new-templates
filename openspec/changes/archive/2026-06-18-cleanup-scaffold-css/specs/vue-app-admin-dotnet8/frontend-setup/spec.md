## ADDED Requirements

### Requirement: main.css 不含 scaffold 殘留樣式
`main.css` SHALL 只包含 `@import './base.css'`，不得包含 `#app` 的 max-width / padding 限制、`.green` class、`a` 的 scaffold 連結樣式、或任何 `@media` 將 `body` 設為 `display: flex` 的規則。

#### Scenario: 桌面寬度下 body 不被 flexbox 置中
- **WHEN** 瀏覽器視窗寬度 ≥ 1024px
- **THEN** `body` 不帶 `display: flex`，`MainLayout` 的 `vh-100 d-flex flex-column` 正常填滿視窗高度

#### Scenario: #app 不限制最大寬度
- **WHEN** `main.css` 被載入
- **THEN** `#app` 元素無 `max-width` 與 `padding` 限制，admin layout 可填滿全部可用空間

### Requirement: base.css 只保留 body 基礎樣式
`base.css` SHALL 包含 box-sizing reset（`*, *::before, *::after { box-sizing: border-box }`）與 `body` 的 `min-height`、`font-family`、`font-size`、`line-height` 設定，不得包含 `--vt-c-*` CSS 自訂屬性、`--section-gap`、semantic color 變數、或 `@media (prefers-color-scheme: dark)` block。

#### Scenario: 無 --vt-c-* 變數宣告
- **WHEN** 開發者在元件中嘗試使用 `var(--vt-c-white)`
- **THEN** 變數無值（undefined），不存在於 CSS 中

#### Scenario: body 基礎樣式正常套用
- **WHEN** 應用程式載入
- **THEN** `body` 套用正確的 `font-family`（`Inter, -apple-system, ...`）與 `font-size: 15px`
